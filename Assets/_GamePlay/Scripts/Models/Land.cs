using Modules;
using Newtonsoft.Json;
using System;
using System.Numerics;

namespace Models
{
    public class Land : Model
    {
        [JsonIgnore]
        public Agriculture Agriculture
        {
            get
            {
                return _agriculture;
            }
            set
            {
                _agriculture = value;
                onChangeAgriculture?.Invoke();
            }
        }

        [JsonIgnore] public Action onChangeAgriculture;

        public int id;
        public Vector2 posistion;
        public Agriculture _agriculture;

    }
}
