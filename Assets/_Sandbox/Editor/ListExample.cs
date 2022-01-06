using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Serialization;
 
 
public class ListViewBindingExample : EditorWindow
{
    [MenuItem("Window/UI Toolkit/ListViewBindingExample")]
    public static void ShowExample()
    {
        ListViewBindingExample wnd = GetWindow<ListViewBindingExample>();
        wnd.titleContent = new GUIContent("ListViewBindingExample");
    }
 
    [Serializable]
    public struct CustomStruct
    {
        public string StringValue;
        public float FloatValue;
        public CustomStruct(string strValue, float fValue)
        {
            StringValue = strValue;
            FloatValue = fValue;
        }
    }
   
    public enum DisplayArrayType
    {
        IntegerNumbers,
        CustomStructs
    }
   
    [SerializeField]
    private DisplayArrayType m_DisplayArrayType;
 
    [SerializeField] private List<int> m_Numbers;
    [SerializeField] private List<CustomStruct> m_CustomStructs;
 
   
    private ListView m_ListView;
    private SerializedObject serializedObject;
   
    private SerializedProperty m_ArraySizeProperty;
    private SerializedProperty m_ArrayProperty;
 
    private int m_ListViewInsertIndex = -1; // To make sure we insert the ListView at the right place in our visualTree
   
    public void CreateGUI()
    {
        CreateDataIfNecessary();
 
        serializedObject = new SerializedObject(this);
        VisualElement root = rootVisualElement;
 
        var rowContainer = new VisualElement();
        rowContainer.style.flexDirection = FlexDirection.Row;
        rowContainer.style.justifyContent = Justify.FlexStart;
 
        var dataSelector = new EnumField();
        dataSelector.bindingPath = nameof(m_DisplayArrayType);
       
        dataSelector.RegisterValueChangedCallback(SwitchDisplayedData);
 
        root.Add(dataSelector);
 
        AddButton(rowContainer, "Default", () => CreateListView(false, false));
        AddButton(rowContainer, "Custom MakeItem", () => CreateListView(true, false));
        AddButton(rowContainer, "Custom MakeItem+BindItem", () => CreateListView(true, true));
 
        root.Add(rowContainer);
       
        CreateListView(false, false);
       
        rowContainer = new VisualElement();
        rowContainer.style.flexDirection = FlexDirection.Row;
        rowContainer.style.justifyContent = Justify.FlexEnd;
        root.Add(rowContainer);
 
       
        AddButton(rowContainer, "-", DecreaseArraySize);
        AddButton(rowContainer, "+", IncreaseArraySize);
       
        root.Bind(serializedObject);
    }
 
    private void CreateDataIfNecessary()
    {
        if (m_Numbers == null)
        {
            m_Numbers = new List<int>() {1, 2, 3};
        }
 
        if (m_CustomStructs == null)
        {
            m_CustomStructs = new List<CustomStruct>();
            for (var i = 0; i < 3; ++i)
            {
                m_CustomStructs.Add(new CustomStruct($"Value number {i}", i + 0.5f));
            }
        }
    }
 
    private void IncreaseArraySize()
    {
        m_ArraySizeProperty.intValue++;
        serializedObject.ApplyModifiedProperties();
    }
 
    private void DecreaseArraySize()
    {
        if (m_ArraySizeProperty.intValue > 0)
        {
            m_ArraySizeProperty.intValue--;
            serializedObject.ApplyModifiedProperties();
        }
    }
 
 
    void AddButton(VisualElement container, string label, Action onClick)
    {
        container.Add(new Button(onClick) {text = label});
    }
 
    private void SwitchDisplayedData(ChangeEvent<Enum> evt)
    {
        var newValue = (DisplayArrayType) evt.newValue;
 
        if (m_DisplayArrayType != newValue)
 
        {
            // Because we're hooked before the bindings system, we'll receive value changes before it.
            // So we need to affect the value right away before recreating our list
            m_DisplayArrayType = newValue;
            CreateListView(false, false);
        }
    }
 
