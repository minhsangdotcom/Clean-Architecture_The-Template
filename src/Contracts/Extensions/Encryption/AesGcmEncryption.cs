using System.Security.Cryptography;
using System.Text;

namespace Contracts.Extensions.Encryption;

public class AesGcmEncryption
{
    private static readonly int _keySize = 32;

    ///32 = 256bits encryption key (32 * 8).
    private static readonly int _nonceSize = AesGcm.NonceByteSizes.MaxSize;
    private static readonly int _tagSize = AesGcm.TagByteSizes.MaxSize;

    private static int KeySize
    {
        get { return _keySize; }
    }
    private static int NonceSize
    {
        get { return _nonceSize; }
    }
    private static int TagSize
    {
        get { return _tagSize; }
    }

    public static string GenKey(string stringKey)
    {
        byte[] key = new byte[KeySize]; //256bits encryption key.

        byte[] keyBytes = Encoding.UTF8.GetBytes(stringKey);

        // Copy or truncate the bytes from stringKey into the key array
        if (keyBytes.Length >= KeySize)
        {
            Array.Copy(keyBytes, key, KeySize); // Truncate if longer
        }
        else
        {
            // If too short, copy and pad the remaining bytes with zeros
            Array.Copy(keyBytes, key, keyBytes.Length);
        }

        // Converting the key into base 64 strings for easier manipulation.
        string keyString = Convert.ToBase64String(key);

        return keyString;
    }

    public static string Encrypt(string text, string base64key, string? authenticationTag = null)
    {
        byte[] key = Convert.FromBase64String(base64key);

        byte[] nonce = new byte[NonceSize];
        byte[] tag = new byte[TagSize];

        // filling the arrays with strong random bytes.
        RandomNumberGenerator.Fill(nonce);
        RandomNumberGenerator.Fill(tag);

        byte[] cipherText = new byte[text.Length];
        byte[] encryptedText;
        using (AesGcm cipher = new(key, TagSize))
        {
            if (authenticationTag == null)
            {
                cipher.Encrypt(nonce, Encoding.UTF8.GetBytes(text), cipherText, tag, null);
            }
            else
            {
                authenticationTag = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(authenticationTag)
                );
                cipher.Encrypt(
                    nonce,
                    Encoding.UTF8.GetBytes(text),
                    cipherText,
                    tag,
                    Encoding.UTF8.GetBytes(authenticationTag)
                );
            }

            encryptedText = Concat(tag, Concat(nonce, cipherText));
        }

        return Convert.ToBase64String(encryptedText);
    }

    public static string Decrypt(
        string encryptedText,
        string base64key,
        string? authenticationTag = null
    )
    {
        byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);

        byte[] key = Convert.FromBase64String(base64key);
        byte[] tag = SubArray(encryptedTextBytes, 0, TagSize);
        byte[] nonce = SubArray(encryptedTextBytes, TagSize, NonceSize);

        byte[] cipherText = SubArray(
            encryptedTextBytes,
            TagSize + NonceSize,
            encryptedTextBytes.Length - tag.Length - nonce.Length
        );

        byte[] decryptedText = new byte[cipherText.Length];
        using (AesGcm cipher = new(key, TagSize))
        {
            if (authenticationTag == null)
            {
                cipher.Decrypt(nonce, cipherText, tag, decryptedText, null);
            }
            else
            {
                authenticationTag = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(authenticationTag)
                );
                cipher.Decrypt(
                    nonce,
                    cipherText,
                    tag,
                    decryptedText,
                    Encoding.UTF8.GetBytes(authenticationTag)
                );
            }
        }

        return Encoding.UTF8.GetString(decryptedText);
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        byte[] output = new byte[a.Length + b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            output[i] = a[i];
        }

        for (int i = 0; i < b.Length; i++)
        {
            output[a.Length + i] = b[i];
        }

        return output;
    }

    private static byte[] SubArray(byte[] data, int start, int length)
    {
        byte[] result = new byte[length];

        Array.Copy(data, start, result, 0, length);

        return result;
    }
}
