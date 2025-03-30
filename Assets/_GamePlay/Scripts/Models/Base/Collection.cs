using Modules;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Models
{
    public static class Collection
    {
        private static Dictionary<string, object> _models = new Dictionary<string, object>();
        public static T LoadModel<T>() where T : Model, new()
        {
            if (_models.ContainsKey(typeof(T).Name))
            {
                return (T)_models[typeof(T).Name];
            }
            else
            {                
                T model = DataHelper.Instance.Get<T>(typeof(T).Name);
                if(model == null)
                {
                    model = new T();
                    model.SetDefault();
                }
                _models.Add(typeof(T).Name, model);
                return model;
            }
        }

        public static void SaveModel<T>(T model, bool isSaveImmediate = false) where T : Model, new()
        {
            DataHelper.Instance.Set(typeof(T).Name, model);
            if (isSaveImmediate)
            {
                DataHelper.Instance.Save();
            }
        }

        public static void Clear()
        {
            _models.Clear();
        }
    }
}


