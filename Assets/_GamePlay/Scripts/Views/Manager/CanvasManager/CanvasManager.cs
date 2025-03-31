using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _canvasUIs;

        private void Awake()
        {
            foreach (var canvas in _canvasUIs)
            {
                Instantiate(canvas, transform);
            }
        }
    }
}
