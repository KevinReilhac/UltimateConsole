using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StacktraceTest : MonoBehaviour
{
    public Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            Debug.Log(StackTraceUtility.ExtractStackTrace());
        });
    }
}
