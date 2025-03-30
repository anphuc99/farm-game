namespace Modules
{
    public class BindableAntiCheat<T> : Bindable<T>
    {
        private AntiCheat<T> _value;
        public override T Value 
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public BindableAntiCheat(T value = default) : base(value) 
        {
            _value = value;
        }
    }
}
