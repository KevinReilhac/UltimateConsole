using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UltimateConsole;
using UltimateConsole.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UltimateConsole.Editor.Settings;

namespace UltimateConsole.Editor.Window
{
    public class UltimateConsoleWindow : EditorWindow, IULogHandler
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset = default;
        [SerializeField] private VisualTreeAsset debugLineTemplate;

        private VisualElement logLinesContainer = null;
        private LogLine currentSelectedLogLine = null;
        private Label stackTraceText = null;
        private UltimateConsoleSettings settings = null;
        private Label detailsText;
        private LogFilters logFilters = new LogFilters();

        private Dictionary<ULog, List<LogLine>> logLinesFromULog = new Dictionary<ULog, List<LogLine>>();
        private LogTypesToggles logTypesToggles;
        private bool isCollapsed = false;
        private bool isPauseOnError = false;

        private const string PARENTHESIS_FORMAT_PATTERN = @"Assets[/\\][^()]+\.cs\((\d+),(\d+)\)";
        private const string COLON_FORMAT_PATTERN = @"Assets[/\\][^:]+\.cs:(\d+)";

        [MenuItem("Window/UI Toolkit/UltimateConsoleWindow")]
        public static void ShowExample()
        {
            UltimateConsoleWindow wnd = GetWindow<UltimateConsoleWindow>();
            wnd.titleContent = new GUIContent("Ultimate Console");
        }

        private void OnEnable()
        {
            logFilters.OnLogFilterChanged += Refresh;
            LogLine.OnLogLineSelected += SelectLogLine;
            LogLine.OnLogLineDoubleClicked += OnLogLineDoubleClicked;
            settings = UltimateConsoleSettings.GetOrCreateSettings();
            Application.logMessageReceived += OnUnityConsoleLog;
            UConsole.RegisterLogHandler(this);
        }

        private void OnDisable()
        {
            logFilters.OnLogFilterChanged -= Refresh;
            LogLine.OnLogLineSelected -= SelectLogLine;
            LogLine.OnLogLineDoubleClicked -= OnLogLineDoubleClicked;
            Application.logMessageReceived -= OnUnityConsoleLog;
            UConsole.UnRegisterLogHandler(this);
        }

        private void Refresh()
        {
            bool checkLog = false;
            logTypesToggles?.ResetCounts();

            /// Hide loglines that are not in the filters
            foreach (KeyValuePair<ULog, List<LogLine>> logLines in logLinesFromULog)
            {
                checkLog = logFilters.CheckLog(logLines.Key);
                for (int i = 0; i < logLines.Value.Count; i++)
                {
                    logTypesToggles?.IncrementLogCount(logLines.Key.logType);
                    if (isCollapsed)
                    {
                        if (i == 0)
                        {
                            logLines.Value[i].style.display = checkLog ? DisplayStyle.Flex : DisplayStyle.None;
                            logLines.Value[i].CollapsedCount = logLines.Value.Count;
                        }
                        else
                            logLines.Value[i].style.display = DisplayStyle.None;
                    }
                    else
                    {
                        logLines.Value[i].style.display = checkLog ? DisplayStyle.Flex : DisplayStyle.None;
                        logLines.Value[i].CollapsedCount = 0;
                    }
                }
            }
        }

        private void SelectLogLine(LogLine logLine)
        {
            //Ping context if it exists, Before checking if it is the same logline to letting the user spam
            if (logLine != null && logLine.Log.HasValue && logLine.Log.Value.context != null)
                EditorGUIUtility.PingObject(logLine.Log.Value.context as UnityEngine.Object);

            //Check if the logline is the same as the current selected logline
            if (currentSelectedLogLine == logLine) return;

            //If there is a current selected logline, set it to not selected
            if (currentSelectedLogLine != null)
                currentSelectedLogLine.IsSelected = false;
            currentSelectedLogLine = logLine;

            UpdateDetailsText(logLine);
        }

