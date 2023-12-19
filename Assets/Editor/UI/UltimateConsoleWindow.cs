using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    }
}
