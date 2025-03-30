using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Models;
using Controllers;

namespace Views
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _txtCoin;
        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtLands;
        [SerializeField] private TextMeshProUGUI _txtWorker;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Player player = PlayerController.GetPlayer();
            player.money.OnBind(OnChangeCoin);
            player.level.OnBind(OnChangeLevel);
            OnChangeCoin(player.money.Value);
            OnChangeLevel(player.level.Value);
            UpdateLandsUI();
            UpdateWorkerUI();
        }

        private void OnDestroy()
        {
            Player player = PlayerController.GetPlayer();
            player.money.UnBind(OnChangeCoin);
            player.level.UnBind(OnChangeLevel);
            Instance = null;
        }

        private void OnChangeCoin(int coin)
        {
            _txtCoin.text = coin.ToString();
        }

        private void OnChangeLevel(int level)
        {
            _txtLevel.text = level.ToString();
        }

        public void UpdateLandsUI()
        {
            var lands = LandController.GetLands();

            int countLandHasAgriculture = 0;            

            foreach(var land in lands.Items)
            {
                if (land.Agriculture != null)
                {                    
                    countLandHasAgriculture++;
                }                
            }

            _txtLands.text = $"{countLandHasAgriculture}/{lands.Items.Count}";
        }        

        public void UpdateWorkerUI()
        {
            var workers = WorkerController.GetWorkers();

            int countWorkerWorking = 0;
            foreach(var worker in workers.Items)
            {
                if(worker.stateWorker.Value != StateWorker.None)
                {
                    countWorkerWorking++;
                }
            }

            _txtWorker.text = $"{countWorkerWorking}/{workers.Items.Count}";
        }

        public void OnAddLevel()
        {
            Vector2 pos = PositionHelper.GetPosition(PositionType.HomePositon).position;

            Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
        }

        public void OnAddLands()
        {
            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION_YES_NO, new NoticationYesNoParam()
            {
                title = TextConstant.NOTIFICATION,
                description = string.Format(TextConstant.EXPAND_LAND, GameSetting.Instance.expandLandCost),
                btnYes = new NoticationYesNoParam.btn()
                {
                    text = TextConstant.YES,
                    onClick = () =>
                    {
                        StatusController status = PlayerController.ExpandLand();
                        if (status == StatusController.Success)
                        {
                            UpdateLandsUI();
                            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION, new NoticationParam()
                            {
                                title= TextConstant.NOTIFICATION,
                                description = TextConstant.EXPAND_LAND_SUCCESS,
                                button = new NoticationParam.btn()
                                {
                                    text= TextConstant.OK,
                                    onClick = () => 
                                    {
                                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO);
                                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION);
                                        
                                    }
                                }
                            });
                        }
                        else
                        {
                            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION, new NoticationParam()
                            {
                                title = TextConstant.NOTIFICATION,
                                description = TextConstant.statusController[status],
                                button = new NoticationParam.btn()
                                {
                                    text = TextConstant.OK,
                                    onClick = () => PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION)
                                }
                            });
                        }
                    }
                },

                btnNo = new NoticationYesNoParam.btn()
                {
                    text= TextConstant.NO,
                    onClick = () =>
                    {
                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO);
                    }
                }
            });
        }

        public void OnAddWorker()
        {
            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION_YES_NO, new NoticationYesNoParam()
            {
                title = TextConstant.NOTIFICATION,
                description = string.Format(TextConstant.EXPAND_LAND, GameSetting.Instance.expandLandCost),
                btnYes = new NoticationYesNoParam.btn()
                {
                    text = TextConstant.YES,
                    onClick = () =>
                    {
                        StatusController status = WorkerController.HireWorker();
                        if (status == StatusController.Success)
                        {
                            UpdateWorkerUI();
                            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION, new NoticationParam()
                            {
                                title = TextConstant.NOTIFICATION,
                                description = TextConstant.EXPAND_LAND_SUCCESS,
                                button = new NoticationParam.btn()
                                {
                                    text = TextConstant.OK,
                                    onClick = () =>
                                    {
                                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO);
                                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION);

                                    }
                                }
                            });
                        }
                        else
                        {
                            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION, new NoticationParam()
                            {
                                title = TextConstant.NOTIFICATION,
                                description = TextConstant.statusController[status],
                                button = new NoticationParam.btn()
                                {
                                    text = TextConstant.OK,
                                    onClick = () => PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION)
                                }
                            });
                        }
                    }
                },

                btnNo = new NoticationYesNoParam.btn()
                {
                    text = TextConstant.NO,
                    onClick = () =>
                    {
                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO);
                    }
                }
            });
        }
    }
}
