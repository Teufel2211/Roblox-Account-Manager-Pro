# Release

## Release-Workflow

Das Projekt verwendet GitHub Actions zur Erstellung und Veröffentlichung von Releases.

### Veröffentlichter Release-Workflow

- Bei einem `release`-Event vom Typ `published` wird automatisch:
  - die Lösung gebaut
  - `RobloxAccountManagerPro.UI` als `win-x64` Self-contained Single File veröffentlicht
  - `RobloxAccountManagerPro.exe` als Release-Asset hochgeladen

### Release-Asset

Die veröffentlichte Datei heißt:

- `RobloxAccountManagerPro.exe`

und wird direkt an die GitHub Release-Seite angehängt.

### Release erstellen

1. Erstelle eine neue Release im GitHub-Repository
2. Setze den Release-Status auf **Published**
3. Nachdem die Action durchgelaufen ist, findest du das Asset unter der Release-Seite

## Fehlerbehebung

Wenn der Schritt `Upload release asset` übersprungen wird, liegt das meist daran, dass der Release nicht als `published` angelegt wurde oder der Workflow auf einem Tag-Push statt einem GitHub Release-Event lief.
