using System;
using System.Collections;
using System.Collections.Generic;
using UltimateConsole;
using UnityEngine;
using UnityEngine.UI;

public class StacktraceTest : MonoBehaviour
{
    public Button btn;
    public Button btn2;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            UConsole.LogWarning("My FIRST MESSAGE", LogChanels.PlayerController);
        });

        btn2.onClick.AddListener(() =>
        {
            UConsole.LogError("Second Message", LogChanels.AI, LogType.Error);
        });
    }
}
