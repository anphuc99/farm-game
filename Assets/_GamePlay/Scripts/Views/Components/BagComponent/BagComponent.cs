using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BagComponent : MonoBehaviour
    {
        [SerializeField] private Image _imgFrame;
        [SerializeField] private TextMeshProUGUI _txtAmount;
        [SerializeField] public Image imgAvatar;
        [SerializeField] private Sprite _sprNormal;
        [SerializeField] private Sprite _sprSelect;

        [NonSerialized] public bool isSelected;

        [NonSerialized] public IBagAction bagAction;

        private BagItem _bagItem;
        private BagManager _bagManager;

        private void OnDestroy()
        {
            _bagItem.amount.UnBind(OnAmountChange);
            bagAction.OnDeselected();
        }

        private void Update()
        {
            if (isSelected)
            {
                bagAction.OnUpdateSelected();
            }
        }

        public void SetUp(BagItem bagItem, BagManager bagManager)
        {
            _bagItem = bagItem;
            _bagManager = bagManager;
            var itemData = ItemDatas.Instance.items.Find(x=>x.id == bagItem.id);
            imgAvatar.sprite = itemData.avatar;
            imgAvatar.SetNativeSize();
            bagItem.amount.OnBind(OnAmountChange);
            OnAmountChange(bagItem.amount.Value);
            bagAction.SetUp(bagItem, this);
        }

        public void OnAmountChange(int amount)
        {
            _txtAmount.text = amount.ToString();
        }

        public void OnSelected()
        {
            if (isSelected)
            {
                _imgFrame.sprite = _sprNormal;
                isSelected = false;
                bagAction.OnDeselected();
                return;
            }

            _imgFrame.sprite = _sprSelect;
            isSelected = true;
            _bagManager.OnBagSelect(this);
            bagAction.OnSelected();
        }

        public void OnOtherSelected(BagComponent bagComponent)
        {
            _imgFrame.sprite = _sprNormal;
            isSelected = false;
            bagAction.OnDeselected();
        }
    }
}
