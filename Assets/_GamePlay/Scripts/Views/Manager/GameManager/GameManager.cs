using Controllers;
using Models;
using Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Views
{
    public class GameManager : MonoBehaviour
    {        
        public static GameManager Instance { get; private set; }

        private const float TIME_UPDATE_LASTEPLAYGAME = 60;

        private float _curTimeUpdateLastPlayGame = TIME_UPDATE_LASTEPLAYGAME;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            var gameSettin = GameSetting.Instance;
            LoadingManager.Instance.ShowLoading();
            Player player = Collection.LoadModel<Player>();
            if(player.lastTimePlayGame != -1)
            {
                LoadingManager.Instance.SetProgress(20, 2);
                await WorkerController.AutoWork();                
            }            
            SceneManager.LoadSceneAsync(SceneConstant.GAME_PLAY);
            await LoadingManager.Instance.SetProgress(100, 1);
            LoadingManager.Instance.HideLoading();
            player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
            Collection.SaveModel(player);
        }
        
        private void Update()
        {
            if(_curTimeUpdateLastPlayGame <= 0)
            {
                Player player = Collection.LoadModel<Player>();
                player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
                _curTimeUpdateLastPlayGame = TIME_UPDATE_LASTEPLAYGAME;
                Collection.SaveModel(player);
            }
            else
            {
                _curTimeUpdateLastPlayGame -= Time.deltaTime;
            }
        }

        private void OnApplicationQuit()
        {
            DataHelper.Instance.Save();
        }
    }
}
