using Modules;
using Newtonsoft.Json;

namespace Models
{
    public class BagItem : Model
    {
        [JsonIgnore]
        public TypeItem Type
        {
            get
            {
                ItemData itemData = ItemDatas.Instance.items.Find(x=>x.id == id);
                return itemData.type;
            }
        }

        public int id;
        public BindableAntiCheat<int> amount;        
    }
}


