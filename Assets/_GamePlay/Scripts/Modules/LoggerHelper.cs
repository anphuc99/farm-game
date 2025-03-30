using System.Runtime.CompilerServices;
using UnityEngine;

public static class LoggerHelper
{
    /// <summary>
    /// Ghi log thông thường kèm thông tin nơi log được gọi.
    /// </summary>
    /// <param name="message">Nội dung log</param>
    /// <param name="memberName">Tên hàm gọi (tự động lấy)</param>
    /// <param name="sourceFilePath">Đường dẫn file gọi (tự động lấy)</param>
    /// <param name="sourceLineNumber">Số dòng gọi (tự động lấy)</param>
    public static void Log(object message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        Debug.Log($"{message}\n (Called from {memberName} in {sourceFilePath} at line {sourceLineNumber})");
    }

    /// <summary>
    /// Ghi log cảnh báo kèm thông tin nơi log được gọi.
    /// </summary>
    /// <param name="message">Nội dung cảnh báo</param>
    /// <param name="memberName">Tên hàm gọi (tự động lấy)</param>
    /// <param name="sourceFilePath">Đường dẫn file gọi (tự động lấy)</param>
    /// <param name="sourceLineNumber">Số dòng gọi (tự động lấy)</param>
    public static void LogWarning(object message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        Debug.LogWarning($"{message}\n (Called from {memberName} in {sourceFilePath} at line {sourceLineNumber})");
    }

    /// <summary>
    /// Ghi log lỗi kèm thông tin nơi log được gọi.
    /// </summary>
    /// <param name="message">Nội dung lỗi</param>
    /// <param name="memberName">Tên hàm gọi (tự động lấy)</param>
    /// <param name="sourceFilePath">Đường dẫn file gọi (tự động lấy)</param>
    /// <param name="sourceLineNumber">Số dòng gọi (tự động lấy)</param>
    public static void LogError(object message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        Debug.LogError($"{message}\n (Called from {memberName} in {sourceFilePath} at line {sourceLineNumber})");
    }
}
