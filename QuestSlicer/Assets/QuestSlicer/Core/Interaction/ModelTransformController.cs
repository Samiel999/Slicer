namespace QuestSlicer.Core
{
    using UnityEngine;

    /// <summary>
    /// Steuert die Umrechnung zwischen VR-Controller-Bewegung und Modell-Bewegung.
    /// Verwendet WorkspaceScale, um bei verändertem Ansichtsmaßstab korrekt zurückzurechnen.
    /// </summary>
    public class ModelTransformController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WorkspaceScale workspaceScale;

        [Header("Settings")]
        [Tooltip("Geschwindigkeit der Modell-Translation in mm pro Sekunde")]
        [SerializeField] private float translationSpeedMmPerSecond = 100f;

        [Tooltip("Rotationsgeschwindigkeit in Grad pro Sekunde")]
        [SerializeField] private float rotationSpeedDegPerSecond = 90f;

        private WorkspaceScale Scale => workspaceScale ?? FindObjectOfType<WorkspaceScale>();

        /// <summary>
        /// Transformiert eine Controller-Bewegung in Unity-Metern
        /// in eine fachliche Modell-Bewegung in mm.
        /// </summary>
        public float ControllerMoveToModelMoveMm(float controllerMoveMeters)
        {
            return Scale?.CameraMoveToModelMoveMm(controllerMoveMeters) ?? (controllerMoveMeters * 1000f);
        }

        /// <summary>
        /// Wendet eine Controller-Bewegung auf das Modell an,
        /// unter Berücksichtigung des aktuellen Ansichtsmaßstabs.
        /// </summary>
        public void ApplyTranslationFromController(UnityEngine.Vector3 controllerDeltaMeters)
        {
            if (Scale == null) return;

            // Controller-Bewegung in mm (bezogen auf Modell-Größe)
            float moveMmX = Scale.CameraMoveToModelMoveMm(controllerDeltaMeters.x);
            float moveMmY = Scale.CameraMoveToModelMoveMm(controllerDeltaMeters.y);
            float moveMmZ = Scale.CameraMoveToModelMoveMm(controllerDeltaMeters.z);

            // In Unity-Meter zurückrechnen und anwenden
            transform.Translate(
                UnitConversion.MmToM(moveMmX),
                UnitConversion.MmToM(moveMmY),
                UnitConversion.MmToM(moveMmZ),
                Space.Self
            );
        }

        /// <summary>
        /// Wendet Controller-Input als Rotation auf das Modell an.
        /// </summary>
        public void ApplyRotationFromController(float horizontalAxis, float verticalAxis)
        {
            float yaw = rotationSpeedDegPerSecond * horizontalAxis * Time.deltaTime;
            float pitch = rotationSpeedDegPerSecond * verticalAxis * Time.deltaTime;

            transform.Rotate(0, yaw, 0, Space.World);
            transform.Rotate(pitch, 0, 0, Space.Self);
        }
    }
}
