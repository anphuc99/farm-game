using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    /// <summary>
    /// Quản lý màn hình loading và hiệu ứng thanh tiến trình.
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }

        [SerializeField] private Image progressBar;

        private Tween tween;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void ShowLoading() => gameObject.SetActive(true);
        public async Task SetProgress(float progress, float time = 0)
        {
            var tcs = new TaskCompletionSource<bool>();
            tween?.Kill(true);
            tween = DOVirtual.Float(progressBar.fillAmount, progress / 100, time, value =>
            {
                progressBar.fillAmount = value;
            });
            tween.OnComplete(() => tcs.SetResult(true));
            await tcs.Task;
        }
        public void HideLoading() => gameObject.SetActive(false);
    }
}
