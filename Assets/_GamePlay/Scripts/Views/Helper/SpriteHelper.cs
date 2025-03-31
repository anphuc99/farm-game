using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    [Serializable]
    public class SpriteData
    {
        public string key;
        public Sprite sprite;
    }

    public class SpriteHelper : MonoBehaviour
    {
        public static SpriteHelper Instance { get; private set; }
        
        [TableList]
        [SerializeField] private List<SpriteData> sprites;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public Sprite GetSprite(string key)
        {
            var spr = sprites.Find(x => x.key == key);
            if (spr == null)
            {
                return null;
            }
            return spr.sprite;
        }


    }
}
