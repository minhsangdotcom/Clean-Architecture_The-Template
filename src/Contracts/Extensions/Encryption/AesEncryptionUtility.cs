using System.Security.Cryptography;
using System.Text;

namespace Contracts.Extensions.Encryption;

public class AesEncryptionUtility
{
    private static readonly int KeySize = 256; // AES-256
    private static readonly int BlockSize = 128;
    
    public static string Encrypt(string plainText, string key)
    {
        // Generate an IV (Initialization Vector)
        byte[] iv = new byte[16]; // AES block size is 16 bytes
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        // Convert the key and plaintext to byte arrays
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32)); // Ensure 32-byte key for AES-256
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Key = keyBytes;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        // Write the IV first, as we'll need it for decryption
        msEncrypt.Write(iv, 0, iv.Length);

        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            csEncrypt.Write(plainBytes, 0, plainBytes.Length);
            csEncrypt.FlushFinalBlock();
        }

        // Get the encrypted bytes from the memory stream
        byte[] encryptedBytes = msEncrypt.ToArray();

        // Convert the encrypted bytes to Base64 string
        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string encryptedText, string key)
    {
        // Convert the encrypted text and key to byte arrays
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32)); // Ensure 32-byte key for AES-256

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Key = keyBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract the IV from the encrypted bytes
        byte[] iv = new byte[16];
        Array.Copy(encryptedBytes, iv, iv.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        // Return the decrypted string
        return srDecrypt.ReadToEnd();
    }
}
