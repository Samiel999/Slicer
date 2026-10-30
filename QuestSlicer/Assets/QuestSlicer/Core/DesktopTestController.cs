namespace QuestSlicer.Core
{
    using UnityEngine;

    /// <summary>
    /// Einfache Desktop-Testbedienung für SlicerWorkspace ohne Headset.
    /// Steuert ein ausgewähltes Modell mit WASD/Arrow keys und Q/E für Rotation.
    /// </summary>
    public class DesktopTestController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeedMmPerSecond = 50f;
        [SerializeField] private float rotateSpeedDegPerSecond = 60f;

        private void Update()
        {
            bool hasInput = false;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float rotQ = 0f, rotE = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;
            if (Input.GetKey(KeyCode.Q)) rotQ = -1f;
            if (Input.GetKey(KeyCode.E)) rotQ = 1f;
            if (Input.GetKey(KeyCode.R)) rotE = -1f;
            if (Input.GetKey(KeyCode.F)) rotE = 1f;

            if (h != 0f || v != 0f || rotQ != 0f || rotE != 0f)
            {
                hasInput = true;
            }

            if (!hasInput) return;

            float moveMeters = UnitConversion.MmToM(moveSpeedMmPerSecond * Time.deltaTime);
            float rotDeg = rotateSpeedDegPerSecond * Time.deltaTime;

            // Move forward/backward relative to camera direction
            if (v != 0f)
            {
                var forward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
                forward.y = 0f;
                if (forward.sqrMagnitude > 0.001f)
                {
                    forward = forward.normalized;
                    transform.Translate(forward * moveMeters * v, Space.World);
                }
            }

            // Strafe left/right
            if (h != 0f)
            {
                var right = Camera.main != null ? Camera.main.transform.right : Vector3.right;
                right.y = 0f;
                if (right.sqrMagnitude > 0.001f)
                {
                    right = right.normalized;
                    transform.Translate(right * moveMeters * h, Space.World);
                }
            }

            // Rotate
            if (rotQ != 0f)
            {
                transform.Rotate(0, rotQ * rotDeg, 0, Space.World);
            }
            if (rotE != 0f)
            {
                transform.Rotate(rotE * rotDeg, 0, 0, Space.Self);
            }
        }
    }
}
