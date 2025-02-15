using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UltimateConsole.Editor.Settings;

namespace UltimateConsole.Editor.Window
{
    public class LogLine : VisualElement
    {
        public static event Action<LogLine> OnLogLineSelected = null;
        public static event Action<LogLine> OnLogLineDoubleClicked = null;

        private const string LOG_LINE_CLASS = "log-line";
        private const string LOG_LINE_MESSAGE_CLASS = "log-line-message";
        private const string LOG_LINE_ICON_CLASS = "log-line-icon";
        private const string LOG_LINE_COLLAPSED_COUNT_CONTAINER_CLASS = "log-line-collapsed-count-container";
        private const string LOG_LINE_COLLAPSED_COUNT_LABEL_CLASS = "log-line-collapsed-count-label";
        private const string IS_SELECTED_CLASS = "log-line-selected";
        private const float DOUBLE_CLICK_TIME_THRESHOLD = 0.3f;
        private Label messageLabel = null;
        private VisualElement icon = null;
        private VisualElement collapsedCountContainer = null;
        private Label collapsedCountLabel = null;


        private UltimateConsoleSettings _settings = null;
        private float lastClickTime = 0f;

        #region Attributes
        public new class UxmlFactory : UxmlFactory<LogLine, UxmlTraits>
        {
            public override string uxmlQualifiedName => "UltimateConsole.Editor.Window.LogLine";
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription messageAttr = new UxmlStringAttributeDescription()
            {
                name = "message",
                defaultValue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            };

            UxmlEnumAttributeDescription<LogType> LogTypeAttr = new UxmlEnumAttributeDescription<LogType>()
            {
                name = "logType",
                defaultValue = LogType.Log,
            };

            UxmlIntAttributeDescription ChanelAttr = new UxmlIntAttributeDescription()
            {
                name = "chanel",
                defaultValue = 0,
            };

            UxmlBoolAttributeDescription IsSelectedAttr = new UxmlBoolAttributeDescription()
            {
                name = "isSelected",
                defaultValue = false,
            };

            UxmlIntAttributeDescription CollapsedCountAttr = new UxmlIntAttributeDescription()
            {
                name = "collapsedCount",
                defaultValue = 0,
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)

            {
                base.Init(ve, bag, cc);
                LogLine logLine = ve as LogLine;

                logLine.Message = messageAttr.GetValueFromBag(bag, cc);
                logLine.LogType = LogTypeAttr.GetValueFromBag(bag, cc);
                logLine.Chanel = (short)ChanelAttr.GetValueFromBag(bag, cc);
                logLine.IsSelected = IsSelectedAttr.GetValueFromBag(bag, cc);
                logLine.CollapsedCount = CollapsedCountAttr.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        #region SETTERS
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                SetMessage(value);
            }
        }

        private void SetMessage(string message)
        {
            messageLabel.text = message;
            _message = message;
        }

        private LogType _logType = LogType.Log;
        public LogType LogType
        {
            get => _logType;
            set => SetLogType(value);
        }

        private void SetLogType(LogType logType)
        {
            _logType = logType;
            icon.style.unityBackgroundImageTintColor = _settings.GetColorFromLogType(logType);
        }

        private long _chanel = 0;
        public long Chanel
        {
            get => _chanel;
            set => SetChanel(value);
        }

        private void SetChanel(long chanel)
        {
            _chanel = chanel;
            icon.style.backgroundImage = _settings.GetChanelIcon(chanel);
        }

        public bool IsSelected
        {
            get => this.ClassListContains(IS_SELECTED_CLASS);
            set => this.EnableInClassList(IS_SELECTED_CLASS, value);
        }

        private ULog? _log = null;
        public ULog? Log
        {
            get => _log;
            set => SetLog(value);
        }

        private void SetLog(ULog? log)
        {
            if (log == null)
            {
                Message = string.Empty;
                LogType = LogType.Log;
                Chanel = 0;
            }
            else
            {
                Message = log.Value.message;
                LogType = log.Value.logType;
                Chanel = log.Value.chanel;
            }
            _log = log;
        }

        public void ResetSearchResult()
        {
            Message = Log.Value.message;
        }

        public void SetSearchResult(int startIndex, int endIndex)
        {
            string startmessage;
            string midmessage;
            string endmessage;

            startmessage = Log.Value.message.Substring(0, startIndex);
            midmessage = Log.Value.message.Substring(startIndex, endIndex - startIndex);
            endmessage = Log.Value.message.Substring(endIndex);

            Message = string.Format("{0}<color={1}>{2}</color>{3}", startmessage, UltimateConsoleWindowUtility.HighlightBackgroundColor, midmessage, endmessage);
        }

        private int _collapsedCount = 0;
        public int CollapsedCount
        {
            get => _collapsedCount;
            set => SetCollapsedCount(value);
        }

        private void SetCollapsedCount(int collapsedCount)
        {
            _collapsedCount = collapsedCount;

            if (collapsedCount > 1)
            {
                collapsedCountLabel.text = collapsedCount.ToString();
                collapsedCountContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                collapsedCountContainer.style.display = DisplayStyle.None;
            }
        }

        #endregion

        public LogLine()
        {
            _settings = UltimateConsoleSettings.GetOrCreateSettings();

            this.AddToClassList(LOG_LINE_CLASS);

            messageLabel = new Label();
            messageLabel.AddToClassList(LOG_LINE_MESSAGE_CLASS);
            messageLabel.name = "message";
            messageLabel.text = "message";
            messageLabel.enableRichText = true;


            icon = new VisualElement();
            icon.AddToClassList(LOG_LINE_ICON_CLASS);
            icon.name = "icon";


            collapsedCountContainer = new VisualElement();
            collapsedCountContainer.AddToClassList(LOG_LINE_COLLAPSED_COUNT_CONTAINER_CLASS);
            collapsedCountContainer.style.display = DisplayStyle.None;
            collapsedCountContainer.name = "collapsed-count-container";


            collapsedCountLabel = new Label();
            collapsedCountLabel.AddToClassList(LOG_LINE_COLLAPSED_COUNT_LABEL_CLASS);
            collapsedCountLabel.text = "2";
            collapsedCountLabel.name = "collapsed-count-label";


            hierarchy.Add(icon);
            hierarchy.Add(messageLabel);
            hierarchy.Add(collapsedCountContainer);
            collapsedCountContainer.Add(collapsedCountLabel);

            RegisterCallback<ClickEvent>(OnClick);
        }

        private void OnClick(ClickEvent evt)
        {
            if (Time.time - lastClickTime < DOUBLE_CLICK_TIME_THRESHOLD)
                OnLogLineDoubleClicked?.Invoke(this);
            IsSelected = true;
            OnLogLineSelected?.Invoke(this);
            lastClickTime = Time.time;
        }
    }


}