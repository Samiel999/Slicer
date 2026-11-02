namespace QuestSlicer.Core
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Einfache Desktop-Testbedienung für SlicerWorkspace ohne Headset.
    /// Steuert das Modell mit WASD/Pfeiltasten (Bewegung auf der Druckbettebene)
    /// und Q/E bzw. R/F für Rotation. Die Debug-Kamera wird dabei nicht bewegt.
    /// Nur im Unity Editor aktiv, nicht im Android-Player.
    /// </summary>
    public class DesktopTestController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Modell, das bewegt und rotiert wird (ExampleCube_20mm)")]
        [SerializeField] private ModelTransformController modelController;

        [Tooltip("Richtungsreferenz für kamera-relative Bewegung (Debug Camera). Wird nicht verändert.")]
        [SerializeField] private Transform directionReference;

        [Header("Settings")]
        [SerializeField] private float moveSpeedMmPerSecond = 50f;
        [SerializeField] private float rotateSpeedDegPerSecond = 60f;

        private bool _warnedMissingReferences;

        private void Update()
        {
            if (!Application.isEditor) return;

            if (modelController == null || directionReference == null)
            {
                if (!_warnedMissingReferences)
                {
                    Debug.LogWarning("[QuestSlicer] DesktopTestController: modelController oder directionReference fehlt. Modellsteuerung deaktiviert.", this);
                    _warnedMissingReferences = true;
                }
                enabled = false;
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            float h = 0f;
            float v = 0f;
            float rotQ = 0f, rotE = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1f;
            if (keyboard.qKey.isPressed) rotQ -= 1f;
            if (keyboard.eKey.isPressed) rotQ += 1f;
            if (keyboard.rKey.isPressed) rotE -= 1f;
            if (keyboard.fKey.isPressed) rotE += 1f;

            bool hasInput = h != 0f || v != 0f || rotQ != 0f || rotE != 0f;
            if (!hasInput) return;

            float moveMm = moveSpeedMmPerSecond * Time.deltaTime;
            float rotDeg = rotateSpeedDegPerSecond * Time.deltaTime;

            // Kamera-relative Richtung, auf die Druckbettebene (XZ) projiziert
            Vector3 forward = Flatten(directionReference.forward);
            Vector3 right = Flatten(directionReference.right);
            Vector3 moveDirMm = (forward * v + right * h) * moveMm;
            if (moveDirMm != Vector3.zero) modelController.TranslateModelMm(moveDirMm);

            // Rotation am Modell mit der Desktop-Geschwindigkeit
            var model = modelController.transform;
            if (rotQ != 0f) model.Rotate(0, rotQ * rotDeg, 0, Space.World);
            if (rotE != 0f) model.Rotate(rotE * rotDeg, 0, 0, Space.Self);
        }

        private static Vector3 Flatten(Vector3 direction)
        {
            direction.y = 0f;
            return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.zero;
        }
    }
}
