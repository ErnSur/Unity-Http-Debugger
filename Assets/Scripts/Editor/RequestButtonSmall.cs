using System;
using System.Collections.Generic;
using System.ComponentModel;
using ArteHacker.UITKEditorAid;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class RequestButtonSmall : RequestButton
    {
        [Q("rbs--code")]
        private Label rbsCode;
        [Q("rbs-type")]
        private Label rbsType;
        [Q("rbs-name")]
        private Label rbsName;

        private ValueTracker<Enum> typeTracker;
        public RequestButtonSmall()
        {
            this.InitFromUxml();
            // Add(typeTracker = new ValueTracker<Enum>("", evt =>
            // {
            //     try
            //     {
            //         var v = evt.newValue;
            //         
            //         Debug.Log($"New v: {v}");
            //         //var enumName = p.enumNames[p.enumValueIndex];
            //         //rbsType.text = FormatHttpMethodType(enumName);
            //     }
            //     catch (Exception e)
            //     {
            //         //rbsType.text = "";
            //     }
            // }));
        }
        //TODO: Dont worry about binding too much because we need to implement search and that will fuck things over
        public void SetBindingPaths(string typeProp, string nameProp, string statusCode)
        {
            rbsName.bindingPath= (nameProp);
            rbsCode.bindingPath= (statusCode);
            //typeTracker.bindingPath = typeProp;
            this.TrackPropertyChange<string>(typeProp, v =>
            {
                try
                {
                    //var enumName = p.enumNames[p.enumValueIndex];
                    rbsType.text = FormatHttpMethodType(v.ToString());
                }
                catch (Exception e)
                {
                    rbsType.text = "";
                }
            });
        }

        public void BindProperties(SerializedProperty typeProp, SerializedProperty nameProp,SerializedProperty statusCode)
        {
            rbsName.BindProperty(nameProp);
            rbsCode.BindProperty(statusCode);
            this.TrackPropertyChange(typeProp, p =>
            {
                try
                {
                    if(p.propertyType != SerializedPropertyType.Enum || p.enumValueIndex == -1)
                        return;
                    
                    var enumName = p.enumNames[p.enumValueIndex];
                    rbsType.text = FormatHttpMethodType(enumName);
                }
                catch (Exception e)
                {
                    rbsType.text = "";
                }
            });
        }

        public new class UxmlFactory : UxmlFactory<RequestButtonSmall, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription text = new UxmlStringAttributeDescription()
                { name = "text", defaultValue = "New Request" };

            private UxmlEnumAttributeDescription<HttpMethodType> type =
                new UxmlEnumAttributeDescription<HttpMethodType>() { name = "type" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.As<RequestButtonSmall>().rbsName.text = text.GetValueFromBag(bag, cc);
                ve.As<RequestButtonSmall>().rbsType.text = type.GetValueFromBag(bag, cc).ToString();
            }
        }
    }
}