    void CreateListView(bool customMakeItem, bool customBindItem)
    {
        if (m_ListView != null)
        {
            //We clean ourselves up
            m_ListView.Unbind();
            m_ListView?.RemoveFromHierarchy();
        }
       
        m_ListView = new ListView();
        //m_ListView.showBoundCollectionSize = false;
 
        m_ListView.name = "List-" + m_DisplayArrayType.ToString();
       
        if (m_DisplayArrayType == DisplayArrayType.CustomStructs)
        {
            m_ListView.bindingPath = nameof(m_CustomStructs);
            m_ListView.itemHeight = 60;
        }
        else
        {
            m_ListView.bindingPath = nameof(m_Numbers);
            m_ListView.itemHeight = 20;
        }
 
        m_ArrayProperty = serializedObject.FindProperty(m_ListView.bindingPath);
        m_ArraySizeProperty = serializedObject.FindProperty(m_ListView.bindingPath + ".Array.size");
       
        m_ListView.style.flexGrow = 1;
 
        if (customMakeItem || customBindItem)
        {
            m_ListView.name = m_ListView.name + "-custom-item";
            // You can have only make item (default bindItem should work)
            if (m_DisplayArrayType == DisplayArrayType.CustomStructs)
            {
                m_ListView.makeItem = () => CreateCustomStructListItem(customBindItem);
            }
            else
            {
                m_ListView.makeItem = () => CreateNumberListItem(customBindItem);
            }
        }
 
        if (customBindItem)
        {
            m_ListView.name += "+bind";
 
            // The default bindItem will find the first IBindable type and bind the property to it
            // If you really have specific use cases you might want to set your own bindItem
         
 
            m_ListView.bindItem = ListViewBindItem;
        }
 
        if (m_ListViewInsertIndex < 0)
        {
            m_ListViewInsertIndex = rootVisualElement.childCount;
            rootVisualElement.Add(m_ListView);
               
        }
        rootVisualElement.Insert(m_ListViewInsertIndex, m_ListView);
        m_ListView.Bind(serializedObject);
    }
 
    private void AddRemoveItemButton(VisualElement row, bool enable)
    {
        var button = new Button() {text = "-"};
        button.clicked+=() =>
        {
            //var clickedElement = evt.target as VisualElement;
 
            if (button != null && button.userData is int index)
            {
                m_ArrayProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            }
        };
 
        if (enable)
        {
            button.tooltip = "Remove this item from the list";
        }
        else
        {
            button.SetEnabled(false);
            row.tooltip = "Item removing is only available with custom BindItem";
        }
        row.Add(button);
    }
   
    VisualElement CreateCustomStructListItem(bool removeButtonAvailable)
    {
        var keyFrameContainer = new BindableElement(); //BindableElement so the default bind can assign the item's root property
        var lbl = new Label("Custom Item UI");
        lbl.AddToClassList("custom-label");
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.justifyContent = Justify.SpaceBetween;
        row.Add(lbl);
 
        AddRemoveItemButton(row, removeButtonAvailable);
 
        keyFrameContainer.Add(row);
        keyFrameContainer.Add(new TextField() {bindingPath = nameof(CustomStruct.StringValue)});
        keyFrameContainer.Add(new FloatField() {bindingPath = nameof(CustomStruct.FloatValue)});
        return keyFrameContainer;
    }
   
    VisualElement CreateNumberListItem(bool removeButtonAvailable)
    {
        var row = new VisualElement(); //BindableElement so the default bind can assign the item's root property
        row.style.flexDirection = FlexDirection.Row;
        row.style.justifyContent = Justify.SpaceBetween;
 
        row.Add(new Label()); // default bind need this to be the first Bindable in the tree
        AddRemoveItemButton(row, removeButtonAvailable);
        return row;
    }
   
    void ListViewBindItem(VisualElement element, int index)
    {
        var label = element.Q<Label>(className: "custom-label");
        if (label != null)
        {
            label.text = "Custom Item UI (Custom Bound)";
        }
 
        var button = element.Q<Button>();
        if (button != null)
        {
            button.userData = index;
        }
 
        //we find the first Bindable
        var field = element as IBindable;
        if (field == null)
        {
            //we dig through children
            field = element.Query().Where(x => x is IBindable).First() as IBindable;
        }
 
        // Bound ListView.itemsSource is a IList of SerializedProperty
        var itemProp = m_ListView.itemsSource[index] as SerializedProperty;
 
        field.bindingPath = itemProp.propertyPath;
 
        element.Bind(itemProp.serializedObject);
    }
}