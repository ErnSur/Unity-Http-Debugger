using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class BaseBindableTraits<TValueType, TValueUxmlAttributeType> : VisualElement.UxmlTraits
        where TValueUxmlAttributeType : TypedUxmlAttributeDescription<TValueType>, new()
    {
        private TValueUxmlAttributeType m_Value;
    
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((INotifyValueChanged<TValueType>) ve).SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
        }
    
        public BaseBindableTraits()
        {
            TValueUxmlAttributeType uxmlAttributeType = new TValueUxmlAttributeType();
            uxmlAttributeType.name = "value";
            this.m_Value = uxmlAttributeType;
        }
    }
}