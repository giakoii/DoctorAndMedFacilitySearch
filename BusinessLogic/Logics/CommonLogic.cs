using System.Security.Cryptography;
using System.Text;
using BusinessLogic.Services;
using DataAccessObject.Models;

namespace BusinessLogic.Logics;

/// <summary>
/// Common logic
/// </summary>
public static class CommonLogic
{
    /// <summary>
    /// Encrypt the text
    /// </summary>
    /// <param name="beforeEncrypt"></param>
    /// <param name="systemConfig"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EncryptText(string beforeEncrypt, IBaseService<SystemConfig, string, VwSystemConfig> systemConfig)
    {
        // Check for null or empty
        ArgumentException.ThrowIfNullOrEmpty(beforeEncrypt);
        
        // Get the system config
        var key = systemConfig.Find(x => x.Id == BusinessLogic.Utils.SystemConfig.EncryptKey)?.FirstOrDefault()?.Value;
        var iv = systemConfig.Find(x => x.Id == BusinessLogic.Utils.SystemConfig.EncryptIv)?.FirstOrDefault()?.Value;
        // Check for null
        if (key == null)
        {
            throw new ArgumentException();
        }
        // Encrypt the text
        using (Aes aes = Aes.Create())
        {
            // Set the key and IV
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            
            // Encrypt
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(beforeEncrypt);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }   
        }
    }

    /// <summary>
    /// Decrypt the text
    /// </summary>
    /// <param name="beforeDecrypt"></param>
    /// <param name="systemConfig"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string DecryptText(string beforeDecrypt, IBaseService<SystemConfig, string, VwSystemConfig> systemConfig)
    {
        // Check for null or empty
        ArgumentException.ThrowIfNullOrEmpty(beforeDecrypt);
        // Get the system config
        var key = systemConfig.Find(x => x.Id == BusinessLogic.Utils.SystemConfig.EncryptKey)?.FirstOrDefault()?.Value;
        var iv = systemConfig.Find(x => x.Id == BusinessLogic.Utils.SystemConfig.EncryptIv)?.FirstOrDefault()?.Value;
        // Check for null
        if (key == null)
        {
            throw new ArgumentException();
        }
        // Decrypt the text
        using (Aes aes = Aes.Create())
        {
            // Set the key and IV
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            // Decrypt
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(beforeDecrypt)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}