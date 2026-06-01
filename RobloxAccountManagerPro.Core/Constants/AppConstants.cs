namespace RobloxAccountManagerPro.Core.Constants;

/// <summary>
/// Application-wide constants.
/// </summary>
public static class AppConstants
{
    public const string ApplicationName = "Roblox Account Manager Pro";
    public const string ApplicationVersion = "2.0.0";
    public const string CompanyName = "RobloxAMP";
    
    // Theme
    public const string DarkTheme = "Dark";
    public const string LightTheme = "Light";
    
    // Categories
    public const string CategoryMain = "Main";
    public const string CategoryAlt = "Alt";
    public const string CategoryDev = "Dev";
    public const string CategoryVip = "VIP";
    
    // Tags
    public const string TagVip = "VIP";
    public const string TagMain = "Main";
    public const string TagAlt = "Alt";
    public const string TagDev = "Dev";
    
    // Process Management
    public const string RobloxProcessName = "RobloxPlayerBeta";
    public const int ProcessMetricsUpdateIntervalMs = 1000;
    
    // Security
    public const int AesKeySize = 256;
    public const int PasswordHashIterations = 10000;
}