        private void OnLogLineDoubleClicked(LogLine logLine)
        {
            //If the logline is an error, try to open the parenthesis format (ex: Assets/Scripts/Main.cs(84,20))
            if (logLine.LogType == LogType.Error)
            {
                if (TryOpenParenthesisFormat(logLine.Log.Value.message))
                    return;
            }

            //Then try to open the colon format (ex: Assets/Scripts/Main.cs:84)
            TryOpenColonFormat(logLine.Log.Value.message);
        }

        private bool TryOpenParenthesisFormat(string message)
        {
            Regex parenthesisFormat = new Regex(PARENTHESIS_FORMAT_PATTERN);
            Match match = parenthesisFormat.Match(message);

            if (!match.Success) return false;

            string fullPath = match.Groups[0].Value;
            string lineNumberStr = match.Groups[1].Value;
            string columnNumberStr = match.Groups[2].Value;
            fullPath = fullPath.Substring(0, fullPath.IndexOf("("));

            if (int.TryParse(lineNumberStr, out int lineNumber))
            {
                InternalEditorUtility.OpenFileAtLineExternal(fullPath, lineNumber);
                return true;
            }
            return false;
        }

        private bool TryOpenColonFormat(string message)
        {
            Regex colonFormat = new Regex(COLON_FORMAT_PATTERN);
            Match match = colonFormat.Match(message);

            if (!match.Success) return false;

            string fullPath = match.Groups[0].Value;
            string lineNumberStr = match.Groups[1].Value;
            fullPath = fullPath.Substring(0, fullPath.IndexOf(":"));

            if (int.TryParse(lineNumberStr, out int lineNumber))
            {
                InternalEditorUtility.OpenFileAtLineExternal(fullPath, lineNumber);
                return true;
            }
            return false;
        }

        private void UpdateDetailsText(LogLine logLine)
        {
            //Update details text and stacktrace text
            if (logLine != null)
            {
                detailsText.text = logLine.Log.Value.message;
                SetStackTraceText(logLine.Log.Value.stacktrace);
            }
            else
            {
                detailsText.text = string.Empty;
                SetStackTraceText(string.Empty);
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualTreeAsset.CloneTree(root);

            stackTraceText = root.Q<Label>("StackTraceText");
            stackTraceText.text = string.Empty;
            logLinesContainer = root.Q<ScrollView>("LogLineContainer").contentContainer;
            logLinesContainer.Clear();

            MaskField chanelsDropDown = root.Q<MaskField>("ChanelsDropdown");
            chanelsDropDown.RegisterValueChangedCallback(OnChanelsChange);
            chanelsDropDown.value = UltimateConsoleWindowPrefs.Chanels;
            detailsText = root.Q<Label>("DetailsText");
            detailsText.text = string.Empty;

            logTypesToggles = root.Q<LogTypesToggles>();
            logTypesToggles.SetLogTypes(UltimateConsoleWindowPrefs.GetEnabledLogTypes(), false);
            logTypesToggles.OnLogTypesChanged += SetLogTypes;

            // Register search field
            ToolbarSearchField toolbarSearchField = root.Q<ToolbarSearchField>();
            toolbarSearchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChange);

            // Register clear button
            ToolbarButton clearButton = root.Q<ToolbarButton>("ClearButton");
            clearButton.clickable.clicked += OnClearLogs;

            // Register collapse button
            ToolbarToggle collapseButton = root.Q<ToolbarToggle>("CollapseToggle");
            collapseButton.RegisterCallback<ChangeEvent<bool>>(OnCollapseToggleChange);
            collapseButton.value = UltimateConsoleWindowPrefs.CollapseEnabled;

            ToolbarToggle pauseOnErrorToggle = root.Q<ToolbarToggle>("ErrorPause");
            pauseOnErrorToggle.RegisterCallback<ChangeEvent<bool>>(OnPauseOnErrorToggleChange);
            pauseOnErrorToggle.value = UltimateConsoleWindowPrefs.ErrorPauseEnabled;
            SetupDropdownFields(chanelsDropDown);
            RegisterStackTraceTextLinks(stackTraceText);

            logFilters.UpdateLogTypes(UltimateConsoleWindowPrefs.GetEnabledLogTypes());
        }

