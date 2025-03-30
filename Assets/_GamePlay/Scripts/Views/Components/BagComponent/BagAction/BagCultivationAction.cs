using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BagCultivationAction : IBagAction
    {
        private BagItem _bagItem;
        private BagComponent _bagComponent;

        public void SetUp(BagItem bagItem, BagComponent bagComponent)
        {
            _bagComponent = bagComponent;
            _bagItem = bagItem;
        }

        public void OnDeselected()
        {
            EventManager.Emit(EventConstant.ON_SEED_DESELECTED, _bagItem);
        }

        public void OnSelected()
        {
            EventManager.Emit(EventConstant.ON_SEED_SELECTED, _bagItem);
        }

        public void OnUpdateSelected()
        {
            
        }
    }
}
