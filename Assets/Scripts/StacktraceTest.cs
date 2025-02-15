using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateConsole;
using UnityEngine;
using UnityEngine.UI;

public class StacktraceTest : MonoBehaviour
{
    public Button btnTemplate;

    private void Awake()
    {
        btnTemplate.gameObject.SetActive(false);

        CreateButton("PlayerController Log Warning", () =>
        {
            UConsole.LogWarning("Log Warning", ELogChanels.PlayerController);
        });

        CreateButton("AI Log Error", () =>
        {
            UConsole.LogError("Log Error", ELogChanels.AI, LogType.Error);
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
            UConsole.Log("Log", ELogChanels.Default, context: this);
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

}