        private void SetLogTypes(List<LogType> list)
        {
            logFilters.UpdateLogTypes(list);
            UltimateConsoleWindowPrefs.SetEnabledLogTypes(list);
            Refresh();
        }

        #region UI_CALLBACKS

        private void OnChanelsChange(ChangeEvent<int> evt)
        {
            logFilters.Chanels = evt.newValue;
            UltimateConsoleWindowPrefs.Chanels = evt.newValue;
        }

        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            logFilters.SearchText = evt.newValue;
        }

        private void OnCollapseToggleChange(ChangeEvent<bool> evt)
        {
            isCollapsed = evt.newValue;
            UltimateConsoleWindowPrefs.CollapseEnabled = isCollapsed;
            Refresh();
        }

        private void OnPauseOnErrorToggleChange(ChangeEvent<bool> evt)
        {
            isPauseOnError = evt.newValue;
            UltimateConsoleWindowPrefs.ErrorPauseEnabled = isPauseOnError;
        }

        #endregion

        private void SetupDropdownFields(MaskField chanelsDropdown)
        {
            List<string> enumNames = new List<string>() { "Default" };
            enumNames.AddRange(settings.chanelSettings.Select(s => s.Name));

            chanelsDropdown.choices = enumNames;
        }

        private LogLine CreateLine(ULog log)
        {
            LogLine logLine = new LogLine();
            logLine.Log = log;
            logLinesContainer.Add(logLine);

            return logLine;
        }

        #region StackTrace

        private void RegisterStackTraceTextLinks(Label label)
        {
            label.RegisterCallback<PointerUpLinkTagEvent>(StackTraceHyperLinkUp);
        }

        private void SetStackTraceText(string stackTraceString)
        {
            //Remove 3 lines
            stackTraceString = string.Join("\n\n", stackTraceString.Split('\n').Skip(3).ToArray());

            //Place links tags
            string pattern = @"\(\s*at\s*(.*?)\s*\)";
            string replacement = "<link=\"1\"><color=#40a0ff><u>$1</u></color></link>";

            string result = Regex.Replace(stackTraceString, pattern, replacement);
            stackTraceText.text = result;
        }

        private void StackTraceHyperLinkUp(PointerUpLinkTagEvent evt)
        {
            string[] splited = evt.linkText.Split(':');
            InternalEditorUtility.OpenFileAtLineExternal(splited[0], int.Parse(splited[1]));
        }
        #endregion

        #region LogEvents

        private void OnUnityConsoleLog(string logString, string stackTrace, LogType type)
        {
            ULog log = new ULog()
            {
                message = logString,
                stacktrace = stackTrace,
                logType = type,
                chanel = 1
            };

            OnNewLog(log);
        }


        public void OnClearLogs()
        {
            SelectLogLine(null);
            logLinesContainer.Clear();
            logLinesFromULog.Clear();

            Refresh();
        }


        public void OnNewLog(ULog log)
        {
            LogLine logLine = CreateLine(log);
            if (!logLinesFromULog.ContainsKey(log))
            {
                logLinesFromULog.Add(log, new List<LogLine>());
            }

            logLinesFromULog[log].Add(logLine);
            Refresh();

            if (isPauseOnError && (log.logType == LogType.Error || log.logType == LogType.Exception))
            {
                EditorApplication.isPaused = true;
            }

            //If there is no selected logline, update the details text from the last logline
            if (currentSelectedLogLine == null && logFilters.CheckLog(log))
                UpdateDetailsText(logLine);
        }


        public void OnRemoveLog(ULog log)
        {
            if (logLinesFromULog.TryGetValue(log, out List<LogLine> logLines))
            {
                foreach (LogLine logLine in logLines)
                    logLinesContainer.Remove(logLine);
                logLinesFromULog.Remove(log);
            }
            Refresh();

        }
        #endregion

    }
}