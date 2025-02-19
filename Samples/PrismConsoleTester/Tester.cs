using System;
using System.Collections.Generic;
using System.Linq;
using PrismLog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrismLog.Samples
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private GameObject logPanel;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMP_Dropdown chanelDropdown;
        [SerializeField] private TMP_Dropdown logTypeDropdown;
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private TextMeshProUGUI errorText;

        void Start()
        {
            logPanel.SetActive(true);
            errorPanel.SetActive(false);
            SetupDropdown();
            sendButton.onClick.AddListener(SendLog);
        }

        private void SendLog()
        {
            LogType logTypeValue = (LogType)logTypeDropdown.value;
            string message = messageInputField.text;

            int chanelIndex = chanelDropdown.value;
            PrismLogChanel chanelValue = (PrismLogChanel)(1 << chanelIndex);

            PConsole.Log(message, chanelValue, logTypeValue);
        }

        private void SetupDropdown()
        {
            chanelDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var chanel in Enum.GetNames(typeof(PrismLogChanel)))
                options.Add(new TMP_Dropdown.OptionData(chanel));
            chanelDropdown.AddOptions(options);

            logTypeDropdown.ClearOptions();
            logTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new TMP_Dropdown.OptionData("<color=red>Error</color>"),
            new TMP_Dropdown.OptionData("<color=red>Assert</color>"),
            new TMP_Dropdown.OptionData("<color=yellow>Warning</color>"),
            new TMP_Dropdown.OptionData("Log"),
            new TMP_Dropdown.OptionData("<color=red>Exception</color>")
        });
        }

        private string[] GetChanelNames()
        {
            return Enum.GetValues(typeof(PrismLogChanel))
                .Cast<PrismLogChanel>()
                .Select(chanel => chanel.ToString())
                .ToArray();
        }
    }
}