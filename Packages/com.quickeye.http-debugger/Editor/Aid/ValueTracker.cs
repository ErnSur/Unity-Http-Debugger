using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher.Aid
{
    public class ValueTracker<TValue> : BindableElement, INotifyValueChanged<TValue>
    {
        /// <summary> USS class name of elements of this type. </summary>
        public static readonly string ussClassName = $"editor-aid-{typeof(TValue).Name}-value-tracker";
        private TValue m_Value;

        /// <summary> Delegate called when value changes. </summary>
        public EventCallback<ChangeEvent<TValue>> valueChangedCallback {get; set;}

        /// <summary> ValueTracker constructor. </summary>
        public ValueTracker()
        {
            pickingMode = PickingMode.Ignore;
            style.display = DisplayStyle.None;
            AddToClassList(ussClassName);
        }

        /// <summary> Convenience ValueTracker constructor.</summary>
        /// <param name="propertyPath">Path of the property to be tracked</param>
        /// <param name="callback">Set a callback to be called when the property changes</param>
        /// <param name="initialValue">The initial value of the property. Set it to avoid triggering the callback when the element is bound.</param>
        public ValueTracker(
            string propertyPath,
            EventCallback<ChangeEvent<TValue>> callback = null,
            TValue initialValue = default) : this()
        {
            SetUp(propertyPath, callback, initialValue);
        }

        /// <summary> Convenience ValueTracker constructor.</summary>
        /// <param name="property">Property to be tracked</param>
        /// <param name="callback">Set a callback to be called when the property changes</param>
        /// <param name="initialValue">The initial value of the property. Set it to avoid triggering the callback when the element is bound.</param>
        public ValueTracker(
            SerializedProperty property,
            EventCallback<ChangeEvent<TValue>> callback = null,
            TValue initialValue = default) : this()
        {
            SetUp(property, callback, initialValue);
        }

        /// <summary>
        /// Convenience Setup method. It sets the <see cref="IBindable.bindingPath"/> of the tracker, registers a callback and sets an initial value.
        /// </summary>
        /// <param name="propertyPath">Path of the property to be tracked</param>
        /// <param name="callback">Set a callback to be called when the property changes</param>
        /// <param name="initialValue">The initial value of the property. Set it to avoid triggering the callback when the element is bound.</param>
        public void SetUp(string propertyPath, EventCallback<ChangeEvent<TValue>> callback, TValue initialValue = default)
        {
            SetValueWithoutNotify(initialValue);
            bindingPath = propertyPath;
            valueChangedCallback = callback;

            // There's a issue present at least in Unity 2020.2.7f1, maybe also in older/newer versions, where ChangeEvents are fired on every
            // binding, even if the value hasn't changed. We'll submit a bug report, but on the chance that Unity considers this an expected
            // behavior, here's a fix for us.
            this.RegisterValueChangedCallback(e =>
            {
                if (!EqualityComparer<TValue>.Default.Equals(e.previousValue, e.newValue))
                    valueChangedCallback?.Invoke(e);
            });
        }

        /// <summary>
        /// Convenience Setup method. It sets the <see cref="IBindable.bindingPath"/> of the tracker, registers a callback and sets an initial value.
        /// </summary>
        /// <param name="property">Property to be tracked</param>
        /// <param name="callback">Set a callback to be called when the property changes</param>
        /// <param name="initialValue">The initial value of the property. Set it to avoid triggering the callback when the element is bound.</param>
        public void SetUp(SerializedProperty property, EventCallback<ChangeEvent<TValue>> callback, TValue initialValue = default)
        {
            if (property == null)
            {
                Debug.LogError("Cannot bind tracker to a null SerializedProperty");
                return;
            }

            SetUp(property.propertyPath, callback, initialValue);
        }

        /// <summary>The value of the tracker updated by Unity with the property's value.</summary>
        public TValue value
        {
            get => m_Value;
            set
            {
                if (!EqualityComparer<TValue>.Default.Equals(m_Value, value))
                {
                    if (panel != null)
                    {
                        using (ChangeEvent<TValue> e = ChangeEvent<TValue>.GetPooled(m_Value, value))
                        {
                            e.target = this;
                            SetValueWithoutNotify(value);
                            SendEvent(e);
                        }
                    }
                    else
                    {
                        SetValueWithoutNotify(value);
                    }
                }
            }
        }

        /// <summary> Set the value of the tracker without updating the property or triggering callbacks.</summary>
        /// <param name="value">The new value</param>
        public void SetValueWithoutNotify(TValue value)
        {
            m_Value = value;
        }
    }
}