namespace RobloxAccountManagerPro.Core.Interfaces;

using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Interface for encryption and security operations.
/// </summary>
public interface IEncryptionService
{
    string EncryptPassword(string plaintext);
    string DecryptPassword(string encrypted);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateSecureToken(int length = 32);
    void SecureWipeMemory(byte[] data);
}
