using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrismLog.Editor
{
    internal static class PrismLogSettingsProviderGUIContents
    {
        private const string TOOLTIP_EXCEPTION_DISPLAY_MODE = "- CheckChanel: The exception will be displayed only if the default chanel is enabled.\n\n- CheckLogType: The exception will be displayed only if the error log type is enabled.";

        private static GUIContent _exceptionDisplayModeContent;
        public static GUIContent ExceptionDisplayModeContent
        {
            get
            {
                return new GUIContent(
                    "Exception Display Mode",
                    TOOLTIP_EXCEPTION_DISPLAY_MODE
                );
            }
        }
    }
}
