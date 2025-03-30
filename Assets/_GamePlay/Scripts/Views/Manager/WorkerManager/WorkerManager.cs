using Controllers;
using DG.Tweening;
using Models;
using Modules;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Views
{
    public class WorkerManager : MonoBehaviour
    {
        [SerializeField] private WorkerComponent _workerComponent;

        private List<WorkerComponent> _workerList = new List<WorkerComponent>();
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => LandManager.Instance.isSetupComplete);
            var workers = WorkerController.GetWorkers();
            for(int i = 0; i < workers.Items.Count; i++) 
            {
                AddWorker(i, workers.Items[i]);
            }

            workers.OnBind(OnChangeWorker);
        }

        private void OnChangeWorker(int index, Worker worker, BindingStatus status)
        {
            if(status == BindingStatus.Add)
            {
                AddWorker(index, worker);
            }
        }

        public void AddWorker(int index, Worker worker)
        {
            var workerComponent = Instantiate(_workerComponent, transform);
            workerComponent.transform.position = Vector3.zero;
            workerComponent.SetUp(index, worker, this);
            _workerList.Add(workerComponent);
        }
    }
}
