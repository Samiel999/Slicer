# QuestSlicer – Architektur-Dokumentation

## Überblick

QuestSlicer ist eine standalone VR-App für die Meta Quest 3, die 3D-Druckmodelle
(STL, später 3MF) lädt, geometrisch in Schichten zerlegt und die Konturen visuell
darstellt. Sie ist ein **Viewer und Analyzer**, kein vollständiger Slicer mit
G-Code-Generierung.

## System-Architektur

```
┌─────────────────────────────────────────────────────────┐
│                   VR Presentation Layer                  │
│  ┌─────────────┐  ┌──────────────┐  ┌────────────────┐ │
│  │   XR Input   │  │  Slice View   │  │   Settings UI  │ │
│  │  (Controllers)│  │(Line/Mesh)   │  │ (Hololens UI) │ │
│  └──────┬───────┘  └──────┬───────┘  └───────┬────────┘ │
│         │                 │                   │           │
├─────────┼─────────────────┼───────────────────┼───────────┤
│         │                 │                   │           │
│  ┌──────▼─────────────────▼───────────────────▼─────────┐ │
│  │              Slice Renderer (Unity)                    │ │
│  │  · LineRenderer pro Kontur                            │ │
│  │  · Optional: Mesh-Overlay für Schichten               │ │
│  │  · Farbgebung nach Schicht-Höhe                       │ │
│  └──────────────────────┬───────────────────────────────┘ │
│                         │                                  │
│  ┌──────────────────────▼───────────────────────────────┐ │
│  │             Model Manipulator                         │ │
│  │  · Position (6DOF) via Controller                     │ │
│  │  · Rotation (6DOF) via Controller                     │ │
│  │  · Scale (Uniform / Axis) via Controller              │ │
│  └──────────────────────┬───────────────────────────────┘ │
│                         │                                  │
├─────────────────────────┼─────────────────────────────────┤
│                         │                                  │
│  ┌──────────────────────▼───────────────────────────────┐ │
│  │              Slicer Engine (Pure C#)                  │ │
│  │  · SliceModel – Verwaltung des geladenen Modells      │ │
│  │  · SliceCalculator – Plane-Cast-Slicer                │ │
│  │  · SliceResult – Schicht-Datenstruktur                │ │
│  └──────────────────────┬───────────────────────────────┘ │
│                         │                                  │
│  ┌──────────────────────▼───────────────────────────────┐ │
│  │              Model Loader (IO)                        │ │
│  │  · STL Loader (binary + ASCII)                        │ │
│  │  · (Future) 3MF Loader                                │ │
│  │  · Ergebnis: TriangulatedMesh in mm                   │ │
│  └──────────────────────────────────────────────────────┘ │
└───────────────────────────────────────────────────────────┘
```

## Module

### 1. Model Loader (Assets/Scripts/IO/)

**Aufgabe:** STL-Dateien (binär und ASCII) in eine einheitliche
`TriangulatedMesh`-Struktur konvertieren.

**Schnittstelle:**
```csharp
public interface IModelLoader
{
    TriangulatedMesh Load(string filePath);
}

public struct TriangulatedMesh
{
    public Vector3[] Vertices;     // in mm
    public int[] Indices;          // trippel-weise
    public Vector3[] Normals;      // optional
    public Vector3 BoundsMin;      // bounding box in mm
    public Vector3 BoundsMax;
}
```

**Entscheidungen:**
- **STL zuerst:** Einfachste 3D-Form, weit verbreitet, keine Kompression/Zip.
- **3MF später:** Komplexer (Zip-basiert, XML-Namespaces), kommt in Milestone 4.
- **STEP/IGES extern:** App erwartet bereits triangulierte STL-Dateien.
- **Koordinaten:** Import immer in **mm**, Unity verwendet standardmäßig Meter.
  Der Loader skaliert intern auf Unity-Meter (÷1000) oder arbeitet in einer
  benutzerdefinierten Einheiten-Klasse.

### 2. Slicer Engine (Assets/Scripts/Core/)

**Aufgabe:** `TriangulatedMesh` in Schichtkonturen zerlegen.

**Algorithmus:** Plane-Cast pro Schicht-Höhe.
- Für jede Schicht `i` mit Höhe `h = i * layerHeight`:
  - Schnittebene bei `y = h`
  - Jedes Dreieck auf Schnitt mit der Ebene prüfen
  - Schnittkanten sammeln und zu geschlossenen Konturen verknüpfen
  - Äußere Kontur + innere Löcher (Holes) identifizieren

