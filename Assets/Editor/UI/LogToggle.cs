using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateConsole.Editor.Window
{
    public class LogToggle : ToolbarToggle
    {
        private const string COUNT_LABEL_CLASS = "log-types-toggle-count-label";
        private const string ICON_CLASS = "log-types-toggle-icon";
        private const string TOGGLE_CLASS = "log-types-toggle";

        private VisualElement icon;

        public new class UxmlFactory : UxmlFactory<LogToggle, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public UxmlIntAttributeDescription countAttr = new UxmlIntAttributeDescription { name = "count" };
            public UxmlEnumAttributeDescription<LogType> logTypeAttr = new UxmlEnumAttributeDescription<LogType> { name = "log-type" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                LogToggle logToggle = ve as LogToggle;
                logToggle.LogType = logTypeAttr.GetValueFromBag(bag, cc);
                logToggle.Count = countAttr.GetValueFromBag(bag, cc);
            }
        }

        public LogToggle()
        {
            AddToClassList(TOGGLE_CLASS);
            labelElement.AddToClassList(COUNT_LABEL_CLASS);
            icon = this.Q<VisualElement>(className: "unity-toggle__input");
            icon.AddToClassList(ICON_CLASS);

            Count = 0;
        }

        public LogToggle(LogType logType) : this()
        {
            LogType = logType;
            name = logType.ToString();
        }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                label = _count > 999 ? "999+" : _count.ToString();
                tooltip = _count.ToString();
            }
        }

        private LogType _logType;
        public LogType LogType
        {
            get => _logType;
            set
            {
                _logType = value;
                icon.style.backgroundImage = GetIcon(_logType);
            }
        }

        private Texture2D GetIcon(LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                    return EditorGUIUtility.Load("d_console.infoicon.sml") as Texture2D;
                case LogType.Warning:
                    return EditorGUIUtility.Load("d_console.warnicon.sml") as Texture2D;
                case LogType.Error:
                    return EditorGUIUtility.Load("d_console.erroricon.sml") as Texture2D;
                default:
                    return null;
            }
        }

    }
}
