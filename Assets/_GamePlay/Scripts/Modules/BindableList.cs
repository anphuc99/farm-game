using System.Collections.Generic;
using System;

namespace Modules
{
    public enum BindingStatus
    {
        Add,
        Remove,
    }
    public class BindableList<T>
    {
        private List<T> _items;
        private Action<int, T, BindingStatus> _actionBind;
        private Action<List<T>> _actionBindList;
        public BindableList()
        {
            _items = new List<T>();
        }

        public List<T> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Add(T item)
        {
            _items.Add(item);
            OnPropertyChanged();
            OnPropertyChanged(_items.Count - 1,item, BindingStatus.Add);
        }

        public void Remove(T item)
        {
            int index = _items.IndexOf(item);
            _items.Remove(item);
            OnPropertyChanged();
            OnPropertyChanged(index, item, BindingStatus.Remove);
        }

        public void RemoveAt(int index)
        {
            T item = _items[index];
            _items.RemoveAt(index);
            OnPropertyChanged();
            OnPropertyChanged(index, item, BindingStatus.Remove);
        }

        public void Clear()
        {
            _items.Clear();
            OnPropertyChanged();
        }

        protected void OnPropertyChanged()
        {
            _actionBindList?.Invoke(Items);
        }

        protected void OnPropertyChanged(int index, T item, BindingStatus status)
        {
            _actionBind?.Invoke(index, item, status);
        }

        public void OnBind(Action<int, T, BindingStatus> action)
        {
            _actionBind += action;
        }

        public void UnBind(Action<int, T, BindingStatus> action)
        {
            _actionBind -= action;
        }

        public void OnBind(Action<List<T>> action)
        {
            _actionBindList += action;
        }

        public void UnBind(Action<List<T>> action)
        {
            _actionBindList -= action;
        }
    }
}
