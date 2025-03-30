using Codice.CM.Common;
using Controllers;
using DG.Tweening;
using Models;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Views
{
    public class WorkerComponent : MonoBehaviour
    {
        private const float TIME_UPDATE_PROGRESS = 1f;

        [ShowInInspector] StateWorker state => _worker?.stateWorker.Value ?? StateWorker.None;

        [SerializeField] private ProgressBarHelper _progressBarPrefab;
        [SerializeField] private Transform _progressBarPosition;
        [SerializeField] private float _speed = 3f;
        private float _nodeThreshold = 0.1f;        

        private Worker _worker;
        private int _index;
        private WorkerManager _workerManager;
        private int _curWorkInLand = -1;
        private ProgressBarHelper _progressBar;
        
        private Vector3 target;        
        private List<Vector3> paths;
        
        public void SetUp(int index, Worker worker, WorkerManager workerManager)
        {
            _worker = worker;
            _index = index;
            _workerManager = workerManager;
            Transform worldCanvas = PositionHelper.GetPosition(PositionType.WorldCanvasPosition);
            _progressBar = Instantiate(_progressBarPrefab, worldCanvas);
            _progressBar.gameObject.SetActive(false);
            _progressBar.MinValue = 0;
            _progressBar.MaxValue = 100;
            StartCoroutine(UpdateState());
        }
        void Update()
        {
            if(_progressBar != null)
            {
                _progressBar.transform.position = _progressBarPosition.transform.position;
            }
            if (paths != null && paths.Count > 0)
            {
                // Lấy node tiếp theo trong đường đi
                Vector2 nextNode = paths[0];
                Vector2 currentPos = transform.position;

                // Di chuyển đến node tiếp theo
                transform.position = Vector2.MoveTowards(currentPos, nextNode, _speed * Time.deltaTime);

                // Khi đạt gần node, loại bỏ node đó và chuyển sang node tiếp theo
                if (Vector2.Distance(currentPos, nextNode) < _nodeThreshold)
                {
                    paths.RemoveAt(0);
                }
            }
        }

        public async void SetState()
        {
            StateWorker stateWorker = _worker.stateWorker.Value;
            if (_curWorkInLand == _worker.workingInLand)
            {
                return;
            }
            PlayerManager.Instance.UpdateWorkerUI();
            _curWorkInLand = _worker.workingInLand;
            if (stateWorker == StateWorker.None)
            {
                Vector3 startPos = transform.position;
                paths = await Task.Run(() => AStarPathfinding.Instance.FindPathAsVector3(startPos, Vector3.zero));
                target = Vector3.zero;
            }
            else
            {
                await Task.Delay(500);
                Vector3 target = LandManager.Instance.GetLandComponentByIndex(_worker.workingInLand).transform.position;
                Vector3 startPos = transform.position;
                paths = await Task.Run(() => AStarPathfinding.Instance.FindPathAsVector3(startPos, target));
                this.target = target;
            }
        }

        private void UpdateProgressBar()
        {
            StateWorker stateWorker = _worker.stateWorker.Value;
            if(stateWorker != StateWorker.None && paths != null && paths.Count <= 0)
            {
                float progress = WorkerController.GetProgressWork(_index);
                if (progress > 2)
                {
                    _progressBar.gameObject.SetActive(true);
                    _progressBar.Value = progress;                    
                }
            }
            else
            {
                _progressBar.gameObject.SetActive(false);
            }
        }

        public IEnumerator UpdateState()
        {
            while(true)
            {
                WorkerController.UpdateWorker(_index);
                SetState();
                UpdateProgressBar();
                yield return new WaitForSeconds(TIME_UPDATE_PROGRESS);
            }
        }
    }
}
