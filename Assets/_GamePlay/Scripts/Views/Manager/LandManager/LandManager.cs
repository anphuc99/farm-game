using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using Models;
using Controllers;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;

namespace Views
{
    public class LandManager : MonoBehaviour
    {
        public static LandManager Instance { get; private set; }

        [SerializeField] private LandComponent landComponent;
        [SerializeField] private ButtonAddLandComponent buttonAddLandComponent;
        [SerializeField] private Vector2 vector1;
        [SerializeField] private Vector2 minPosition;
        [SerializeField] private Vector2 maxPosition;

        [NonSerialized] public bool isSetupComplete;

        private List<LandComponent> landComponents = new List<LandComponent>();

        private List<ButtonAddLandComponent> buttonAddLandComponents = new List<ButtonAddLandComponent>();
        private BagItem landSelect;
        private void Awake()
        {
            Instance = this;
            EventManager.Resgister(EventConstant.ON_LAND_SELECTED, OnLandSelected);
            EventManager.Resgister(EventConstant.ON_LAND_DESELECTED, OnLandDeselected);
        }

        private void OnLandSelected(object obj)
        {
            foreach (var land in buttonAddLandComponents)
            {
                land.gameObject.SetActive(true);
            }
            landSelect = (BagItem)obj;
        }

        private void OnLandDeselected(object obj)
        {
            foreach (var land in buttonAddLandComponents)
            {
                land.gameObject.SetActive(false);
            }
            landSelect = null;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (DataHelper.Instance.isNew)
            {
                Player player = PlayerController.GetPlayer();
                for(int i = 0; i < player.lands.Items.Count; i++)
                {
                    var child = transform.GetChild(i);
                    LandController.SetPositionLand(i, child.transform.position);
                }
            }

            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            SetUp();
            SpamPlusLand();
        }

        private void OnDestroy()
        {
            EventManager.UnResgister(EventConstant.ON_LAND_SELECTED, OnLandSelected);
            EventManager.UnResgister(EventConstant.ON_LAND_DESELECTED, OnLandDeselected);

            Instance = null;
            Player player = PlayerController.GetPlayer();
            player.lands.UnBind(OnAddLand);
        }

        public async void SetUp()
        {
            Player player = PlayerController.GetPlayer();
            for (int i = 0; i < player.lands.Items.Count; i++)
            {
                var land = player.lands.Items[i];
                AddLand(land, i);
            }

            player.lands.OnBind(OnAddLand);
            await Task.Delay(300);
            Pathfinding.Instance.CreateGrid();
            isSetupComplete = true;
        }

        public async void OnAddLand(int index, Land land, BindingStatus bindingStatus)
        {
            if(bindingStatus == BindingStatus.Add)
            {
                AddLand(land, index);
                await Task.Delay(300);
                Pathfinding.Instance.UpdateGrid();
            }
        }

        public void AddLand(Land land, int index)
        {
            var landComponent = Instantiate(this.landComponent, transform);
            landComponent.transform.position = new Vector2(land.posistion.X, land.posistion.Y);
            landComponent.SetUp(index);
            landComponents.Add(landComponent);
        }

        public void SpamPlusLand()
        {
            Vector2 startPosition = minPosition;
            SupSpamPlusLand(startPosition);
            SupSpamPlusLand(startPosition + vector1);
        }

        private void SupSpamPlusLand(Vector2 startPosition)
        {
            Player player = PlayerController.GetPlayer();
            var lands = player.lands;
            Transform worldCanvas = PositionHelper.GetPosition(PositionType.WorldCanvasPosition);
            for (float x = startPosition.x; x <= maxPosition.x; x += vector1.x * 2)
            {
                for (float y = startPosition.y; y >= maxPosition.y; y += vector1.y * 2)
                {
                    var findLandAlreadyOccupied = lands.Items.Find(a => a.posistion.X.Equals(x) && a.posistion.Y.Equals(y));

                    if(findLandAlreadyOccupied == null )
                    {
                        var button = Instantiate(buttonAddLandComponent, worldCanvas);
                        button.transform.position = new Vector2(x, y);
                        button.gameObject.SetActive(false);
                        button.SetUp(this);
                        buttonAddLandComponents.Add(button);
                    }

                }
            }
        }

        public void AddLandOnClick(ButtonAddLandComponent buttonAddLand)
        {
            if(landSelect != null)
            {
                LandController.AddLand(landSelect.id, buttonAddLand.transform.position);
                Destroy(buttonAddLand.gameObject);
                buttonAddLandComponents.Remove(buttonAddLand);
                PlayerManager.Instance.UpdateLandsUI();
            }
        }

        public LandComponent GetLandComponentByIndex(int index)
        {
            if(index < landComponents.Count)
            {
                return landComponents[index];
            }
            return null;
        }
    }
}
