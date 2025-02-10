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

        private Dictionary<ULog, LogLine> logLinesFromULog = new Dictionary<ULog, LogLine>();

        [MenuItem("Window/UI Toolkit/UltimateConsoleWindow")]
        public static void ShowExample()
        {
            UltimateConsoleWindow wnd = GetWindow<UltimateConsoleWindow>();
            wnd.titleContent = new GUIContent("Ultimate Console");
        }

        private void OnEnable()
        {
            logFilters.OnLogFilterChanged += OnLogFilterChanged;
            LogLine.OnLogLineSelected += OnLogLineSelected;
            settings = UltimateConsoleSettings.GetOrCreateSettings();
            UConsole.RegisterLogHandler(this);
        }

        private void OnDisable()
        {
            logFilters.OnLogFilterChanged -= OnLogFilterChanged;
            LogLine.OnLogLineSelected -= OnLogLineSelected;
            UConsole.UnRegisterLogHandler(this);
        }

        private void OnLogFilterChanged()
        {
            bool checkLog;
            /// Hide loglines that are not in the filters
            foreach (KeyValuePair<ULog, LogLine> logLine in logLinesFromULog)
            {
                checkLog = logFilters.CheckLog(logLine.Key);
                logLine.Value.style.display = checkLog ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }


        private void OnLogLineSelected(LogLine logLine)
        {
            if (currentSelectedLogLine != null)
                currentSelectedLogLine.IsSelected = false;
            currentSelectedLogLine = logLine;

            detailsText.text = logLine.Log.Value.message;
            SetStackTraceText(logLine.Log.Value.stacktrace);
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
            detailsText = root.Q<Label>("DetailsText");
            detailsText.text = string.Empty;

            // Register search field
            ToolbarSearchField toolbarSearchField = root.Q<ToolbarSearchField>();
            toolbarSearchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldChange);

            // Register log toggles
            ToolbarToggle logTypeToggle = root.Q<ToolbarToggle>("LogToggle");
            ToolbarToggle warningToggle = root.Q<ToolbarToggle>("WarningToggle");
            ToolbarToggle errorToggle = root.Q<ToolbarToggle>("ErrorToggle");

            logTypeToggle.RegisterCallback<ChangeEvent<bool>>(OnLogToggleChange);
            warningToggle.RegisterCallback<ChangeEvent<bool>>(OnLogWarningToggleChange);
            errorToggle.RegisterCallback<ChangeEvent<bool>>(OnLogErrorToggleChange);

            // Register clear button
            ToolbarButton clearButton = root.Q<ToolbarButton>("ClearButton");
            clearButton.clickable.clicked += OnClearLogs;

            // Register collapse button
            ToolbarToggle collapseButton = root.Q<ToolbarToggle>("CollapseToggle");
            collapseButton.RegisterCallback<ChangeEvent<bool>>(OnCollapseToggleChange);


            SetupDropdownFields(chanelsDropDown);

            RegisterStackTraceTextLinks(stackTraceText);
        }


        #region UI_CALLBACKS

        private void OnLogErrorToggleChange(ChangeEvent<bool> evt)
        {
            logFilters.UpdateLogTypes(LogType.Error, evt.newValue);
        }

        private void OnLogWarningToggleChange(ChangeEvent<bool> evt)
        {
            logFilters.UpdateLogTypes(LogType.Warning, evt.newValue);
        }

        private void OnLogToggleChange(ChangeEvent<bool> evt)
        {
            logFilters.UpdateLogTypes(LogType.Log, evt.newValue);
        }

        private void OnChanelsChange(ChangeEvent<int> evt)
        {
            logFilters.Chanels = (short)evt.newValue;
        }

        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            logFilters.SearchText = evt.newValue;
        }

        private void OnCollapseToggleChange(ChangeEvent<bool> evt)
        {
            Debug.LogWarning("Collapse not implemented yet");
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
        public void OnClearLogs()
        {
            logLinesContainer.Clear();
            logLinesFromULog.Clear();
        }


        public void OnNewLog(ULog log)
        {
            LogLine logLine = CreateLine(log);
            logLinesFromULog.Add(log, logLine);

            logLine.style.display = logFilters.CheckLog(log).isDisplayable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void OnRemoveLog(ULog log)
        {
            if (logLinesFromULog.TryGetValue(log, out LogLine logLine))
            {
                logLinesContainer.Remove(logLine);
                logLinesFromULog.Remove(log);
            }
        }
        #endregion

    }
}