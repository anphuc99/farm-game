using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class DontDestroyOnLoadHelper : MonoBehaviour
    {        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
