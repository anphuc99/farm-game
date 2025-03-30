using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public interface IBagAction
    {
        public void SetUp(BagItem bagItem, BagComponent bagComponent);
        public void OnSelected();
        public void OnDeselected();
        public void OnUpdateSelected();
    }
}
