using System;
using System.Collections.Generic;
using System.Linq;
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
        private Label stackTraceText = null;
        private UltimateConsoleSettings settings = null;

        [MenuItem("Window/UI Toolkit/UltimateConsoleWindow")]
        public static void ShowExample()
        {
            UltimateConsoleWindow wnd = GetWindow<UltimateConsoleWindow>();
            wnd.titleContent = new GUIContent("Ultimate Console");
        }

        private void OnEnable()
        {
            settings = UltimateConsoleSettings.GetOrCreateSettings();
            UConsole.RegisterLogHandler(this);
        }

        private void OnDisable()
        {
            UConsole.UnRegisterLogHandler(this);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualTreeAsset.CloneTree(root);

            Label stacktraceText = root.Q<Label>("StackTraceText");
            logLinesContainer = root.Q<ScrollView>("LogLineContainer").contentContainer;
            logLinesContainer.Clear();

            MaskField chanelsDropDown = root.Q<MaskField>("ChanelsDropdown");
            SetupDropdownFields(chanelsDropDown);
            RegisterStackTraceTextLinks(stacktraceText);
        }

        private void SetupDropdownFields(MaskField chanelsDropdown)
        {
            List<string> enumNames = new List<string>() { "Default" };
            enumNames.AddRange(settings.chanelSettings.Select(s => s.Name));

            chanelsDropdown.choices = enumNames;
        }


        private void RegisterStackTraceTextLinks(Label label)
        {
            label.RegisterCallback<PointerUpLinkTagEvent>(StackTraceHyperLinkUp);
        }

        private void StackTraceHyperLinkUp(PointerUpLinkTagEvent evt)
        {
            string[] splited = evt.linkText.Split(':');
            InternalEditorUtility.OpenFileAtLineExternal(splited[0], int.Parse(splited[1]));
        }

        private void CreateLine(ULog log)
        {
            VisualElement visualElement = debugLineTemplate.Instantiate();

            visualElement.userData = log;
            Label text = visualElement.Q<Label>("Text");
            VisualElement icon = visualElement.Q("Icon");

            text.text = log.message;
            text.style.color = settings.GetColorFromLogType(log.logType);

            icon.style.backgroundImage = settings.GetChanelIcon(log.chanel);
            logLinesContainer.Add(visualElement);
        }

        #region LogEvents
        public void OnClearLogs()
        {
            logLinesContainer.Clear();
        }

        public void OnNewLog(ULog log)
        {
            CreateLine(log);
        }

        public void OnRemoveLog(ULog log)
        {
        }
        #endregion
    }
}