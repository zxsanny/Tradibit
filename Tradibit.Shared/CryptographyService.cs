using System.Security.Cryptography;
using System.Text;
using Tradibit.SharedUI;

namespace Tradibit.Shared;

public class EncryptionService
{
    private static byte[] Key { get; }
    private static byte[] Iv { get; }
    
    static EncryptionService()
    {
        var key = Environment.GetEnvironmentVariable(Constants.AesKey, EnvironmentVariableTarget.Machine);
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("There is no key in env vars! Please add according keys!");
        Key = Encoding.UTF8.GetBytes(key);
        
        var iv =  Environment.GetEnvironmentVariable(Constants.AesIv, EnvironmentVariableTarget.Machine);
        if (string.IsNullOrEmpty(iv)) throw new ArgumentException("There is no key in env vars! Please add according keys!");
        Iv = Encoding.UTF8.GetBytes(iv);
    }
    
    public static string Encrypt(string plainText)
    {
        if (plainText is not { Length: > 0 }) throw new ArgumentNullException(nameof(plainText));
        if (Key is not { Length: > 0 }) throw new ArgumentNullException(nameof(Key));
        if (Iv is not { Length: > 0 }) throw new ArgumentNullException(nameof(Iv));
        
        using var aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = Iv;
        
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        
        swEncrypt.Write(plainText);
        return Encoding.UTF8.GetString(msEncrypt.ToArray());
    }
    
    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText)) throw new ArgumentNullException(nameof(encryptedText));
        
        var cipherText = Encoding.UTF8.GetBytes(encryptedText); 
        
        using var aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = Iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }
}
