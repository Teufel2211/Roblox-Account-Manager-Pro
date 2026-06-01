namespace RobloxAccountManagerPro.Services.Security;

using System.Security.Cryptography;
using System.Text;
using RobloxAccountManagerPro.Core.Constants;
using RobloxAccountManagerPro.Core.Interfaces;

/// <summary>
/// Provides AES-256 encryption and security operations using Windows DPAPI.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private static readonly byte[] _encryptionKey = GetOrCreateEncryptionKey();

    public string EncryptPassword(string plaintext)
    {
        try
        {
            using var aes = Aes.Create();
            aes.KeySize = AppConstants.AesKeySize;
            aes.GenerateIV();

            var key = DeriveKey(plaintext);
            aes.Key = key;

            using var encryptor = aes.CreateEncryptor();
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var encrypted = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

            var result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }
        catch
        {
            throw new InvalidOperationException("Encryption failed");
        }
    }

    public string DecryptPassword(string encrypted)
    {
        try
        {
            var data = Convert.FromBase64String(encrypted);
            using var aes = Aes.Create();
            aes.KeySize = AppConstants.AesKeySize;

            var iv = new byte[aes.IV.Length];
            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var key = DeriveKey(encrypted);
            aes.Key = key;

            using var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(data, iv.Length, data.Length - iv.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch
        {
            throw new InvalidOperationException("Decryption failed");
        }
    }

    public string HashPassword(string password)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, 16, AppConstants.PasswordHashIterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(20);
        var salt = pbkdf2.Salt;

        var hashWithSalt = new byte[36];
        Buffer.BlockCopy(salt, 0, hashWithSalt, 0, 16);
        Buffer.BlockCopy(hash, 0, hashWithSalt, 16, 20);

        return Convert.ToBase64String(hashWithSalt);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);
        var salt = new byte[16];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, AppConstants.PasswordHashIterations, HashAlgorithmName.SHA256);
        var hash2 = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash2[i])
                return false;

        return true;
    }

    public string GenerateSecureToken(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var tokenData = new byte[length];
        rng.GetBytes(tokenData);
        return Convert.ToBase64String(tokenData);
    }

    public void SecureWipeMemory(byte[] data)
    {
        if (data == null) return;
        for (int i = 0; i < data.Length; i++)
            data[i] = 0;
    }

    private static byte[] DeriveKey(string password)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, _encryptionKey, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }

    private static byte[] GetOrCreateEncryptionKey()
    {
        var keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "RobloxAMP", ".key");
        
        if (File.Exists(keyPath))
        {
            var protectedKey = File.ReadAllBytes(keyPath);
            return ProtectedData.Unprotect(protectedKey, null, DataProtectionScope.CurrentUser);
        }

        var key = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        Directory.CreateDirectory(Path.GetDirectoryName(keyPath)!);
        var protectedData = ProtectedData.Protect(key, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(keyPath, protectedData);

        return key;
    }
}
