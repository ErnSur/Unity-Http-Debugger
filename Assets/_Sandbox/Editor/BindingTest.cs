using System.Collections.Generic;
using QuickEye.RequestWatcher;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BindingTest : EditorWindow
{
    [MenuItem("Window/UIElements/BindingTest")]
    public static void ShowExample()
    {
        BindingTest wnd = GetWindow<BindingTest>();
        wnd.titleContent = new GUIContent("BindingTest");
    }

    [SerializeField]
    private List<string> sampleReq = new List<string>()
    {
        "sds", "sdsef"
    };

    [SerializeField]
    private VisualTreeAsset tree;

    [Q("list")]
    private ListView list;

    [Q("val-button")]
    private Button valButton;

    [Q("serObj-button")]
    private Button serObjButton;

    [Q("bind-button")]
    private Button bindButton;


    private SerializedObject serObj;
    private SerializedProperty serProp;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        //tree = Resources.Load<VisualTreeAsset>("BindingTest");
        tree.CloneTree(root);
        root.AssignQueryResults(this);
        serObj = new SerializedObject(this);
        serProp = serObj.FindProperty("sampleReq");
        list.BindProperty(serProp);
        list.bindItem += (element, i) =>
        {
            if (i == 0)
            {
                return;
            }
            i--;
                element.RemoveFromHierarchy();
            Debug.Log($"index {i}");
        };
        //serProp = serObj.FindProperty("sampleReq.lastResponse.");

        //label.BindProperty(serProp);

        valButton.Text("Update value").Clicked(() => { sampleReq.Add("143"); });
        serObjButton.Text("Update serO").Clicked(() => { serObj.Update(); });
        bindButton.Text("Bind").Clicked(() =>
        {
            rootVisualElement.Bind(serObj);
            //serProp.
        });
    }
}