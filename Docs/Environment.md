# QuestSlicer – Umgebung & Abhängigkeiten

## Hardware

| Komponente | Wert |
|-----------|------|
| Host-System | macOS (Apple Silicon / ARM64) |
| Zielgerät | Meta Quest 3 (Standalone, Android 14+) |

## Unity-Umgebung

| Komponente | Wert |
|-----------|------|
| Unity CLI | 1.0.0-beta.14 |
| Installierter Editor | 6000.6.7f1 (Unity 6.6.7 LTS, arm64) |
| Editor-Pfad | `/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app` |
| Lizenz | Unity Personal (Assigned), OAuth |
| Authentifizierung | Eingeloggt (OAuth, Keychain) |
| Available LTS Versions | 6000.3.27f1, 6000.0.85f1, 6000.3.26f1, ... |

## Installierte Module

| Modul | Status |
|-------|--------|
| Web | ✅ installiert |
| Android | ✅ installiert |

## Prüfbare Abhängigkeiten

```bash
# Unity CLI Version
unity --version                       # → 1.0.0-beta.14

# Installierte Editoren und Module
unity editors --installed --format json

# Doctor-Check (System-Abhängigkeiten)
unity doctor --format json

# Auth Status
unity auth status --format json

# License Status
unity license status --format json
```

## Externe Werkzeuge

| Tool | Status | Bemerkung |
|------|--------|-----------|
| Git | ✅ 2.54.0 | `osxkeychain` credential helper |
| Git LFS | ❌ nicht installiert | Braucht ggf. später für Asset-Repo |
| GitHub CLI (`gh`) | ❌ nicht installiert | Für PRs/Milestones |
| GitLab CLI (`glab`) | ❌ nicht installiert | |
| Plastic SCM (`cm`) | ❌ nicht installiert | Nur bei UVCS-Nutzung |

## Bekannte Warnungen

- `DOCTOR_CHECK_AUTH_KEYCHAIN_DENIED`: Keychain-Zugriff für Auth-Broker verweigert.
  → Keine Blockade für CLI-Befehle, da OAuth-Token bereits in Keychain gespeichert.

## Blocker

**Keine.** Android-Modul wurde erfolgreich installiert.

```bash
# Verifikation – Ausgabe:
# moduleNames: ["Android", "Web"]
unity editors --installed --format json
```

## Paket-Quellen

- **Unity Built-in Templates:** `com.unity.template.urp-blank` (Universal 3D)
- **Unity Package Registry:** Alle anderen Pakete
- **GitHub:** Kein privates Package-Repository konfiguriert

## Unity-Version-Auswahl

**Gewählt: 6000.6.7f1 (Unity 6 LTS)**

- LTS-Zweig = stabilste Wahl, ~2 Jahre Patches.
- Aktuellste LTS-Revision (6000.6.x) enthält die neuesten Bugfixes.
- URP 17.6.0 kompatibel mit Quest 3.
- OpenXR 1.17.1 mit Meta OpenXR 2.5.1 Support.

## Versions-Kompatibilität

| Package | Benötigte Unity | Aktuellste | Kompatibel |
|---------|----------------|------------|------------|
| URP | ≥ 6000.0 | 17.6.0 | ✅ (6000.6.7f1) |
| OpenXR | ≥ 6000.0 | 1.17.1 | ✅ |
| XRI | ≥ 6000.0 | 3.5.1 | ✅ |
| Meta OpenXR | ≥ 6000.0 | 2.5.1 | ✅ |
| Input System | ≥ 6000.0 | 1.20.0 | ✅ |

## Projekt-Status

| Datei/Verzeichnis | Status |
|-------------------|--------|
| `QuestSlicer/Packages/manifest.json` | ✅ URP + Input System enthalten |
| `QuestSlicer/ProjectSettings/XRSettings.asset` | ⚠️ VR noch nicht konfiguriert |
| `QuestSlicer/Assets/Scenes/SampleScene.unity` | ✅ Template-Standard-Szene |
| `.git` | ⬜ Noch nicht initialisiert |

## Nicht verifizierte Angaben

- **STL-Ladeperformance auf Quest 3:** Noch nicht gemessen.
- **LineRenderer-Performance bei großen Modellen (> 100k Dreiecke):** Noch nicht getestet.
- **Android-Modul-Größe:** Installationsgröße noch nicht geprüft.
