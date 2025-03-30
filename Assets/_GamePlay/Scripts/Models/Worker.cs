using Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public enum StateWorker
    {
        None,
        Cultivation,
        Harvest,
        CleanDeadAgriculture,
    }

    public class Worker : Model
    {
        public Bindable<StateWorker> stateWorker = new Bindable<StateWorker>(StateWorker.None);
        public int workingInLand = -1;
        public long timeStartWork;

    }
}