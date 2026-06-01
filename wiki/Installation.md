# Installation

## Voraussetzungen

- Windows 10 oder Windows 11
- .NET 8 Runtime oder SDK
- Git
- Optional: Supabase-Account für Cloud-Backup

## Repository klonen

```bash
git clone https://github.com/yourusername/RobloxAccountManagerPro.git
cd "Roblox Account Manager Pro"
```

## Pakete wiederherstellen

```bash
dotnet restore
```

## Projekt öffnen

```bash
dotnet sln RobloxAccountManagerPro.sln
```

## Supabase konfigurieren

1. Erstelle ein Projekt auf https://supabase.com
2. Sammle Project URL und API-Key
3. Trage die Werte in `RobloxAccountManagerPro.UI/App.config` ein:

```xml
<add key="Supabase:Url" value="https://your-project.supabase.co" />
<add key="Supabase:ApiKey" value="sb_publishable_your_key" />
```

## Anwendung ausführen

```bash
dotnet build
dotnet run --project RobloxAccountManagerPro.UI
```