**Schnittstelle:**
```csharp
public struct SliceResult
{
    public float LayerHeight;      // mm
    public SliceLayer[] Layers;    // [0..numLayers-1]
    public Vector3 ModelBoundsMin; // mm, vor dem Zerschneiden
    public Vector3 ModelBoundsMax; // mm
}

public struct SliceLayer
{
    public int LayerIndex;         // 0-basiert
    public float HeightMM;         // absolute Höhe in mm
    public Vector2[] OuterContours;// List von Polygonen (2D-Projektion)
    public bool HasInfill;
}
```

**Testbarkeit:**
- Die gesamte Logik ist **ohne Unity**-Code geschrieben.
- EditMode-Tests können `SliceCalculator.Calculate()` mit künstlichen
  `TriangulatedMesh`-Daten aufrufen.
- Vergleich mit Referenz-Ergebnissen pro Schicht.

### 3. Slice Renderer (Assets/Scripts/View/)

**Aufgabe:** `SliceResult` in sichtbare VR-Geometrie übersetzen.

**Darstellungsmodi:**
1. **Line Mode** (Standard): `LineRenderer` für jede Kontur,
   Farbe basierend auf Schicht-Höhe.
2. **Solid Mode:** Dünne Mesh-Ebenen für jede Schicht.
3. **Wireframe Mode:** Drahtgitter des Originalmodells + Konturen.

**Entscheidungen:**
- **Line Mode zuerst:** Geringster Overhead auf Quest 3, gut für große Modelle.
- **Solid Mode als Option:** Für kleinere Modelle mit wenigen Schichten.
- **Kein Partikel-System:** Keine Animation während des Slicens.

### 4. VR Interaction (Assets/Scripts/View/)

**Aufgabe:** Modelle im VR-Raum manipulieren.

**Komponenten:**
- **SceneModelHolder:** Parent-Objekt für geladenes Modell.
- **ModelManipulator:** XR-interagierbare Transformation (Position, Rotation, Scale).
- **AnchorManager:** Verwaltung mehrerer Modell-Anker in einer Szene.

**Controller-Abbildung:**
- **Left Grip:** Modell greifen und verschieben (XRI Pickable).
- **Right Trigger:** Rotation um Y-Achse.
- **Pinch (Index+Thumb):** Skalieren mit Trigger-Value.

### 5. Settings UI (Assets/Scripts/UI/)

**Aufgabe:** Slicer-Parameter und Ansichts-Steuerung in VR.

**Parameter:**
- Layer Height (0.05 – 0.3 mm, Schrittweite 0.05)
- Scale Factor (Ansichtsmaßstab, z. B. 1:1, 1:2, 1:10)
- Anzeige-Modus (Line/Solid/Wireframe)
- Farben-Schema (Regenbogen, Monochrom, nach Schicht)
- Schicht-Indikator anzeigen/verbergen

**UI-Implementierung:**
- **XR Canvas** (World Space) mit XRI Ray Interactor.
- Unity UI (UGUI) als Canvas, nicht IMGUI.
- **Kein** Touchscreen/UI für Desktop.

## Datenfluss

```
STL-Datei
    │
    ▼
STLLoader → TriangulatedMesh (Vertices in mm)
    │
    ▼
SliceCalculator → SliceResult (Layers mit Konturen)
    │
    ▼
SliceRenderer → LineRenderer/Mesh (VR-sichtbar)
    │
    ▼
ModelManipulator → VR-Input → Transform-Updates
```

## Abhängigkeiten

| Package | Version | Zweck |
|---------|---------|-------|
| com.unity.render-pipelines.universal | 17.6.0 | URP Rendering |
| com.unity.xr.openxr | 1.17.1 | OpenXR Runtime |
| com.unity.xr.interaction.toolkit | 3.5.1 | VR Input & Interaction |
| com.unity.xr.meta-openxr | 2.5.1 | Meta Quest spezifisch |
| com.unity.xr.core-utils | 2.6.0 | XR-Hilfsklassen |
| com.unity.xr.hands | 1.8.1 | Handtracking (später) |
| com.unity.xr.management | 4.6.0 | XR Plugin Mgmt |
| com.unity.inputsystem | 1.19.0+ | Input System |
| com.unity.test-framework | 1.8.0 | EditMode Tests |

## Nicht-Ziele

Siehe [AGENTS.md §5](../AGENTS.md#5-nicht-ziele-out-of-scope).
