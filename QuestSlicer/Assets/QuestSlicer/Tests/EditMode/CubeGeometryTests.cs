namespace QuestSlicer.Tests
{
    using NUnit.Framework;
    using UnityEngine;
    using QuestSlicer.Core.Geometry;

    /// <summary>
    /// Regressionstests für die Würfel-Mesh-Erzeugung.
    /// </summary>
    public class CubeGeometryTests
    {
        [Test]
        public void CreateCubeMm_20_Has24VerticesAnd24Normals()
        {
            var mesh = GeometryPrimitives.CreateCubeMm(20f);
            Assert.AreEqual(24, mesh.vertexCount);
            Assert.AreEqual(24, mesh.normals.Length);
        }

        [Test]
        public void CreateCubeMm_20_Has36ValidIndices()
        {
            var mesh = GeometryPrimitives.CreateCubeMm(20f);
            Assert.AreEqual(36, mesh.GetIndexCount(0));
            Assert.AreEqual(12, mesh.triangles.Length / 3);
            int maxIndex = -1;
            foreach (var i in mesh.triangles) maxIndex = Mathf.Max(maxIndex, i);
            Assert.Less(maxIndex, mesh.vertexCount);
        }

        [Test]
        public void CreateCubeMm_20_HasNoDegenerateTriangles()
        {
            var mesh = GeometryPrimitives.CreateCubeMm(20f);
            var v = mesh.vertices;
            var t = mesh.triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                var n = Vector3.Cross(v[t[i + 1]] - v[t[i]], v[t[i + 2]] - v[t[i]]);
                Assert.Greater(n.magnitude, 1e-12f, "degenerate triangle at " + i);
            }
        }

        [Test]
        public void CreateCubeMm_20_NormalsPointOutward()
        {
            var mesh = GeometryPrimitives.CreateCubeMm(20f);
            var v = mesh.vertices;
            var n = mesh.normals;
            var t = mesh.triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                var cross = Vector3.Cross(v[t[i + 1]] - v[t[i]], v[t[i + 2]] - v[t[i]]);
                // Winding must agree with the face normal of each triangle
                Assert.Greater(Vector3.Dot(cross, n[t[i]]), 0f, "inward winding at " + i);
            }
        }

        [Test]
        public void CreateCubeMm_20_HasEdgeLength0_02m()
        {
            var mesh = GeometryPrimitives.CreateCubeMm(20f);
            var size = mesh.bounds.size;
            Assert.AreEqual(0.02f, size.x, 0.0001f);
            Assert.AreEqual(0.02f, size.y, 0.0001f);
            Assert.AreEqual(0.02f, size.z, 0.0001f);
        }
    }
}
