using Models;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class CheatTime : MonoBehaviour
    {
        [Button]
        public void Add5s()
        {
            DateTimeHelper.dateAddTime += 5;
        }

        [Button]
        public void Add60s()
        {
            DateTimeHelper.dateAddTime += 60;
        }

        [Button]
        public void Add600s()
        {
            DateTimeHelper.dateAddTime += 600;
        }

        [Button]
        public void Add6000s()
        {
            DateTimeHelper.dateAddTime += 6000;
        }

        [Button]
        public void AddMoney(int money)
        {
            Player player = Collection.LoadModel<Player>();
            player.money.Value += money;
            Collection.SaveModel(player);   
        }
    }
}
