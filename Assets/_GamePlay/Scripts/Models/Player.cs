using Modules;

namespace Models
{
    public class Player : Model
    {
        public BindableAntiCheat<int> money;
        public BindableAntiCheat<int> level;

        public BindableList<Worker> workers;
        public BindableList<Land> lands;
        public BindableList<BagItem> bagItems;

        public long lastTimePlayGame = -1;

        public override void SetDefault()
        {
            base.SetDefault();
            money = new BindableAntiCheat<int>(GameSetting.Instance.initialMoney);
            level = new BindableAntiCheat<int>(0);

            lands = new BindableList<Land>();
            for (int i = 0; i < GameSetting.Instance.initialLandPlots; i++)
            {
                lands.Add(new Land());
            }

            workers = new BindableList<Worker>();

            for (int i = 0; i < GameSetting.Instance.initialWorkers; i++)
            {
                workers.Add(new Worker());
            }

            bagItems = new BindableList<BagItem>();
            for (int i = 0; i < GameSetting.Instance.initialItems.Count; i++)
            {
                var initItem = GameSetting.Instance.initialItems[i];
                bagItems.Add(new BagItem() { id = initItem.id, amount = new BindableAntiCheat<int>(initItem.amount) });
            }
        }
    }
}


