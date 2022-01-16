using System;
using System.Collections.Generic;
using System.Linq;
using QuickEye.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class Tab : BaseBindable<bool>
    {
        public VisualElement TabContent { get; set; }
        private Clickable clickable;

        public event Action clicked
        {
            add => clickable.clicked += value;
            remove => clickable.clicked -= value;
        }

        public string text
        {
            get => label.text;
            set => label.text = value;
        }

        [Q]
        protected Label label;

        public Tab()
        {
            this.InitResources();
            AddToClassList("tab");
            clickable = new Clickable(() =>
            {
                value = true;
            });
            this.AddManipulator(clickable);
        }

        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            SetActive(newValue);
        }

        private void SetActive(bool isActive)
        {
            EnableInClassList("tab--active", isActive);
            TabContent?.ToggleDisplayStyle(isActive);
            if (isActive)
                DeactivateSiblings();
        }

        private void DeactivateSiblings()
        {
            foreach (var tab in parent.Children().OfType<Tab>())
            {
                if (tab != this)
                    tab.SetValueWithoutNotify(false);
            }
        }

        public class UxmlFactory : UxmlFactory<Tab, UxmlTraits>
        {
        }

        public class UxmlTraits : BaseBindableTraits<bool, UxmlBoolAttributeDescription>
        {
            private UxmlStringAttributeDescription text = new UxmlStringAttributeDescription()
                { name = "text", defaultValue = "Tab" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.As<Tab>().label.text = text.GetValueFromBag(bag, cc);
            }
        }
    }
}