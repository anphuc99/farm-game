using System;
using System.Collections.Generic;

namespace Modules
{
    public class Bindable<T>
    {
        private Action<T> _actionBind;

        private T _value;

        public virtual T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public Bindable(T value = default)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        protected void OnPropertyChanged()
        {           
            _actionBind?.Invoke(Value);
        }

        public void OnBind(Action<T> action)
        {
            _actionBind += action;
        }

        public void UnBind(Action<T> action)
        {
            _actionBind -= action;
        }        
    }
}
