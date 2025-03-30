using Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class ButtonAddLandComponent : MonoBehaviour
    {
        private LandManager _landManager;

        public void SetUp(LandManager landManager)
        {
            _landManager = landManager;
        }

        public void AddLandOnClick()
        {
            _landManager.AddLandOnClick(this);
        }
    }
}
