using Controllers;
using Models;
using Modules;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Views
{
    public class GameManager : MonoBehaviour
    {        
        public static GameManager Instance { get; private set; }

        private const float TIME_UPDATE_LASTEPLAYGAME = 60;

        private float _curTimeUpdateLastPlayGame = TIME_UPDATE_LASTEPLAYGAME;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforLoadGame()
        {
            SceneManager.LoadScene(0);
        }
#endif

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
            if(player.lastTimePlayGame != -1 && player.money.Value < GameSetting.Instance.moneyToCompleteGame)
            {
                DataHelper.Instance.blockSave = true;
                LoadingManager.Instance.SetProgress(20, 2);
                await WorkerController.AutoWork();   
                DataHelper.Instance.blockSave = false;
                DataHelper.Instance.Save();
            }            
            SceneManager.LoadSceneAsync(SceneConstant.GAME_PLAY);
            await LoadingManager.Instance.SetProgress(100, 1);
            LoadingManager.Instance.HideLoading();
            player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
            player.money.OnBind(CompleteGame);
            CompleteGame(player.money.Value);
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

        private void OnDestroy()
        {
            Player player = Collection.LoadModel<Player>();
            player.money.UnBind(CompleteGame);
        }

        private void OnApplicationQuit()
        {
            DataHelper.Instance.Save();
        }

        private void CompleteGame(int money)
        {
            if(money >= GameSetting.Instance.moneyToCompleteGame)
            {
                EventManager.Emit(EventConstant.ON_COMPLETE_GAME, true);
            }
        }

        public void ResetGame()
        {
            DataHelper.Instance.ClearData();
            Collection.Clear();
            SceneManager.LoadScene(0);
            Start();
        }
    }
}
