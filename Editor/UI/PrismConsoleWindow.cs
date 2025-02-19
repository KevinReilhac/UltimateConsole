using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using PrismLog.Editor.Settings;
using System.Diagnostics;
using System.Text;

namespace PrismLog.Editor.Window
{
    public class PrismConsoleWindow : EditorWindow, IPLogHandler
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset = default;
        [SerializeField] private VisualTreeAsset debugLineTemplate;

        private VisualElement logLinesContainer = null;
        private LogLine currentSelectedLogLine = null;
        private Label stackTraceText = null;
        private PrismLogSettings settings = null;
        private Label detailsText;
        private LogFilters logFilters = new LogFilters();

        private Dictionary<PLog, List<LogLine>> logLinesFromULog = new Dictionary<PLog, List<LogLine>>();
        private LogTypesToggles logTypesToggles;
        private bool isCollapsed = false;
        private bool isPauseOnError = false;

        private const string PARENTHESIS_FORMAT_PATTERN = @"Assets[/\\][^()]+\.cs\((\d+),(\d+)\)";

        [MenuItem("Window/UI Toolkit/UltimateConsoleWindow")]
        public static void ShowExample()
        {
            PrismConsoleWindow wnd = GetWindow<PrismConsoleWindow>();
            wnd.titleContent = new GUIContent("Ultimate Console");
        }

        private void OnEnable()
        {
            settings = PrismLogSettings.GetOrCreateSettings();

            logFilters.OnLogFilterChanged += Refresh;
            LogLine.OnLogLineSelected += SelectLogLine;
            LogLine.OnLogLineDoubleClicked += OnLogLineDoubleClicked;
            Application.logMessageReceived += OnUnityConsoleLog;
            PConsole.RegisterLogHandler(this);
        }

        private void OnDisable()
        {
            logFilters.OnLogFilterChanged -= Refresh;
            LogLine.OnLogLineSelected -= SelectLogLine;
            LogLine.OnLogLineDoubleClicked -= OnLogLineDoubleClicked;
            Application.logMessageReceived -= OnUnityConsoleLog;
            PConsole.UnRegisterLogHandler(this);
        }

        private void OnBecameVisible()
        {
            Refresh();
        }

