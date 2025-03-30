using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Modules
{
    public class DataHelper
    {
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.dat");

        public static DataHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Load();
                }
                return instance;
            }
        }

        public bool isNew;

        private static DataHelper instance;

        private JObject _datas = new JObject();

        // auto-save
        private bool _isDirty = false;
        private float _autoSaveDelay = 5f;

        // Lưu dữ liệu xuống file (đã mã hóa AES)
        public void Save()
        {
            try
            {
                string json = _datas.ToString(Formatting.None);
                string encrypted = EncryptionHelper.Encrypt(json);
                File.WriteAllText(SaveFilePath, encrypted);
#if UNITY_EDITOR
                LoggerHelper.Log($"Data saved to: {SaveFilePath}");
#endif
            }
            catch (Exception e)
            {
                LoggerHelper.LogError($"Save failed: {e}");
            }
        }

        // Load dữ liệu từ file (giải mã AES)
        private static DataHelper Load()
        {
            DataHelper manager = new DataHelper();

            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string encrypted = File.ReadAllText(SaveFilePath);
                    string json = EncryptionHelper.Decrypt(encrypted);
                    manager._datas = JObject.Parse(json);
                }
                catch (Exception e)
                {
                    LoggerHelper.LogError($"Load failed: {e}");
                    manager._datas = new JObject(); // fallback
                }
            }
            else
            {
                manager.isNew = true;
                manager._datas = new JObject(); // Không có file lưu
            }

            return manager;
        }

        public void ClearData()
        {
            string filePath = SaveFilePath;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                LoggerHelper.Log("Đã xóa file dữ liệu tại: " + filePath);
            }
            else
            {
                LoggerHelper.LogWarning("Không tìm thấy file dữ liệu tại: " + filePath);
            }

            instance = null;

            // Reset dữ liệu trong DataManager.            
            LoggerHelper.Log("DataManager đã được reset.");
        }

#if UNITY_EDITOR
        public void ClearCache()
        {
            instance = null;
        }
#endif

        // Thêm hoặc cập nhật key với giá trị kiểu object.
        public void Set(string key, object value)
        {
            _datas[key] = JToken.FromObject(value);
            MarkDirty();
        }

        // Lấy giá trị theo key, có thể ép kiểu ra loại mong muốn.
        public T Get<T>(string key, T defaultValue = default)
        {
            if (_datas.TryGetValue(key, out var token))
            {
                try
                {
                    var chode = token.ToObject<T>();
                    return chode;
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        // Đánh dấu dữ liệu đã thay đổi 
        private void MarkDirty()
        {
            if (!_isDirty)
            {
                _isDirty = true;
                AutoSave();
            }
        }

        private async void AutoSave()
        {
            await Task.Delay(TimeSpan.FromSeconds(_autoSaveDelay));
            Save();
            _isDirty = false;
        }
    }
}

