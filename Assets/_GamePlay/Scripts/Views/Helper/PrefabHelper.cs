using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class PrefabHelper : MonoBehaviour
    {
        public static PrefabHelper Instance {  get; private set; }

        public GameObject landPrefab;

        private void Awake()
        {
            Instance = this;
        }
    }
}
