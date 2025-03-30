using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class FactoryManager : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            ShowInfoUpgrade.Instance.ShowInfo(transform);
        }
    }
}
