using System;
using System.Collections.Generic;
using ArteHacker.UITKEditorAid;
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
    [Q("list")]
    private ListView list;
    [Q("val-button")]
    private Button valButton;
    [Q("serObj-button")]
    private Button serObjButton;
    [Q("bind-button")]
    private Button bindButton;
    [Q("rbs")]
    private QuickEye.RequestWatcher.RequestButtonSmall rbs;
    [Q("code")]
    private QuickEye.RequestWatcher.CodeField code;


    [SerializeField]
    private HDRequest sampleReq;

    [SerializeField]
    private List<HDRequest> reqList = new List<HDRequest>();
    
    private SerializedObject serObj;
    private SerializedProperty serProp;

    public void CreateGUI()
    {
        var root = InitUxml();
        serObj = new SerializedObject(this);

        rbs.SetBindingPaths("sampleReq.type","sampleReq.name","sampleReq.lastResponse.statusCode");

        code.Field.bindingPath = "sampleReq.body";
        SetupList();
        //SetupPlayList();
        
        SetupButtons();
        root.Bind(serObj);
        Debug.Log($"Create UI Finished");
    }
    

    private void SetupList()
    {
        var listProp = serObj.FindProperty("reqList");
        list.makeItem = () => new RequestButtonSmall();
        list.bindItem = (ve, index) =>
        {
            Debug.Log($"Bind Item: {index}/{listProp.arraySize}");
            var reqProp = listProp.GetArrayElementAtIndex(index);
            var typeProp = reqProp.FindPropertyRelative(nameof(HDRequest.type));
            var nameProp = reqProp.FindPropertyRelative(nameof(HDRequest.name));
            var codeProp = reqProp.FindPropertyRelative(nameof(HDRequest.lastResponse))
                .FindPropertyRelative(nameof(HDResponse.statusCode));
            var button = ve.As<RequestButtonSmall>();
            button.SetBindingPaths(typeProp.propertyPath, 
                nameProp.propertyPath,
                codeProp.propertyPath);
            button.Unbind();
            button.Bind(listProp.serializedObject);
        };
        list.itemsSource = reqList;
    }

    private void SetupButtons()
    {
        //valButton.Text("Update value").Clicked(() => { sampleReq.Add("143"); });
        serObjButton.Text("Update serO").Clicked(() => { serObj.Update(); });
        bindButton.Text("Bind").Clicked(() =>
        {
            rootVisualElement.Bind(serObj);
            //serProp.
        });
    }

    private VisualElement InitUxml()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        var tree = Resources.Load<VisualTreeAsset>("BindingTest");
        tree.CloneTree(root);
        root.AssignQueryResults(this);
        return root;
    }
}