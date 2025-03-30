using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Views
{
    [ExecuteAlways]
    public class ProgressBarHelper : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent OnValueChanged; // Sự kiện xuất hiện trong Inspector

        public float Value
        {
            get { return _value; }
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                UpdateFillAmount();
                // Kích hoạt event khi giá trị thay đổi
                OnValueChanged?.Invoke();
            }
        }

        public float MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                UpdateFillAmount();
            }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                UpdateFillAmount();
            }
        }

        [SerializeField] private float _value = 0;
        [SerializeField] private float _minValue = 0;
        [SerializeField] private float _maxValue = 1;
        [SerializeField] private Image _mask;

        // Phương thức này sẽ được gọi trong Editor mỗi khi có thay đổi trong Inspector
        private void OnValidate()
        {
            _value = Mathf.Clamp(_value, _minValue, _maxValue);
            UpdateFillAmount();
        }

        private void Start()
        {
            UpdateFillAmount();
        }

        // Cập nhật fillAmount dựa trên giá trị hiện tại
        private void UpdateFillAmount()
        {
            if (_mask != null)
            {
                float normalizedValue = (_value - _minValue) / (_maxValue - _minValue);
                _mask.fillAmount = Mathf.Clamp01(normalizedValue);
            }
            else
            {
                Debug.LogWarning("ProgressBarHelper: Chưa gán Image _mask.");
            }
        }

        // Phương thức cập nhật tiến độ từ bên ngoài
        public void SetProgress(float progress)
        {
            Value = progress;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProgressBarHelper))]
    public class ProgressBarHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Hiển thị UnityEvent OnValueChanged và các property con của nó.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnValueChanged"), true);

            // Vẽ các trường _minValue và _maxValue
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_minValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxValue"));

            // Lấy giá trị min và max từ các property tương ứng
            float minValue = serializedObject.FindProperty("_minValue").floatValue;
            float maxValue = serializedObject.FindProperty("_maxValue").floatValue;

            // Vẽ slider cho _value với giới hạn từ _minValue đến _maxValue
            SerializedProperty valueProp = serializedObject.FindProperty("_value");
            valueProp.floatValue = EditorGUILayout.Slider("Value", valueProp.floatValue, minValue, maxValue);

            // Vẽ trường _mask
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_mask"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}
