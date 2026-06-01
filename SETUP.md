# SETUP.md

## 🚀 Complete Setup Guide

### Prerequisites

- **OS**: Windows 10/11
- **.NET**: SDK 8.0 or later
- **Git**: For version control
- **Visual Studio**: 2022 or later (optional but recommended)
- **Supabase Account**: Free tier available at https://supabase.com

---

## 📋 Step-by-Step Installation

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/RobloxAccountManagerPro.git
cd RobloxAccountManagerPro
```

### 2. Install Dependencies

```bash
# Restore NuGet packages
dotnet restore

# Check .NET version
dotnet --version
```

### 3. Supabase Configuration

#### 3.1 Create Supabase Project
1. Go to https://supabase.com
2. Sign up or log in
3. Create new project
4. Note your Project URL and API Key

#### 3.2 Create Database Tables

In Supabase SQL Editor, run:

```sql
-- Accounts Table
CREATE TABLE accounts (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  username TEXT NOT NULL UNIQUE,
  display_name TEXT NOT NULL,
  notes TEXT,
  category TEXT NOT NULL,
  avatar_url TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  is_favorite BOOLEAN DEFAULT FALSE,
  tags JSONB DEFAULT '[]',
  last_used TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE,
  launch_count INT DEFAULT 0
);

-- Activity Logs Table
CREATE TABLE activity_logs (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
  action TEXT NOT NULL,
  status TEXT NOT NULL,
  timestamp TIMESTAMP DEFAULT NOW(),
  details TEXT,
  process_id INT,
  memory_usage_mb INT
);

-- Settings Table
CREATE TABLE settings (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  master_password_hash TEXT,
  auto_lock_enabled BOOLEAN DEFAULT TRUE,
  auto_lock_minutes INT DEFAULT 15,
  theme TEXT DEFAULT 'Dark',
  created_at TIMESTAMP DEFAULT NOW(),
  last_modified TIMESTAMP,
  windows_hello_enabled BOOLEAN DEFAULT FALSE,
  online_backup_enabled BOOLEAN DEFAULT TRUE
);

-- Enable Row Level Security (Optional but recommended)
ALTER TABLE accounts ENABLE ROW LEVEL SECURITY;
ALTER TABLE activity_logs ENABLE ROW LEVEL SECURITY;
ALTER TABLE settings ENABLE ROW LEVEL SECURITY;
```

#### 3.3 Configure App

Update `App.config` in RobloxAccountManagerPro.UI:

```xml
<appSettings>
  <add key="Supabase:Url" value="https://your-project.supabase.co" />
  <add key="Supabase:ApiKey" value="your-anon-key-here" />
</appSettings>
```

### 4. Build Project

```bash
# Debug build
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release
```

### 5. Run Application

```bash
# Run directly
dotnet run --project RobloxAccountManagerPro.UI/RobloxAccountManagerPro.UI.csproj

# Or open in Visual Studio and press F5
```

---

## 🔧 Advanced Configuration

### Local Development

#### Using Visual Studio 2022

1. Open `RobloxAccountManagerPro.sln`
2. Set `RobloxAccountManagerPro.UI` as startup project
3. Press `F5` to debug

#### Using Visual Studio Code

1. Install C# extension
2. Open folder in VS Code
3. Run command: `.NET: Debug Project` (Ctrl+F5)

### Environment Variables

Create `.env` file in project root:

```env
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-api-key
APP_THEME=Dark
LOG_LEVEL=Information
```

### Local Data Storage

Data is stored in:
```
%LocalAppData%\RobloxAMP\
├── Cache\
│   ├── accounts.json
│   ├── settings.json
│   └── activity_logs.json
└── .key (encrypted master key)
```

---

## 🧪 Testing

### Run Unit Tests

```bash
dotnet test --configuration Release
```

### Run Specific Test

```bash
dotnet test --filter "EncryptionService"
```

---

## 🔐 Security Setup

### Master Password

On first launch:
1. Set a strong master password (minimum 12 characters)
2. Confirm the password
3. Password is hashed with PBKDF2-SHA256

### Windows Hello

1. Go to Settings
2. Enable "Use Windows Hello"
3. Authenticate with fingerprint or PIN

### Auto-lock

1. Go to Settings → Security
2. Enable "Auto-lock on inactivity"
3. Set timeout (default 15 minutes)

---

## 📦 Build Release

### Create Release Build

```bash
dotnet publish RobloxAccountManagerPro.UI/RobloxAccountManagerPro.UI.csproj \
  -c Release \
  -o ./publish \
  -p:PublishSingleFile=true \
  -p:SelfContained=false
```

### Create Portable Executable

```bash
dotnet publish RobloxAccountManagerPro.UI/RobloxAccountManagerPro.UI.csproj \
  -c Release \
  -o ./publish-portable \
  -p:PublishSingleFile=true \
  -p:SelfContained=true \
  -p:RuntimeIdentifier=win-x64
```

### Output Location
- Single-file EXE: `./publish/RobloxAccountManagerPro.exe`
- Portable: `./publish-portable/RobloxAccountManagerPro.exe`

---

## 🐛 Troubleshooting

### "Cannot connect to Supabase"

**Solution:**
- Check internet connection
- Verify Supabase URL and API key
- Check if Supabase project is active
- Review firewall/antivirus settings

### "Roblox doesn't launch"

**Solution:**
- Ensure Roblox is installed
- Check if `RobloxPlayerLauncher.exe` exists
- Try manual Roblox launch
- Check application logs

### ".NET SDK not found"

**Solution:**
```bash
# Install .NET 8
winget install Microsoft.DotNet.SDK.8

# Or download from
# https://dotnet.microsoft.com/en-us/download/dotnet/8.0
```

### "App crashes on startup"

**Solution:**
- Check Windows Event Viewer for errors
- Run with `--debug` flag for detailed output
- Delete cache: `%LocalAppData%\RobloxAMP\Cache`

---

## 🔄 Development Workflow

### 1. Create Feature Branch

```bash
git checkout -b feature/my-feature
```

### 2. Make Changes

- Modify code in appropriate layer
- Follow C# style guide
- Add XML documentation

### 3. Test Changes

```bash
dotnet build
dotnet test
```

### 4. Commit Changes

```bash
git add .
git commit -m "Add my feature"
```

### 5. Push and Create PR

```bash
git push origin feature/my-feature
# Open PR on GitHub
```

---

## 📚 Additional Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Supabase Documentation](https://supabase.com/docs)
- [MVVM Pattern Guide](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)

---

## ✅ Verification Checklist

After setup, verify:

- [ ] Solution builds successfully
- [ ] All projects compile without errors
- [ ] Tests pass (if any)
- [ ] Application runs without crashes
- [ ] Supabase connection works
- [ ] Can add/edit/delete accounts
- [ ] Can launch Roblox instances
- [ ] Dashboard displays statistics
- [ ] Logs are being recorded

---

## 🆘 Getting Help

- Open an issue on GitHub
- Check existing documentation
- Review troubleshooting guide
- Contact maintainers

---

**Setup Version:** 2.0.0  
**Last Updated:** January 2024
