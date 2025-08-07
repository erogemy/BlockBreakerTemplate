using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockBreakerBuilderWindow : EditorWindow
{
    [SerializeField] VisualTreeAsset m_VisualTreeAsset;

    [MenuItem("Tools/Erogemy/BlockBreakerBuilderWindow")]
    public static void ShowExample()
    {
        var wnd = GetWindow<BlockBreakerBuilderWindow>();
        wnd.titleContent = new GUIContent("BlockBreakerBuilderWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
