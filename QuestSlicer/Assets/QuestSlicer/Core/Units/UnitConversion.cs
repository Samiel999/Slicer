namespace QuestSlicer.Core
{
    /// <summary>
    /// Zentrale Einheitenumrechnung zwischen Millimetern (fachlich) und Unity-Metern.
    ///
    /// Konvention:
    /// - Alle Modell- und Druckraumdaten werden intern in mm geführt.
    /// - Unity verwendet Meter als interne Längeneinheit.
    /// - Ein separater Workspace-Ansichtsmaßstab skaliert nur die Kamera-Distanz,
    ///   nicht die tatsächlichen Modellabmessungen.
    ///
    /// Achsenkonvention:
    /// - Druckraumhöhe entspricht Unity Y (up).
    /// - Druckbettabdeckung liegt auf der XZ-Ebene.
    /// - Unity X = Druckraum X, Unity Z = Druckraum Z.
    /// </summary>
    public static class UnitConversion
    {
        /// <summary>
        /// Millimeter in Unity-Meter umrechnen.
        /// </summary>
        public static float MmToM(float mm)
        {
            return mm * 0.001f;
        }

        /// <summary>
        /// Unity-Meter in Millimeter umrechnen.
        /// </summary>
        public static float MToMm(float m)
        {
            return m / 0.001f;
        }

        /// <summary>
        /// Vector3 von mm nach Unity-Meter konvertieren.
        /// </summary>
        public static UnityEngine.Vector3 MmToM(UnityEngine.Vector3 mm)
        {
            return mm * 0.001f;
        }

        /// <summary>
        /// Vector3 von Unity-Meter nach mm konvertieren.
        /// </summary>
        public static UnityEngine.Vector3 MToMm(UnityEngine.Vector3 m)
        {
            return m / 0.001f;
        }
    }
}
