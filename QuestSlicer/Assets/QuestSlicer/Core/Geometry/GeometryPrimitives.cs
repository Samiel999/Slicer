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

            // Vertices must be set before indices, otherwise Unity discards the index buffer
            mesh.SetVertices(GetCubeVertices(edgeLengthM));
            mesh.SetIndices(GetCubeIndices(), MeshTopology.Triangles, 0);
            mesh.SetNormals(GetCubeNormals());
            mesh.SetUVs(0, GetCubeUVs());
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);

            return mesh;
        }

        private static Vector3[] GetCubeVertices(float size)
        {
            // 24 vertices: four per face, so each face has its own normal
            float h = size * 0.5f;
            return new[]
            {
                // Front (+Z)
                new Vector3(-h, -h, h), new Vector3(h, -h, h), new Vector3(h, h, h), new Vector3(-h, h, h),
                // Back (-Z)
                new Vector3(h, -h, -h), new Vector3(-h, -h, -h), new Vector3(-h, h, -h), new Vector3(h, h, -h),
                // Top (+Y)
                new Vector3(-h, h, h), new Vector3(h, h, h), new Vector3(h, h, -h), new Vector3(-h, h, -h),
                // Bottom (-Y)
                new Vector3(-h, -h, -h), new Vector3(h, -h, -h), new Vector3(h, -h, h), new Vector3(-h, -h, h),
                // Right (+X)
                new Vector3(h, -h, h), new Vector3(h, -h, -h), new Vector3(h, h, -h), new Vector3(h, h, h),
                // Left (-X)
                new Vector3(-h, -h, -h), new Vector3(-h, -h, h), new Vector3(-h, h, h), new Vector3(-h, h, -h),
            };
        }

        private static int[] GetCubeIndices()
        {
            // 12 triangles (36 indices), two per face, four vertices per face
            return new[]
            {
                0, 1, 2, 0, 2, 3,       // Front
                4, 5, 6, 4, 6, 7,       // Back
                8, 9, 10, 8, 10, 11,    // Top
                12, 13, 14, 12, 14, 15, // Bottom
                16, 17, 18, 16, 18, 19, // Right
                20, 21, 22, 20, 22, 23, // Left
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
