using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class TabGroup : Toolbar
    {
        
        
        public string[] tabNames;
        public new class UxmlFactory : UxmlFactory<CodeField, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            //Uxml value = new  { name = "value" };

            UxmlBoolAttributeDescription isDelayed = new UxmlBoolAttributeDescription
                { name = "delayed", defaultValue = true };

            UxmlBoolAttributeDescription isReadOnly = new UxmlBoolAttributeDescription { name = "readonly" };
            UxmlBoolAttributeDescription multiline = new UxmlBoolAttributeDescription { name = "multiline" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var label = ve as CodeField;
                //label.SetValueWithoutNotify(value.GetValueFromBag(bag, cc));
                label.isDelayed = isDelayed.GetValueFromBag(bag, cc);
                label.multiline = multiline.GetValueFromBag(bag, cc);
                label.isReadOnly = isReadOnly.GetValueFromBag(bag, cc);
            }
        }
    }
}