# Graph Report - .  (2026-05-10)

## Corpus Check
- Large corpus: 1123 files · ~696,537 words. Semantic extraction will be expensive (many Claude tokens). Consider running on a subfolder, or use --no-semantic to run AST-only.

## Summary
- 50 nodes · 38 edges · 11 communities detected
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 1,500 input · 500 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Apple Platforms Setup|Apple Platforms Setup]]
- [[_COMMUNITY_MAUI App Lifecycle|MAUI App Lifecycle]]
- [[_COMMUNITY_Android Application Base|Android Application Base]]
- [[_COMMUNITY_Windows Desktop Platform|Windows Desktop Platform]]
- [[_COMMUNITY_Main UI Entry (MainPage)|Main UI Entry (MainPage)]]
- [[_COMMUNITY_MauiProgram Bootstrapper|MauiProgram Bootstrapper]]
- [[_COMMUNITY_Android Activity Lifecycle|Android Activity Lifecycle]]
- [[_COMMUNITY_iOS Program Runner|iOS Program Runner]]
- [[_COMMUNITY_MacCatalyst Program Runner|MacCatalyst Program Runner]]
- [[_COMMUNITY_Icon Rendering Audit|Icon Rendering Audit]]
- [[_COMMUNITY_Notch & Safe Area Audit|Notch & Safe Area Audit]]

## God Nodes (most connected - your core abstractions)
1. `App` - 3 edges
2. `MainApplication` - 3 edges
3. `AppDelegate` - 3 edges
4. `AppDelegate` - 3 edges
5. `App` - 3 edges
6. `MainPage` - 2 edges
7. `MauiProgram` - 2 edges
8. `MainActivity` - 2 edges
9. `Program` - 2 edges
10. `Program` - 2 edges

## Surprising Connections (you probably didn't know these)
- `Captura: Iconos como texto en Feed` --references--> `Problema: Renderizado de Material Icons`  [EXTRACTED]
  raw/media__1778425161360.png → raw/media__1778424500794.png

## Communities

### Community 0 - "Apple Platforms Setup"
Cohesion: 0.22
Nodes (5): AppDelegate, GrupoXpert.Maui, AppDelegate, GrupoXpert.Maui, MauiUIApplicationDelegate

### Community 1 - "MAUI App Lifecycle"
Cohesion: 0.4
Nodes (3): Application, App, GrupoXpert.Maui

### Community 2 - "Android Application Base"
Cohesion: 0.4
Nodes (3): GrupoXpert.Maui, MainApplication, MauiApplication

### Community 3 - "Windows Desktop Platform"
Cohesion: 0.4
Nodes (3): MauiWinUIApplication, App, GrupoXpert.Maui.WinUI

### Community 4 - "Main UI Entry (MainPage)"
Cohesion: 0.5
Nodes (3): ContentPage, GrupoXpert.Maui, MainPage

### Community 5 - "MauiProgram Bootstrapper"
Cohesion: 0.5
Nodes (2): GrupoXpert.Maui, MauiProgram

### Community 6 - "Android Activity Lifecycle"
Cohesion: 0.5
Nodes (3): GrupoXpert.Maui, MainActivity, MauiAppCompatActivity

### Community 7 - "iOS Program Runner"
Cohesion: 0.5
Nodes (2): GrupoXpert.Maui, Program

### Community 8 - "MacCatalyst Program Runner"
Cohesion: 0.5
Nodes (2): GrupoXpert.Maui, Program

### Community 9 - "Icon Rendering Audit"
Cohesion: 0.67
Nodes (3): Captura: Iconos como texto en Feed, Captura: Iconos como texto en Login, Problema: Renderizado de Material Icons

### Community 10 - "Notch & Safe Area Audit"
Cohesion: 1.0
Nodes (2): Captura: Colisión con el Notch, Problema: Área Segura (Notch) en Android

## Knowledge Gaps
- **14 isolated node(s):** `GrupoXpert.Maui`, `GrupoXpert.Maui`, `GrupoXpert.Maui`, `GrupoXpert.Maui`, `GrupoXpert.Maui` (+9 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **Thin community `MauiProgram Bootstrapper`** (4 nodes): `GrupoXpert.Maui`, `MauiProgram`, `.CreateMauiApp()`, `MauiProgram.cs`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `iOS Program Runner`** (4 nodes): `GrupoXpert.Maui`, `Program`, `.Main()`, `Program.cs`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `MacCatalyst Program Runner`** (4 nodes): `GrupoXpert.Maui`, `Program`, `.Main()`, `Program.cs`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Notch & Safe Area Audit`** (2 nodes): `Captura: Colisión con el Notch`, `Problema: Área Segura (Notch) en Android`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What connects `GrupoXpert.Maui`, `GrupoXpert.Maui`, `GrupoXpert.Maui` to the rest of the system?**
  _14 weakly-connected nodes found - possible documentation gaps or missing edges._