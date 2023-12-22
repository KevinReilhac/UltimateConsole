using System;
using System.Collections;
using System.Collections.Generic;
using UltimateConsole;
using UnityEngine;
using UnityEngine.UI;

public class StacktraceTest : MonoBehaviour
{
    public Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            UConsole.Log("My FIRST MESSAGE", LogChanels.PlayerController, LogType.Error);
        });
    }
}
