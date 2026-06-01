# Roblox Account Manager Pro

**Manage. Launch. Secure.**

A professional, open-source Windows desktop application for managing multiple Roblox accounts with cloud backup, security features, and process management.

## 🌟 Features

### Account Management
- 👥 Add, edit, and delete Roblox accounts
- 🏷️ Organize with categories (Main, Alt, Dev, VIP)
- ⭐ Mark favorite accounts
- 🔖 Tag system for flexible organization
- 🔍 Real-time search and filtering
- 📝 Notes and metadata for each account

### Multi-Launch System
- 🚀 Launch multiple Roblox instances simultaneously
- 📊 Monitor active instances with live metrics
- 💾 Process tracking (PID, memory usage, runtime)
- ⏹️ One-click stop all functionality
- 🔄 Automatic process cleanup

### Dashboard
- 📈 Real-time statistics and metrics
- 📋 Recent accounts overview
- 🎯 Quick action buttons
- 📊 Memory usage monitoring
- 🔌 Connection status indicator

### Security
- 🔐 AES-256 encryption for passwords
- 🔑 Master password protection
- 💻 Windows Hello support
- 🛡️ Auto-lock on inactivity
- 🔒 Windows DPAPI key storage

### Cloud Integration
- ☁️ Supabase backend integration
- 🔄 Automatic sync and backup
- 📡 Offline mode with local caching
- 📊 Activity logging and audit trail

### UI/UX
- 🎨 Modern glass morphism design
- 🌙 Dark mode (light mode coming soon)
- ⚡ Smooth animations
- 📱 Responsive layout
- 💫 Material Design 3 inspired

## 🛠️ Tech Stack

- **Framework**: .NET 8 (Latest LTS)
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Database**: Supabase (PostgreSQL + Auth)
- **Security**: AES-256, PBKDF2, Windows DPAPI
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Async**: Full async/await pattern

## 📦 Project Structure

```
RobloxAccountManagerPro/
├── RobloxAccountManagerPro.Core/
│   ├── Models/           # Data models
│   ├── Interfaces/       # Service contracts
│   ├── DTOs/            # Data transfer objects
│   └── Constants/       # Application constants
├── RobloxAccountManagerPro.Services/
│   ├── Supabase/        # Cloud integration
│   ├── Security/        # Encryption services
│   ├── Process/         # Process management
│   └── Logging/         # Logging services
├── RobloxAccountManagerPro.Data/
│   └── LocalCacheManager.cs
├── RobloxAccountManagerPro.UI/
│   ├── Views/           # XAML views
│   ├── ViewModels/      # MVVM view models
│   ├── Resources/       # Styles and themes
│   └── Infrastructure/  # Commands and helpers
└── .github/workflows/   # CI/CD pipelines
```

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- .NET 8 Runtime
- Supabase Account (free tier available)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/RobloxAccountManagerPro.git
cd RobloxAccountManagerPro
```

2. **Open the solution**
```bash
cd RobloxAccountManagerPro
dotnet sln RobloxAccountManagerPro.sln
```

3. **Restore packages**
```bash
dotnet restore
```

4. **Configure Supabase** (Optional)
- Create a Supabase project at [supabase.com](https://supabase.com)
- Get your project URL and API key
- Configure in application settings

5. **Build and Run**
```bash
dotnet build
dotnet run --project RobloxAccountManagerPro.UI
```

## 🔧 Configuration

### Supabase Setup

Create the following tables in your Supabase project:

**accounts**
```sql
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
```

**activity_logs**
```sql
CREATE TABLE activity_logs (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  account_id UUID NOT NULL REFERENCES accounts(id),
  action TEXT NOT NULL,
  status TEXT NOT NULL,
  timestamp TIMESTAMP DEFAULT NOW(),
  details TEXT,
  process_id INT,
  memory_usage_mb INT
);
```

**settings**
```sql
CREATE TABLE settings (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  master_password_hash TEXT,
  auto_lock_enabled BOOLEAN DEFAULT TRUE,
  theme TEXT DEFAULT 'Dark',
  created_at TIMESTAMP DEFAULT NOW(),
  last_modified TIMESTAMP
);
```

## 📖 Usage

### Adding an Account
1. Click "➕ Add Account" in the Accounts tab
2. Enter account details (Username, Display Name, Category)
3. Optionally set a password (encrypted locally)
4. Add tags for organization
5. Click "Save"

### Launching Accounts
1. Go to the Dashboard tab
2. Click "▶ Start All" to launch all accounts
3. Or select "⭐ Start Favorites" for favorite accounts
4. Monitor instances in the Dashboard

### Managing Instances
- View active instances with memory usage
- Right-click on an instance to terminate
- Click "⏹ Stop All" to close all instances
- Auto-cleanup of closed processes

## 🔐 Security Best Practices

1. **Master Password**: Set a strong master password on first launch
2. **Auto-lock**: Enable auto-lock (default 15 minutes)
3. **Windows Hello**: Use biometric authentication when available
4. **Local Encryption**: Passwords stored as AES-256 encrypted
5. **No Cloud Passwords**: Passwords never sent to cloud storage
6. **Audit Logs**: All actions logged locally for review

## 🌐 API Integration

The app communicates with Supabase REST API:

```csharp
// Example: Get all accounts
var response = await supabaseService.GetAllAccountsAsync();

// Example: Create account
var account = new RobloxAccount { /* ... */ };
await accountService.CreateAccountAsync(account);

// Example: Launch instance
var instance = await processManager.LaunchRobloxAsync(account);
```

## 🐛 Troubleshooting

### Can't connect to Supabase
- Check internet connection
- Verify Supabase URL and API key
- Check firewall settings

### Roblox doesn't launch
- Ensure Roblox is installed
- Check if RobloxPlayerLauncher.exe exists
- Review logs for details

### High memory usage
- Close unused instances
- Reduce Roblox graphics settings
- Monitor in Dashboard

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

For major changes, please open an issue first to discuss proposed changes.

### Code Style
- Follow C# naming conventions (PascalCase for public members)
- Use meaningful variable names
- Add XML documentation for public APIs
- Keep methods focused and single-responsibility
- Use async/await for I/O operations

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📧 Support

For support, please:
- Open an issue on GitHub
- Check existing documentation
- Review the troubleshooting guide

## 🎯 Roadmap

- [ ] Light mode theme
- [ ] Avatar display system
- [ ] Activity log viewer
- [ ] Advanced process monitoring
- [ ] Custom themes support
- [ ] Multi-language support (i18n)
- [ ] Performance profiling tools
- [ ] Portable version (.exe)

## ⭐ Acknowledgments

- Supabase for backend infrastructure
- Material Design for design inspiration
- Microsoft .NET team for .NET 8
- Community contributors

---

**Made with ❤️ for the Roblox community**

Manage. Launch. Secure.
