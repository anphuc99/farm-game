using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class BagProductAction : IBagAction
    {
        private BagItem _bagItem;
        private BagComponent _bagComponent;

        public void SetUp(BagItem bagItem, BagComponent bagComponent)
        {
            _bagItem = bagItem;
            _bagComponent = bagComponent;
        }
        public void OnSelected()
        {
            EventManager.Emit(EventConstant.ON_PRODUCT_SELECTED, _bagItem);
        }

        public void OnDeselected()
        {
            EventManager.Emit(EventConstant.ON_PRODUCT_DESELECTED, _bagItem);
        }


        public void OnUpdateSelected()
        {
        }
    }
}