        private void Refresh()
        {
            LogFilters.LogFiltersResult checkLog = null;
            logTypesToggles?.ResetCounts();

            /// Hide loglines that are not in the filters
            foreach (KeyValuePair<PLog, List<LogLine>> logLines in logLinesFromULog)
            {
                checkLog = logFilters.CheckLog(logLines.Key);
                for (int logLineIndex = 0; logLineIndex < logLines.Value.Count; logLineIndex++)
                {
                    logTypesToggles?.IncrementLogCount(logLines.Key.logType);

                    if (logLines.Value[logLineIndex] == null)
                        continue;

                    if (checkLog.isDisplayable && checkLog.isSearchStringSuccess)
                        logLines.Value[logLineIndex].SetSearchResult(checkLog.searchStringStartIndex, checkLog.searchStringEndIndex);
                    else
                        logLines.Value[logLineIndex].ResetSearchResult();

                    if (isCollapsed)
                    {
                        if (logLineIndex == 0)
                        {
                            logLines.Value[logLineIndex].style.display = checkLog ? DisplayStyle.Flex : DisplayStyle.None;
                            logLines.Value[logLineIndex].CollapsedCount = logLines.Value.Count;
                        }
                        else
                            logLines.Value[logLineIndex].style.display = DisplayStyle.None;
                    }
                    else
                    {
                        logLines.Value[logLineIndex].style.display = checkLog ? DisplayStyle.Flex : DisplayStyle.None;
                        logLines.Value[logLineIndex].CollapsedCount = 0;
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
            if (logLine.Log != null)
            {
                string fileName = logLine.Log.Value.stacktrace[0].GetFileName();
                int lineNumber = logLine.Log.Value.stacktrace[0].GetFileLineNumber();
                int columnNumber = logLine.Log.Value.stacktrace[0].GetFileColumnNumber();
                if (fileName != null)
                {
                    InternalEditorUtility.OpenFileAtLineExternal(fileName, lineNumber, columnNumber);
                }
            }
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
                if (detailsText != null)
                    detailsText.text = string.Empty;
                SetStackTraceText(null);
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
            chanelsDropDown.choices = settings.chanelSettings.Select(s => s.Name).ToList();
            chanelsDropDown.RegisterValueChangedCallback(OnChanelsChange);
            chanelsDropDown.value = PrismConsoleWindowPrefs.Chanels;
            detailsText = root.Q<Label>("DetailsText");
            detailsText.text = string.Empty;

            logTypesToggles = root.Q<LogTypesToggles>();
            logTypesToggles.SetLogTypes(PrismConsoleWindowPrefs.GetEnabledLogTypes(), false);
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
            collapseButton.value = PrismConsoleWindowPrefs.CollapseEnabled;

            ToolbarToggle pauseOnErrorToggle = root.Q<ToolbarToggle>("ErrorPause");
            pauseOnErrorToggle.RegisterCallback<ChangeEvent<bool>>(OnPauseOnErrorToggleChange);
            pauseOnErrorToggle.value = PrismConsoleWindowPrefs.ErrorPauseEnabled;
            SetupDropdownFields(chanelsDropDown);
            RegisterStackTraceTextLinks(stackTraceText);

            logFilters.UpdateLogTypes(PrismConsoleWindowPrefs.GetEnabledLogTypes());

            Refresh();
        }

        private void SetLogTypes(List<LogType> list)
        {
            logFilters.UpdateLogTypes(list);
            PrismConsoleWindowPrefs.SetEnabledLogTypes(list);
            Refresh();
        }

        #region UI_CALLBACKS

        private void OnChanelsChange(ChangeEvent<int> evt)
        {
            logFilters.Chanels = evt.newValue;
            PrismConsoleWindowPrefs.Chanels = evt.newValue;
        }

        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            logFilters.SearchText = evt.newValue;
        }

        private void OnCollapseToggleChange(ChangeEvent<bool> evt)
        {
            isCollapsed = evt.newValue;
            PrismConsoleWindowPrefs.CollapseEnabled = isCollapsed;
            Refresh();
        }

        private void OnPauseOnErrorToggleChange(ChangeEvent<bool> evt)
        {
            isPauseOnError = evt.newValue;
            PrismConsoleWindowPrefs.ErrorPauseEnabled = isPauseOnError;
        }

        #endregion

        private void SetupDropdownFields(MaskField chanelsDropdown)
        {
            List<string> enumNames = new List<string>() { "Default" };
            enumNames.AddRange(settings.chanelSettings.Select(s => s.Name));

            chanelsDropdown.choices = enumNames;
        }

        private LogLine CreateLine(PLog log)
        {
            if (logLinesContainer == null)
                return null;

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

        private void SetStackTraceText(List<StackFrame> stackTrace)
        {
            if (stackTraceText == null)
                return;

            if (stackTrace == null)
            {
                stackTraceText.text = string.Empty;
                return;
            }

            StringBuilder stackTraceBuilder = new StringBuilder();

            for (int i = 1; i < stackTrace.Count; i++)
            {
                stackTraceBuilder.AppendLine(GetFrameText(stackTrace[i], i));
                stackTraceBuilder.AppendLine();
            }

            stackTraceText.text = stackTraceBuilder.ToString();
        }

        private string GetFrameText(StackFrame frame, int stacktraceIndex)
        {
            string linkText = string.Format("{0}|{1}|{2}", frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber());
            string fileName = frame.GetFileName();

            StringBuilder frameText = new StringBuilder();
            frameText.Append(frame.GetMethod().DeclaringType.FullName);
            frameText.Append(".");
            frameText.Append(frame.GetMethod().Name);
            if (!string.IsNullOrEmpty(fileName))
            {
                frameText.Append(" at: ");
                frameText.AppendFormat("<link=\"{0}\"><color=#40a0ff><u>", linkText);
                frameText.Append(fileName);
                frameText.Append("(");
                frameText.Append(frame.GetFileLineNumber());
                frameText.Append(",");
                frameText.Append(frame.GetFileColumnNumber());
                frameText.Append(")");
            }
            frameText.Append("</color></u></link>");
            return frameText.ToString();
        }

        private void StackTraceHyperLinkUp(PointerUpLinkTagEvent evt)
        {
            string[] splited = evt.linkID.Split('|');
            string fileName = splited[0];
            int lineNumber = 0;
            int columnNumber = 0;

            if (splited.Length > 1)
            {
                if (int.TryParse(splited[1], out int lineParseResult))
                    lineNumber = lineParseResult;
            }
            if (splited.Length == 2)
            {
                if (int.TryParse(splited[2], out int columnParseResult))
                    columnNumber = columnParseResult;
            }

            InternalEditorUtility.OpenFileAtLineExternal(fileName, lineNumber, columnNumber);
        }
        #endregion

        #region LogEvents

        private void OnUnityConsoleLog(string logString, string stackTrace, LogType type)
        {
            //Ignore default console log if it is from UConsole
            if (logString.StartsWith(PConsole.DEFAULT_CONSOLE_LOG_START))
                return;

            PLog log = new PLog(logString, 1, type, null);

            OnNewLog(log);
        }


        public void OnClearLogs()
        {
            SelectLogLine(null);
            logLinesContainer.Clear();
            logLinesFromULog.Clear();
            SetStackTraceText(null);
            detailsText.text = string.Empty;

            Refresh();
        }


        public void OnNewLog(PLog log)
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


        public void OnRemoveLog(PLog log)
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