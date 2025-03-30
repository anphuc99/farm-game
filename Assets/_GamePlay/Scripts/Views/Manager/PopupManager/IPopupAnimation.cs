using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public interface IPopupAnimation
    {
        public void OnShow(GameObject view, Action onComplete);
        public void OnHide(GameObject view, Action onComplete);
    }
}


