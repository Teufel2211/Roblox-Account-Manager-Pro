# ARCHITECTURE.md

## System Architecture

### 🏗️ Overview

Roblox Account Manager Pro follows a layered, MVVM-based architecture designed for scalability, maintainability, and extensibility.

```
┌─────────────────────────────────────────────────────┐
│                  UI Layer (WPF)                      │
│  ┌──────────────┬──────────────┬─────────────────┐  │
│  │  MainWindow  │  Dashboard   │ AccountManager  │  │
│  └──────────────┴──────────────┴─────────────────┘  │
├─────────────────────────────────────────────────────┤
│              ViewModel Layer (MVVM)                  │
│  ┌──────────────┬──────────────┬─────────────────┐  │
│  │  Main VM     │  Dashboard   │  AccountManager │  │
│  │              │  ViewModel   │  ViewModel      │  │
│  └──────────────┴──────────────┴─────────────────┘  │
├─────────────────────────────────────────────────────┤
│           Business Logic / Service Layer             │
│  ┌──────────┬──────────┬──────────┬─────────────┐   │
│  │ Account  │ Process  │ Encryption
│  │ Service  │ Manager  │ Service   │ Logging    │   │
│  └──────────┴──────────┴──────────┴─────────────┘   │
├─────────────────────────────────────────────────────┤
│          Data / Integration Layer                    │
│  ┌──────────────┬──────────────────────────────┐    │
│  │ Supabase     │ Local Cache Manager          │    │
│  │ Integration  │ (JSON + Encryption)          │    │
│  └──────────────┴──────────────────────────────┘    │
├─────────────────────────────────────────────────────┤
│              Core / Domain Layer                     │
│  ┌──────────────┬──────────────┬────────────────┐   │
│  │ Models       │ Interfaces   │ Constants      │   │
│  │ (Entities)   │ (Contracts)  │ (Config)       │   │
│  └──────────────┴──────────────┴────────────────┘   │
└─────────────────────────────────────────────────────┘
```

---

## 📦 Project Structure

### **RobloxAccountManagerPro.Core**
The domain/core layer containing business entities and contracts.

```
Core/
├── Models/              # Domain entities
│   ├── RobloxAccount.cs
│   ├── ActivityLog.cs
│   ├── AppSettings.cs
│   ├── ProcessInstance.cs
│   └── DashboardStats.cs
├── Interfaces/          # Service contracts
│   ├── IAccountService.cs
│   ├── IProcessManagerService.cs
│   ├── IEncryptionService.cs
│   ├── ISupabaseService.cs
│   └── ILoggingService.cs
├── DTOs/               # Data transfer objects
│   └── CreateAccountRequest.cs
└── Constants/          # Application constants
    └── AppConstants.cs
```

**Responsibilities:**
- Define business entities
- Establish service contracts
- Maintain application constants
- Zero external dependencies

---

### **RobloxAccountManagerPro.Services**
The business logic layer implementing service patterns.

```
Services/
├── AccountService.cs           # Account CRUD operations
├── Supabase/
│   └── SupabaseService.cs      # Cloud integration
├── Security/
│   └── EncryptionService.cs    # AES-256 encryption
├── Process/
│   └── ProcessManagerService.cs # Roblox process control
└── Logging/
    └── LoggingService.cs       # Circular buffer logging
```

**Responsibilities:**
- Implement business logic
- Coordinate between data and UI layers
- Handle encryption/security
- Manage external integrations
- Provide async operations

---

### **RobloxAccountManagerPro.Data**
The data persistence layer for offline support.

```
Data/
└── LocalCacheManager.cs  # JSON-based local cache with encryption
```

**Responsibilities:**
- Manage local data persistence
- Cache synchronization
- Offline support
- Export/import functionality

---

### **RobloxAccountManagerPro.UI**
The presentation layer with WPF and MVVM.

```
UI/
├── Views/              # XAML window definitions
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   └── ...
├── ViewModels/         # MVVM view models
│   ├── MainWindowViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── AccountManagerViewModel.cs
│   └── ...
├── Resources/          # Styles and themes
│   ├── Colors.xaml
│   ├── Styles/
│   │   ├── ButtonStyles.xaml
│   │   └── TextBlockStyles.xaml
│   └── Themes/
├── Infrastructure/     # MVVM helpers
│   ├── RelayCommand.cs
│   ├── AsyncRelayCommand.cs
│   └── ViewModelBase.cs
├── App.xaml            # Application root
├── App.xaml.cs
└── Program.cs          # DI configuration and entry point
```

**Responsibilities:**
- Render user interface
- Handle user input
- Display data through ViewModels
- Maintain WPF-specific concerns

---

## 🔄 Data Flow

### Account Creation Flow

```
User Input (UI)
    ↓
[MainWindow] → AddAccountCommand (RelayCommand)
    ↓
[AccountManagerViewModel] → CreateAccountAsync()
    ↓
[AccountService] → CreateAccountAsync()
    ↓
[SupabaseService] → InsertAccountAsync()
    ↓
[LocalCacheManager] → SaveAccountsAsync()
    ↓
[UI Updates] → ObservableCollection refresh
```

### Process Management Flow

