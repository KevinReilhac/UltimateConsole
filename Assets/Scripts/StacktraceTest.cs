using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using PrismLog;
using UnityEngine;
using UnityEngine.UI;

public class StacktraceTest : MonoBehaviour
{
    public Button btnTemplate;

    private void Awake()
    {
        btnTemplate.gameObject.SetActive(false);

        CreateButton("UI Log Warning", () =>
        {
            PConsole.LogWarning("Log Warning", ELogChanels.UI);
        });

        CreateButton("AI Log Error", () =>
        {
            PConsole.LogError("Log Error", ELogChanels.AI, LogType.Error);
        });

        CreateButton("Unity default Log", () =>
        {
            Debug.Log("Unity log message");
        });

        CreateButton("Unity default error log", () =>
        {
            Debug.LogError("Unity error log message");
        });

        CreateButton("Context Log", () =>
        {
            PConsole.Log("Log", ELogChanels.Default, context: this);
        });

        CreateButton("Exception Log", () =>
        {
            throw new Exception("Exception log message");
        });

        CreateButton("Assert Log", () =>
        {
            Debug.Assert(false, "Assert log message");
        });

    }

    private void CreateButton(string text, Action onClick)
    {
        Button btn = Instantiate(btnTemplate, btnTemplate.transform.parent);
        btn.gameObject.SetActive(true);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btn.onClick.AddListener(() => onClick());
    }

    private void OnGUI()
    {
        GUILayout.Label("SAUCISSES");
    }

}
