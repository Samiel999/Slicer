namespace QuestSlicer.Core.Geometry
{
    using UnityEngine;

    /// <summary>
    /// Erstellt einfache 3D-Geometrie in mm.
    /// Diese Klasse ist vollständig Unity-abhängig, aber testbar,
    /// da sie nur primitive Mesh-Operationen verwendet.
    /// </summary>
    public static class GeometryPrimitives
    {
        /// <summary>
        /// Erstellt einen Würfel mit gegebener Kantenlänge in mm.
        /// Das resultierende Mesh ist in Unity-Meter zentriert.
        /// </summary>
        public static Mesh CreateCubeMm(float edgeLengthMm)
        {
            float edgeLengthM = UnitConversion.MmToM(edgeLengthMm);
            Mesh mesh = new Mesh
            {
                name = $"Cube_{edgeLengthMm}mm"
            };

            mesh.SetIndices(GetCubeIndices(), MeshTopology.Triangles, 0);
            mesh.SetVertices(GetCubeVertices(edgeLengthM));
            mesh.SetNormals(GetCubeNormals());
            mesh.SetUVs(0, GetCubeUVs());
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);

            return mesh;
        }

        private static Vector3[] GetCubeVertices(float size)
        {
            float half = size * 0.5f;
            return new[]
            {
                // Front face
                new Vector3(-half, -half, half),
                new Vector3(half, -half, half),
                new Vector3(half, half, half),
                new Vector3(-half, half, half),
                // Back face
                new Vector3(-half, -half, -half),
                new Vector3(half, -half, -half),
                new Vector3(half, half, -half),
                new Vector3(-half, half, -half),
            };
        }

        private static int[] GetCubeIndices()
        {
            // 12 triangles (36 indices) for a cube
            return new[]
            {
                // Front
                0, 1, 2, 0, 2, 3,
                // Back
                5, 4, 7, 5, 7, 6,
                // Top
                3, 2, 6, 3, 6, 7,
                // Bottom
                4, 5, 1, 4, 1, 0,
                // Right
                1, 5, 6, 1, 6, 2,
                // Left
                4, 0, 3, 4, 3, 7,
            };
        }

        private static Vector3[] GetCubeNormals()
        {
            return new[]
            {
                new Vector3(0, 0, 1),  // Front
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), // Back
                new Vector3(0, 0, -1),
                new Vector3(0, 0, -1),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),  // Top
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0), // Bottom
                new Vector3(0, -1, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),  // Right
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0), // Left
                new Vector3(-1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(-1, 0, 0),
            };
        }

        private static Vector2[] GetCubeUVs()
        {
            return new[]
            {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            };
        }
    }
}