```
User Clicks "Start All"
    ↓
[DashboardViewModel] → StartAllInstancesAsync()
    ↓
[AccountService] → GetAllAccountsAsync()
    ↓
[ProcessManagerService] → LaunchRobloxAsync() × N
    ↓
[System.Diagnostics.Process] → Start Roblox
    ↓
[Metrics Monitor] → RefreshProcessMetricsAsync() (Timer)
    ↓
[UI Updates] → ActiveInstances collection refresh
```

---

## 🔐 Security Architecture

### Encryption Strategy

1. **Password Encryption**
   - Algorithm: AES-256 in CBC mode
   - Key Derivation: PBKDF2 (10,000 iterations)
   - Storage: Windows DPAPI protected key file

2. **Key Management**
   - Master Key: Generated on first run, stored encrypted
   - Per-Database: Derived from master key + salt
   - Memory: Cleared after use via `SecureWipeMemory()`

3. **Authentication**
   - Master Password: PBKDF2-SHA256 hashed (10,000 iterations)
   - Sessions: In-memory tracking with auto-lock
   - Windows Hello: Integrated for biometric unlock

### Data Protection

```
Plain Password Input
    ↓
Derivation (PBKDF2)
    ↓
AES-256 Encryption
    ↓
Base64 Encoding
    ↓
Local Storage (Encrypted with Windows DPAPI)
    ↓
Never sent to cloud (except encrypted blob if enabled)
```

---

## 🔌 Dependency Injection

### Service Registration (Program.cs)

```csharp
var services = new ServiceCollection();

// Singleton services
services.AddSingleton<ILoggingService, LoggingService>();
services.AddSingleton<IEncryptionService, EncryptionService>();
services.AddSingleton<LocalCacheManager>();

// HTTP client
services.AddHttpClient<ISupabaseService, SupabaseService>();

// ViewModels
services.AddSingleton<MainWindowViewModel>();
services.AddSingleton<DashboardViewModel>();
```

### Benefits
- Loose coupling between layers
- Easy testing with mock implementations
- Centralized configuration
- Lifetime management

---

## 🔄 Async/Await Pattern

All I/O operations are fully asynchronous:

```csharp
// Services provide async methods
Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync();

// ViewModels await results
var accounts = await _accountService.GetAllAccountsAsync();

// UI updates on main thread via Task.FromResult or ConfigureAwait
```

---

## 🎨 MVVM Implementation

### Base ViewModel

```csharp
public class ViewModelBase : INotifyPropertyChanged
{
    protected void OnPropertyChanged(string propertyName);
    protected bool SetProperty<T>(ref T field, T value, string propertyName);
}
```

### Command Pattern

- **RelayCommand**: Synchronous operations
- **AsyncRelayCommand**: Async operations with execution prevention

### Data Binding

```xaml
<TextBlock Text="{Binding TotalAccounts}"/>
<Button Command="{Binding AddAccountCommand}"/>
<ListBox ItemsSource="{Binding Accounts}"/>
```

---

## 📊 Performance Considerations

### Process Monitoring
- Background timer every 1000ms (configurable)
- Lightweight WMI queries
- In-memory caching of active processes

### Memory Management
- ObservableCollections for UI updates
- Lazy loading for large account lists
- Circular buffer (max 1000) for logs

### Network
- Supabase connection pooling
- Offline-first design with local cache
- Retry queue for failed operations

---

## 🧪 Testing Strategy

### Unit Tests
- Mock IAccountService, IProcessManagerService
- Test ViewModel logic independently
- Verify encryption/decryption

### Integration Tests
- Test SupabaseService with test database
- Verify LocalCacheManager persistence
- Process manager simulation

### UI Tests
- WPF test framework or manual validation
- Command execution verification
- Data binding validation

---

## 🚀 Deployment Architecture

```
Development
    ↓
GitHub (main branch)
    ↓
GitHub Actions (Build + Test)
    ↓
Release Tag
    ↓
Artifacts (.exe, portable)
    ↓
GitHub Releases
```

---

## 🔄 Extensibility

### Adding a New Service

1. Define interface in Core/Interfaces
2. Implement in Services/
3. Register in Program.cs
4. Inject into ViewModels

### Adding a New View

1. Create XAML in Views/
2. Create ViewModel in ViewModels/
3. Register in Program.cs
4. Bind in App.xaml resources

---

## 📈 Scalability

### Current Limits
- Max 1000 logged events (circular)
- ~100 active accounts (tested)
- ~10 simultaneous instances (Roblox limit)

### Future Optimizations
- Database connection pooling
- Paging for large account lists
- Background sync service
- Plugin system for extensions

---

## 🎓 Design Patterns Used

1. **MVVM**: Separation of concerns
2. **Dependency Injection**: Loose coupling
3. **Repository Pattern**: Data abstraction
4. **Singleton**: Shared services
5. **Observer Pattern**: INotifyPropertyChanged
6. **Command Pattern**: User actions
7. **Factory Pattern**: Object creation
8. **Adapter Pattern**: Supabase integration

---

**Document Version:** 2.0.0  
**Last Updated:** January 2024  
**Maintainers:** RobloxAMP Contributors
