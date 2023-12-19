using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class UltimateConsoleWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/UltimateConsoleWindow")]
    public static void ShowExample()
    {
        UltimateConsoleWindow wnd = GetWindow<UltimateConsoleWindow>();
        wnd.titleContent = new GUIContent("UltimateConsoleWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
    

        // Instantiate UXML
        m_VisualTreeAsset.CloneTree(root);

        Label stacktraceText = root.Q<Label>(name = "StackTraceText");
        RegisterStackTraceTextLinks(stacktraceText);
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
}
