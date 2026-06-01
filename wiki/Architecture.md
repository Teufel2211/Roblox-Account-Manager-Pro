# Architektur

## Tech-Stack

- .NET 8
- WPF
- MVVM
- Supabase (PostgreSQL + Auth)
- Microsoft.Extensions.DependencyInjection
- Newtonsoft.Json
- System.Configuration.ConfigurationManager

## Projektstruktur

- `RobloxAccountManagerPro.Core`
  - Modelle
  - Schnittstellen
  - DTOs
  - Konstante Werte

- `RobloxAccountManagerPro.Services`
  - Supabase-Integration
  - Sicherheitsdienste
  - Prozessmanagement
  - Logging

- `RobloxAccountManagerPro.Data`
  - Lokale Cache- und Datenzugriffskomponenten

- `RobloxAccountManagerPro.UI`
  - Views
  - ViewModels
  - Ressourcen und Stile
  - WPF-Infrastruktur

## Designprinzipien

- Trennung von Oberfläche und Logik (MVVM)
- Dependency Injection für lose Kopplung
- Cloud-only Supabase-Backends
- Starke Verschlüsselung für sensible Daten
