using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public class EditableLabel : TextField
    {
        public EditableLabel()
        {
            label = null;
            isDelayed = true;
            EndEdit();
            RegisterCallback<NavigationSubmitEvent>(e =>
            {
                EndEdit();
            });
            
            RegisterCallback<FocusOutEvent>(e =>
            {
                EndEdit();
            });
        }

        private void BeginEdit()
        {
            ToggleTextFieldStyle(true);
            SelectAll();
        }

        private void EndEdit()
        {
            ToggleTextFieldStyle(false);
            SelectNone();
        }

        private void ToggleTextFieldStyle(bool enable)
        {
            EnableInClassList("unity-base-text-field",enable);
            EnableInClassList("unity-text-field",enable);
            textInputBase.EnableInClassList("unity-base-text-field__input",enable);
            doubleClickSelectsWord = enable;
            tripleClickSelectsLine = enable;
            isReadOnly = !enable;
        }
        
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            if (evt is MouseDownEvent mouseDown && mouseDown.clickCount >= 2)
            {
                BeginEdit();
            }
        }
        
        public new class UxmlFactory : UxmlFactory<EditableLabel,UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var label = ve as EditableLabel;
                label.SetValueWithoutNotify(_value.GetValueFromBag(bag, cc));
            }
        }
    }
}