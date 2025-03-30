using Controllers;
using Models;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class LandComponent : MonoBehaviour
    {
        public GameObject agriculturePosition;
        public GameObject tutorial;

        private Land land;
        [ShowInInspector] private int index;
        
        
        private StatusController curStatus;
        private BagItem _bagItemSelected;

        private void Awake()
        {
            EventManager.Resgister(EventConstant.ON_SEED_SELECTED, OnSeedSelected);
            EventManager.Resgister(EventConstant.ON_SEED_DESELECTED, OnDeseedSelected);
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (_bagItemSelected != null)
            {
                Cultivation(_bagItemSelected.id);
                tutorial.SetActive(false);
                _bagItemSelected = null;
            }

            if(land.Agriculture != null)
            {
                ShowInfo();
            }

        }

        private void OnDestroy()
        {
            EventManager.UnResgister(EventConstant.ON_SEED_SELECTED, OnSeedSelected);
            EventManager.UnResgister(EventConstant.ON_SEED_DESELECTED, OnDeseedSelected);
            if (land != null)
            {
                land.onChangeAgriculture -= OnChangeAgriculture;
            }
        }

        #region SetUp
        public void SetUp(int indexLand)
        {
            this.land = LandController.GetInfoLand(indexLand);
            index = indexLand;
            if (land.Agriculture != null)
            {
                UpdateStatus();
                StartCoroutine(UpdateStatusEverySecond());
            }

            land.onChangeAgriculture -= OnChangeAgriculture;
            land.onChangeAgriculture += OnChangeAgriculture;
        }        

        public void OnChangeAgriculture()
        {
            PlayerManager.Instance.UpdateLandsUI();
            if (land.Agriculture != null)
            {
                AddAgriculture();
            }
            else
            {
                DestroyAgriculture();
            }
        }
        #endregion
        #region Status
        public void AddAgriculture()
        {            
            UpdateStatus();
            StartCoroutine(UpdateStatusEverySecond());
        }

        public void DestroyAgriculture()
        {
            if (agriculturePosition.transform.childCount > 0)
            {
                Destroy(agriculturePosition.transform.GetChild(0).gameObject);
            }
            StopAllCoroutines();
            curStatus = StatusController.Other;
        }

        private IEnumerator UpdateStatusEverySecond()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            var status = LandController.GetStatusAgriculture(index);

            if(status == curStatus)
            {
                return;
            }

            if(agriculturePosition.transform.childCount > 0)
            {
                Destroy(agriculturePosition.transform.GetChild(0).gameObject);
            }


            switch (status)
            {
                case StatusController.AgricultureImmature:
                    SetUpJunior();
                    break;
                case StatusController.AgricultureHalfMature:
                    SetUpHalfMature(); 
                    break;
                case StatusController.AgricultureMature:
                case StatusController.AgricultureMatureLimit:
                    SetUpMature();
                    break;
                case StatusController.AgricultureDead:
                    SetUpDead();
                    break;
            }
            curStatus = status;
        }

        private void SetUpJunior()
        {
            var argicultureData = AgricultureDatas.Instance.items.Find(x=>x.itemID == land.Agriculture.id);
            var go = Instantiate(argicultureData.junior1, agriculturePosition.transform);
            go.transform.localPosition = Vector3.zero;
        }

        private void SetUpHalfMature()
        {
            var argicultureData = AgricultureDatas.Instance.items.Find(x => x.itemID == land.Agriculture.id);
            var go = Instantiate(argicultureData.junior2, agriculturePosition.transform);
            go.transform.localPosition = Vector3.zero;
        }

        private void SetUpMature()
        {
            var argicultureData = AgricultureDatas.Instance.items.Find(x => x.itemID == land.Agriculture.id);
            var go = Instantiate(argicultureData.mature, agriculturePosition.transform);
            go.transform.localPosition = Vector3.zero;
        }

        private void SetUpDead()
        {
            var argicultureData = AgricultureDatas.Instance.items.Find(x => x.itemID == land.Agriculture.id);
            var go = Instantiate(argicultureData.dead, agriculturePosition.transform);
            go.transform.localPosition = Vector3.zero;
        }
        #endregion
        #region Show Info
        private void ShowInfo()
        {            
            ShowInfoLandManager.Instance.ShowInfo(index, transform);
        }
        #endregion

        #region Cultivation
        public void Cultivation(int idAgriculture)
        {
            PlayerController.Cultivation(idAgriculture, index);
            UpdateStatus();
        }
        #endregion

        #region Seed
        private void OnSeedSelected(object obj)
        {
            if(land.Agriculture == null)
            {
                tutorial.gameObject.SetActive(true);
                _bagItemSelected = (BagItem)obj;
            }
        }
        private void OnDeseedSelected(object obj)
        {
            tutorial.gameObject.SetActive(false);
            _bagItemSelected = null;
        }

        #endregion

        #region cheat
        [Button]
        public void CheatSetUp(int indexLand)
        {
            SetUp(indexLand);
        }
        [Button]
        public void CheatCultivation(int idAgriculture)
        {
            Cultivation(idAgriculture);
        }

        [Button]
        public void CheatRemove()
        {
            LandController.RemoveAgriculture(index);
        }
        #endregion
    }
}
