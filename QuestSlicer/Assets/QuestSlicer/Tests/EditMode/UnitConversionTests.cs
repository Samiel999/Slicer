namespace QuestSlicer.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Tests für die mm/m-Umrechnung.
    /// Vollständig ohne Unity-Abhängigkeiten testbar.
    /// </summary>
    public class UnitConversionTests
    {
        public void MmToM_20mm_Returns0_02m()
        {
            float result = QuestSlicer.Core.UnitConversion.MmToM(20f);
            Assert.AreEqual(0.02f, result, 0.0001f);
        }

        public void MToMm_0_02m_Returns20mm()
        {
            float result = QuestSlicer.Core.UnitConversion.MToMm(0.02f);
            Assert.AreEqual(20f, result, 0.0001f);
        }

        public void MmToM_Roundtrip_ReturnsOriginal()
        {
            const float original = 150f;
            float result = QuestSlicer.Core.UnitConversion.MToMm(QuestSlicer.Core.UnitConversion.MmToM(original));
            Assert.AreEqual(original, result, 0.0001f);
        }

        public void MToMm_Roundtrip_ReturnsOriginal()
        {
            const float original = 0.15f;
            float result = QuestSlicer.Core.UnitConversion.MmToM(QuestSlicer.Core.UnitConversion.MToMm(original));
            Assert.AreEqual(original, result, 0.0001f);
        }

        public void MmToM_Zero_ReturnsZero()
        {
            float result = QuestSlicer.Core.UnitConversion.MmToM(0f);
            Assert.AreEqual(0f, result);
        }

        public void MmToM_Negative_ReturnsNegative()
        {
            float result = QuestSlicer.Core.UnitConversion.MmToM(-10f);
            Assert.AreEqual(-0.01f, result, 0.0001f);
        }

        public void MmToM_Vector3_PreservesComponents()
        {
            var input = new UnityEngine.Vector3(10, 20, 30);
            var expected = new UnityEngine.Vector3(0.01f, 0.02f, 0.03f);
            var result = QuestSlicer.Core.UnitConversion.MmToM(input);

            Assert.AreEqual(expected.x, result.x, 0.0001f);
            Assert.AreEqual(expected.y, result.y, 0.0001f);
            Assert.AreEqual(expected.z, result.z, 0.0001f);
        }

        public void MToMm_Vector3_PreservesComponents()
        {
            var input = new UnityEngine.Vector3(0.01f, 0.02f, 0.03f);
            var expected = new UnityEngine.Vector3(10, 20, 30);
            var result = QuestSlicer.Core.UnitConversion.MToMm(input);

            Assert.AreEqual(expected.x, result.x, 0.0001f);
            Assert.AreEqual(expected.y, result.y, 0.0001f);
            Assert.AreEqual(expected.z, result.z, 0.0001f);
        }
    }

    /// <summary>
    /// Tests für den Workspace-Ansichtsmaßstab.
    /// </summary>
    public class WorkspaceScaleTests
    {
        public void ViewScale_DefaultIs1()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            Assert.AreEqual(1.0f, scale.ViewScale);
        }

        public void ViewScale_SetsCorrectValue()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = 0.5f;
            Assert.AreEqual(0.5f, scale.ViewScale);
        }

        public void ViewScale_MinimumClampedTo0_01()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = -100f;
            Assert.AreEqual(0.01f, scale.ViewScale);
        }

        public void CameraMoveToModelMoveMm_Scale1_1_1Ratio()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = 1.0f;
            float result = scale.CameraMoveToModelMoveMm(0.1f);
            Assert.AreEqual(100f, result, 0.01f);
        }

        public void CameraMoveToModelMoveMm_Scale0_5_2xAmplification()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = 0.5f;
            float result = scale.CameraMoveToModelMoveMm(0.1f);
            Assert.AreEqual(200f, result, 0.01f);
        }

        public void ModelMoveMmToCameraMoveM_Scale2_0_5xReduction()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = 2.0f;
            float result = scale.ModelMoveMmToCameraMoveM(100f);
            Assert.AreEqual(0.2f, result, 0.001f);
        }

        public void ViewScale_DoesNotChangeModelDimensions()
        {
            var scale = new QuestSlicer.Core.WorkspaceScale();
            scale.ViewScale = 0.5f;
            const float modelDimensionMm = 20f;
            float converted = QuestSlicer.Core.UnitConversion.MmToM(modelDimensionMm);
            Assert.AreEqual(0.02f, converted, 0.0001f);
        }
    }
}
