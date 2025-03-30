using Controllers;
using Models;
using Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class BagManager : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private BagComponent _bagCommponent;

        [SerializeField] private bool _onlyShowProduct;

        private List<BagComponent> _bagList = new List<BagComponent>();
        private void Start()
        {
            Player player = PlayerController.GetPlayer();
            var bag = player.bagItems;
            foreach (var item in bag.Items)
            {
                OnAddItem(item);
            }
            bag.OnBind(OnItemChange);
        }

        private void OnDestroy()
        {
            Player player = PlayerController.GetPlayer();
            var bag = player.bagItems;
            bag.UnBind(OnItemChange);
        }

        private void OnItemChange(int index, BagItem bagItem, BindingStatus status)
        {
            if (status == BindingStatus.Add)
            {
                OnAddItem(bagItem);
            }
            else if (status == BindingStatus.Remove)
            {
                OnRemoveItem(index, bagItem);
            }
        }

        private void OnAddItem(BagItem bagItem)
        {
            var bagComponent = Instantiate(_bagCommponent, _container);
            AddBagAction(bagComponent, bagItem);
            bagComponent.SetUp(bagItem, this);
            _bagList.Add(bagComponent);
            if(_onlyShowProduct)
            {
                if(bagItem.Type != TypeItem.Product)
                {
                    bagComponent.gameObject.SetActive(false);
                }
            }
        }

        private void AddBagAction(BagComponent bagComponent, BagItem bagItem)
        {
            switch(bagItem.Type)
            {
                case TypeItem.Seeds:
                    bagComponent.bagAction = new BagCultivationAction();
                    break;
                case TypeItem.Land:
                    bagComponent.bagAction = new BagLandAction();
                    break;
                case TypeItem.Product:
                    bagComponent.bagAction = new BagProductAction();
                    break;
            }
        }

        private void OnRemoveItem(int index, BagItem bagItem)
        {
            var bagComponent = _bagList[index];
            Destroy(bagComponent.gameObject);
            _bagList.RemoveAt(index);
        }

        public void OnBagSelect(BagComponent bagComponent)
        {
            foreach(var bag in _bagList)
            {
                if(bagComponent != bag)
                {
                    bag.OnOtherSelected(bagComponent);
                }
            }
        }
    }
}
