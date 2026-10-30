# QuestSlicer – Entwicklungsplan

## Meilensteine

Jeder Meilenstein enthält **prüfbare** Akzeptanzkriterien.
Kein Meilenstein beginnt, bevor der vorherige abgeschlossen ist.

---

### M0: Umgebung & Projekt-Setup

**Ziel:** Lauffähiges Unity-Projekt mit Quest 3 Build-Konfiguration.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M0.1 | Android-Modul für Unity 6.0.6.7f1 installieren | ✅ `moduleNames` enthält `Android` |
| M0.2 | Projekt mit `com.unity.template.urp-blank` erstellen | ✅ `Assets/`, `Packages/`, `ProjectSettings/` vorhanden |
| M0.3 | XR-Packages hinzufügen (OpenXR, XRI, Meta OpenXR) | ⬜ nach M0 abgeschlossen |
| M0.4 | Build-Target auf Quest 3 (Android ARM64) setzen | ⬜ nach M0 abgeschlossen |
| M0.5 | Git-Repository initialisieren + `.gitignore` | ⬜ nach M0 abgeschlossen |

**Status:** M0.1 ✅ abgeschlossen, M0.2 ✅ abgeschlossen.
M0.3–M0.5 folgen in der Implementierungsphase.

---

### M1: STL-Import

**Ziel:** STL-Dateien laden und als 3D-Objekt in Unity darstellen.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M1.1 | `STLLoader` implementiert (binär + ASCII) | EditMode-Test lädt `cube.stl` → 12 Triangles |
| M1.2 | `TriangulatedMesh` Struktur definiert | `Vertices`, `Indices`, `BoundsMin/Max` populated |
| M1.3 | File-Browser / Load-Action im VR-Input | Controller-Trigger → Load-Panel öffnet sich |
| M1.4 | STL als MeshRenderer-GameObject in Szene | GameObject mit korrektem Mesh und Material sichtbar |
| M1.5 | Einheiten-Konvention dokumentiert | Alle Modelle in mm, Unity-Meter-Adapter aktiv |

---

### M2: Slicer-Engine (Kern)

**Ziel:** Reines C# Slicing-System, testbar ohne Unity.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M2.1 | `SliceCalculator` implementiert (Plane-Cast) | 20×20×10 mm Würfel, 0.2 mm Schicht → 50 Schichten |
| M2.2 | `SliceResult` Datenstruktur definiert | `Layers[]` enthält je eine äußere Kontur |
| M2.3 | EditMode-Tests für Basisgeometrie | 3 Tests: Würfel, Zylinder, leeres Mesh |
| M2.4 | Slicing auf Quest 3 performant (< 2s für 100 Schichten) | Timer in PlayMode < 2s |

---

### M3: Schicht-Visualisierung

**Ziel:** Schichtkonturen in VR sichtbar machen.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M3.1 | `SliceRenderer.LineMode` – LineRenderer pro Kontur | Jede Schicht als Linie mit Farbverlauf |
| M3.2 | `SliceRenderer.SolidMode` – Mesh-Ebenen | Toggle in UI aktivierbar |
| M3.3 | Schicht-Highlighting (aktive Schicht markiert) | Aktive Schicht hervorgehoben |
| M3.4 | Schicht-individuelle Färbung | Farbverlauf von Basis zu nozzle-höhe |

---

### M4: VR-Interaktion

**Ziel:** Modelle im VR-Raum positionieren, drehen, skalieren.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M4.1 | XRI Pickable an Modell-Root | Controller kann Modell greifen |
| M4.2 | Translation via Left Grip | Modell folgt Left-Controller |
| M4.3 | Rotation via Right Trigger | Modell rotiert um Y-Achse mit Trigger |
| M4.4 | Scale via Pinch (Index+Thumb) | Modell skaliert mit Pinch-Distanz |
| M4.5 | Anchor-System für mehrere Modelle | Zwei Modelle gleichzeitig manipulierbar |

---

### M5: Settings UI

**Ziel:** Slicer-Parameter und Ansichtssteuerung in VR.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M5.1 | XR Canvas mit Ray Interactor | UI im VR-Raum sichtbar und interagierbar |
| M5.2 | Layer-Height-Parameter (0.05–0.3 mm) | Slider aktualisiert Slicing |
| M5.3 | Scale-Factor für Ansichtsmaßstab | 1:1, 1:2, 1:10 auswählbar |
| M5.4 | Anzeige-Modus Toggle (Line/Solid) | Wechsel sofort wirksam |
| M5.5 | Schicht-Anzeige an/aus | Toggle schaltet SliceRenderer ein/aus |

---

### M6: 3MF-Unterstützung (Optional / Später)

**Ziel:** 3MF-Dateien als Alternative zu STL laden.

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M6.1 | `ThreeMFLoader` implementiert (ZIP + XML) | Lädt `.3mf` aus dem Zip/Archiv |
| M6.2 | Konvertierung zu `TriangulatedMesh` | Gleiche Ausgabe wie STL-Loader |
| M6.3 | Dateityp-Erkennung automatisch | `.stl` und `.3mf` werden unterschiedlich geladen |

---

### M7: Handtracking & Passthrough (Optional / Später)

| # | Aufgabe | Prüfkriterium |
|---|---------|---------------|
| M7.1 | Hand-tracking Input aktivierbar | `com.unity.xr.hands` geladen |
| M7.2 | Passthrough aktivierbar | Mixed-Reality-Modus auf Quest 3 |

---

## Entwicklungsschritte (nächste Aktionen)

1. **M0.3:** XR-Packages (OpenXR, XRI, Meta OpenXR) zum Projekt hinzufügen
2. **M0.4:** Build-Einstellungen auf Android ARM64 + IL2CPP setzen
3. **M0.5:** Git-Repository im Projektverzeichnis `QuestSlicer/` initialisieren
4. **M1.1:** `STLLoader` Kern-Implementierung beginnen

---

## Offene Entscheidungen

| Entscheidung | Optionen | Status |
|-------------|----------|--------|
| Koordinatensystem | Unity-Meter (÷1000) vs. eigene Units-Klasse | **Offen** |
| Slicer-Algorithmus | Plane-Cast vs. Octree-basiert | Plane-Cast (einfacher, ausreichend) |
| LineRenderer vs. custom Shader | Standard vs. custom | **Offen** – zuerst LineRenderer, dann optimieren |
| UI-Canvas | World Space vs. Screen Space | World Space (VR-nativer) |
| Modell-Import | Drag & Drop vs. UI-Browser | UI-Browser (VR-kompatibel) |
