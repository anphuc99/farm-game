using Modules;

namespace Models
{
    public class Agriculture : Model
    {
        public int id;
        public long cultivationTime;
        public BindableAntiCheat<int> amountProductPicked = new BindableAntiCheat<int>(0);
    }
}


