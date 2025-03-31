using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class EndGameManager : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.Resgister(EventConstant.ON_COMPLETE_GAME, OnCompleteGame);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.UnResgister(EventConstant.ON_COMPLETE_GAME, OnCompleteGame);
        }

        private void OnCompleteGame(object obj)
        {
            gameObject.SetActive(true);
        }

        public void OnButtonRestartGameClick()
        {
            GameManager.Instance.ResetGame();
        }
    }
}
