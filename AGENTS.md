# QuestSlicer – Architekturregeln

## 1. Grundlagen

- **Zielplattform:** Meta Quest 3 (Standalone, Android-basiert).
- **Render-Pipeline:** URP (Universal Render Pipeline) — optimiert für mobile VR.
- **VR-Framework:** XR Interaction Toolkit (XRI) + OpenXR.
- **Eingabe:** Oculus Touch Controller (first), Handtracking und Passthrough (später).
- **Dateiformate:** STL zuerst, 3MF später. STEP/IGES werden **extern** konvertiert.
- **Einheit:** Alle Druckdaten in **Millimetern**. VR-Ansichtsmaßstab separat skalierbar.
- **Kein G-Code:** Die App erzeugt **keinen** G-Code und keine Druckpfadplanung.

## 2. Projektstruktur

```
Slicer/
├── QuestSlicer/           # Unity-Projekt (Root)
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── Core/      # Projektkern, Slicer-Engine
│   │   │   ├── IO/        # Dateilader (STL, später 3MF)
│   │   │   ├── View/      # VR-Interaction, Manipulatoren
│   │   │   ├── UI/        # Overlay-UI (XR Canvas)
│   │   │   └── Tests/     # EditMode + PlayMode Tests
│   │   ├── Models/        # Beispiel-STLs
│   │   ├── Materials/
│   │   ├── Shaders/
│   │   ├── Prefabs/
│   │   ├── Scenes/
│   │   │   ├── Main.unity # Haupt-Szene
│   │   │   └── Test/      # isolierte Testszenen
│   │   ├── Settings/
│   │   │   └── URP-Asset.asset  # URP-Konfiguration
│   │   └── XR/            # XR-Package-Manager-Einstellungen
│   ├── Packages/
│   │   ├── manifest.json
│   │   └── packages-lock.json
│   └── ProjectSettings/
├── Docs/
│   ├── Architecture.md
│   ├── DevelopmentPlan.md
│   └── Environment.md
└── AGENTS.md
```

## 3. Architektur-Prinzipien

### 3.1 Trennung von Slicer-Engine und Rendering

- **SlicerCore** (`Assets/Scripts/Core/`) enthält **keine** Unity-Abhängigkeiten (keine `UnityEngine`-Imports außer `Vector3` für Datenstrukturen).
- Schichtberechnung erfolgt reines C# — testbar im EditMode ohne Unity.
- **SliceRenderer** übernimmt die Übersetzung der Konturen in sichtbare 3D-Geometrie (Mesh, Lines, ColoredSlices).

### 3.2 STL-Import

- `STLLoader` liht binäres und ASCII- `.stl` ein.
- Import läuft asynchron auf einem Thread (Job System oder `Task.Run` mit `NativeArray`).
- Ergebnis: flache Liste von Dreiecken (Vertices + Normals) im **mm**-Koordinatensystem.
- **Keine** Topologie-Validierung im ersten Durchlauf — Fehler werden geloggt und ignoriert.

### 3.3 Slicer-Engine

```
SliceResult
├── LayerHeight (float, mm)
├── Layers[]
│   ├── OuterContour[]            # List<Vector3>
│   └── InfillContours[][]        # Optional
```

- Algorithmus: Planschnitt (Plane-Cast) pro Schicht.
- Erstellt Linien- oder Polygon-Meshes aus den Konturen.
- Ausgabe: `SliceResult` — serialisierbar (JSON / Binary).

### 3.4 VR-Interaction

- **XR Interaction Toolkit 3.x** als Basis.
- **TransformManipulator** (oder eigener): Skalieren, Rotation, Translation pro Objekt.
- Gating: Objekte können nur **verschoben**, **rotiert** und **gleichmäßig/uniform skaliert** werden.
- Anchor-System: Mehrere Modelle in einer Szene verwaltbar.

### 3.5 Rendering der Schichten

- **Contour Lines:** `LineRenderer` pro Kontur, einfärbig nach Schicht-ID.
- **Solid Layers:** Dünne Mesh-Ebenen mit halbtransparentem Material.
- Wechsel zwischen Line- und Solid-Ansicht per UI-Toggle.
- Schicht-individuelle Färbung über Farbverlauf (z. B. Regenbogen oder monochrom).

## 4. Technische Regeln

### 4.1 Package-Management

- Alle Dependencies über **Unity Package Manager** (`Packages/manifest.json`).
- Keine handeditierten `.meta`-Dateien außer bei Custom Shaders/Materials.
- XR-Packages: `com.unity.xr.openxr`, `com.unity.xr.interaction.toolkit`,
  `com.unity.xr.meta-openxr`, `com.unity.xr.core-utils`.

### 4.2 Code-Qualität

- Jede `MonoBehaviour` in **einer** `.cs`-Datei (Names match).
- Scripting Define Symbols: `QUESTSLICER_DEBUG` für Dev-Logging.
- `[RequireComponent]` auf allen Game-Object-Komponenten.
- Unity-Version: **6000.x** (Unity 6 LTS) — keine API-Nutzung jenseits 6.0.

### 4.3 Build-Konfiguration

- **Target SDK:** Android (Quest 3 = ARM64).
- **Minimum API Level:** Android 8.0 (API 26).
- **Target API Level:** Latest installed.
- **Architecture:** ARM64.
- **Scripting Backend:** IL2CPP.
- **Runtime Version:** 6.0.6.7f1 (aktuell installed).

### 4.4 Tests

- **EditMode Tests:** `SliceCoreTest` (reine Mathematik-Logik, keine Unity).
- **PlayMode Tests:** `SliceIntegrationTest` (STL-Import → Slicing → Rendering).
- Test-Framework: `com.unity.test-framework`.

## 5. Nicht-Ziele (Out of Scope)

- G-Code-Generierung
- Druckpfadplanung
- Slicer-Parameter-Optimierung (Overhang, Bridging, Support)
- Multi-Drucker-Unterstützung
- Cloud/Server-Kommunikation
- 3D-Druck-Simulation (Geschwindigkeit, Materialverbrauch)

## 6. Prüfbare Build-Befehle

```bash
# Unity CLI Version
unity --version

# Installierte Editoren prüfen
unity editors --installed --format json

# Doctor-Check
unity doctor --format json

# Projekt-Info (nach Projekt-Erstellung)
unity projects info /path/to/project --format json

# Template-Verfügbarkeit prüfen
unity templates list --editor 6000.6.7f1 --type core --format json
```

## 7. Dokumentations-Schreibweise

- Alle Pfade relativ zu `Assets/`.
- Versionen im Format `x.y.zf1`.
- Package-IDs als `com.unity.*` oder `com.quickslicer.*`.
- Entscheidungen mit **Begründung** und **Alternative** dokumentieren.
