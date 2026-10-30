namespace QuestSlicer.Core
{
    /// <summary>
    /// Verwaltet den Workspace-Ansichtsmaßstab, der die VR-Kamera-Distanz steuert,
    /// ohne die fachlichen Modellabmessungen (in mm) zu verändern.
    /// </summary>
    public class WorkspaceScale
    {
        private float _viewScale = 1.0f;

        /// <summary>
        /// Ansichtsmaßstab. 1.0 = 1:1, 0.5 = 1:2 (Modell wird kleiner angezeigt).
        /// </summary>
        public float ViewScale
        {
            get => _viewScale;
            set => _viewScale = value < 0.01f ? 0.01f : value;
        }

        /// <summary>
        /// Rechnet eine VR-Kamera-Bewegung in eine fachliche Modell-Bewegung zurück.
        /// Bei ViewScale = 0.5 bedeutet 1 Meter Kamerabewegung 0.5 mm Modellverschiebung.
        /// </summary>
        public float CameraMoveToModelMoveMm(float cameraMoveMeters)
        {
            return cameraMoveMeters / _viewScale * 1000f;
        }

        /// <summary>
        /// Rechnet eine fachliche Modellbewegung (mm) in eine VR-Kameradistanz um.
        /// </summary>
        public float ModelMoveMmToCameraMoveM(float modelMoveMm)
        {
            return modelMoveMm / 1000f * _viewScale;
        }
    }
}
