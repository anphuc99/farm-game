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
        #region Singleton
        public static LoadingManager Instance { get; private set; }
        #endregion

        #region Inspector Fields
        [SerializeField] private Image progressBar;
        #endregion

        #region Private Fields
        private Tween tween;
        #endregion

        #region Unity Callbacks
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
        #endregion

        #region Public Methods
        /// <summary>
        /// Hiển thị màn hình loading.
        /// </summary>
        public void ShowLoading() => gameObject.SetActive(true);

        /// <summary>
        /// Cập nhật tiến trình cho thanh progress bar với hiệu ứng tween.
        /// </summary>
        /// <param name="progress">Giá trị tiến trình (0 đến 100).</param>
        /// <param name="time">Thời gian hiệu ứng (giây).</param>
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

        /// <summary>
        /// Ẩn màn hình loading.
        /// </summary>
        public void HideLoading() => gameObject.SetActive(false);
        #endregion
    }
}
