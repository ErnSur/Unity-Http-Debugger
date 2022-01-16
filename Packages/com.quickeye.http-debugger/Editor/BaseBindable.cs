using System.Collections.Generic;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public abstract class BaseBindable<TValueType> : BindableElement, INotifyValueChanged<TValueType>
    {
        private TValueType m_Value;
        
        protected TValueType rawValue
        {
            get => m_Value;
            set => m_Value = value;
        }
    
        public virtual TValueType value
        {
            get => m_Value;
            set
            {
                if (EqualityComparer<TValueType>.Default.Equals(m_Value, value))
                    return;
                if (panel != null)
                {
                    using (ChangeEvent<TValueType> pooled = ChangeEvent<TValueType>.GetPooled(m_Value, value))
                    {
                        pooled.target = (IEventHandler)this;
                        SetValueWithoutNotify(value);
                        SendEvent((EventBase)pooled);
                    }
                }
                else
                    SetValueWithoutNotify(value);
            }
        }
    
        public virtual void SetValueWithoutNotify(TValueType newValue)
        {
            m_Value = newValue;
            MarkDirtyRepaint();
        }
    }
}