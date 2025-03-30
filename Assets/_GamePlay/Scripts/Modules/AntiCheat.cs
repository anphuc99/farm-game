using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Modules
{
    public class AntiCheat<T>
{
    // Dữ liệu được lưu dưới dạng chuỗi an toàn: "hash:encryptedData"
    private string _secureData;

    // Constructor mặc định
    public AntiCheat() { }

    // Constructor khởi tạo với giá trị ban đầu
    public AntiCheat(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Giá trị của biến kiểu T. Khi gán, dữ liệu sẽ được chuyển thành JSON,
    /// sau đó tính hash và mã hóa bằng AES thông qua EncryptionHelper.
    /// Khi lấy giá trị, dữ liệu được giải mã và kiểm tra tính toàn vẹn.
    /// </summary>
    public T Value
    {
        get
        {
            if (string.IsNullOrEmpty(_secureData))
                return default;

            string json = SecureDecrypt(_secureData);
            return JsonConvert.DeserializeObject<T>(json);
        }
        set
        {
            string json = JsonConvert.SerializeObject(value);
            _secureData = SecureEncrypt(json);
        }
    }

    /// <summary>
    /// Tính toán hash SHA256 cho chuỗi đầu vào.
    /// </summary>
    private string ComputeSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Mã hóa dữ liệu: tính hash của plainText và sau đó mã hóa plainText.
    /// Kết quả trả về có định dạng: hash:encryptedData
    /// </summary>
    private string SecureEncrypt(string plainText)
    {
        string hash = ComputeSHA256(plainText);
        string encryptedData = EncryptionHelper.Encrypt(plainText);
        return $"{hash}:{encryptedData}";
    }

    /// <summary>
    /// Giải mã dữ liệu an toàn: tách hash và dữ liệu mã hóa, giải mã và kiểm tra tính toàn vẹn.
    /// Nếu hash không khớp, ném ra Exception.
    /// </summary>
    private string SecureDecrypt(string secureCipherText)
    {
        // Dữ liệu phải có định dạng "hash:encryptedData"
        string[] parts = secureCipherText.Split(new char[] { ':' }, 2);
        if (parts.Length != 2)
        {
            throw new Exception("Dữ liệu không đúng định dạng.");
        }

        string storedHash = parts[0];
        string encryptedData = parts[1];
        string plainText = EncryptionHelper.Decrypt(encryptedData);
        string computedHash = ComputeSHA256(plainText);
        if (!string.Equals(storedHash, computedHash, StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Dữ liệu đã bị can thiệp!");
        }
        return plainText;
    }

    // Cho phép chuyển đổi ngầm (implicit) giữa AntiCheat<T> và T
    public static implicit operator T(AntiCheat<T> antiCheat)
    {
        return antiCheat.Value;
    }

    public static implicit operator AntiCheat<T>(T value)
    {
        return new AntiCheat<T>(value);
    }
}
}
