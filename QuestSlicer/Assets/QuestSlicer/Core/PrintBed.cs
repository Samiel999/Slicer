namespace QuestSlicer.Core
{
    using UnityEngine;

    /// <summary>
    /// Druckbett mit konfigurierbaren Abmessungen.
    /// Alle Abmessungen werden in mm angegeben.
    /// </summary>
    public class PrintBed : MonoBehaviour
    {
        [Header("Print Bed Settings (mm)")]
        [SerializeField] private float widthMm = 250f;
        [SerializeField] private float depthMm = 250f;
        [SerializeField] private float heightMm = 5f;

        /// <summary>
        /// Druckbett-Breite in mm.
        /// </summary>
        public float WidthMm { get => widthMm; set => widthMm = Mathf.Max(1f, value); }

        /// <summary>
        /// Druckbett-Tiefe in mm.
        /// </summary>
        public float DepthMm { get => depthMm; set => depthMm = Mathf.Max(1f, value); }

        /// <summary>
        /// Druckbett-Höhe in mm.
        /// </summary>
        public float HeightMm { get => heightMm; set => heightMm = Mathf.Max(0.1f, value); }

        /// <summary>
        /// Druckbetten-Kantenlänge in Unity-Metern.
        /// </summary>
        public float WidthM => UnitConversion.MmToM(widthMm);

        /// <summary>
        /// Druckbetten-Tiefe in Unity-Metern.
        /// </summary>
        public float DepthM => UnitConversion.MmToM(depthMm);

        private void Reset()
        {
            // Set up default dimensions
            widthMm = 250f;
            depthMm = 250f;
            heightMm = 5f;
        }

        private void OnValidate()
        {
            WidthMm = widthMm;
            DepthMm = depthMm;
        }
    }
}
