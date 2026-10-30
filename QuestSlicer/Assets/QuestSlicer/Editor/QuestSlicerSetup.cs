namespace QuestSlicer.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using QuestSlicer.Core;
    using QuestSlicer.Core.Geometry;

    /// <summary>
    /// Editor-Setup-Routine für QuestSlicer.
    /// Idempotent: ein zweiter Durchlauf erzeugt keine Duplikate.
    /// </summary>
    public static class QuestSlicerSetup
    {
        [MenuItem("Tools/QuestSlicer/Setup Scene")]
        public static void SetupScene()
        {
            // Check if scene already exists
            var existingScene = SceneManager.GetSceneByPath("Assets/QuestSlicer/Scenes/SlicerWorkspace.unity");
            if (existingScene.IsValid())
            {
                Debug.Log("[QuestSlicer] SlicerWorkspace.unity exists. Opening for review.");
                EditorSceneManager.OpenScene("Assets/QuestSlicer/Scenes/SlicerWorkspace.unity");
                return;
            }

            // Create scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "SlicerWorkspace";

            // Add root objects
            SetupWorkspace(scene);
            SetupXROrigins(scene);
            SetupLighting(scene);
            SetupDebugCamera(scene);

            // Save scene
            string scenePath = "Assets/QuestSlicer/Scenes/SlicerWorkspace.unity";
            EditorSceneManager.SaveScene(scene, scenePath);

            // Update EditorBuildSettings
            var ebs = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/QuestSlicer/Scenes/SlicerWorkspace.unity", true)
            };
            EditorBuildSettings.scenes = ebs;
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[QuestSlicer] Created scene: " + scenePath);
            EditorSceneManager.OpenScene(scenePath);
        }

        private static void SetupWorkspace(Scene scene)
        {
            // Workspace root
            var workspace = new GameObject("Workspace");
            workspace.transform.parent = null;
            var printBed = workspace.AddComponent<PrintBed>();

            // Print bed
            var bed = new GameObject("PrintBed");
            bed.transform.parent = workspace.transform;
            bed.transform.position = new Vector3(0, -UnitConversion.MmToM(printBed.HeightMm / 2f), 0);

            // Create bed visual mesh
            CreateBedMesh(bed, printBed.WidthMm, printBed.DepthMm, printBed.HeightMm);

            // Example cube (20mm)
            var exampleCube = new GameObject("ExampleCube_20mm");
            exampleCube.transform.parent = workspace.transform;
            exampleCube.transform.position = new Vector3(0, UnitConversion.MmToM(10f), 0); // 10mm above bed

            var cubeMesh = GeometryPrimitives.CreateCubeMm(20f);
            var cubeMeshFilter = exampleCube.AddComponent<MeshFilter>();
            cubeMeshFilter.sharedMesh = cubeMesh;

            var cubeMeshRenderer = exampleCube.AddComponent<Renderer>();
            var cubeMaterial = CreateOrGetMaterial(new Color(0.8f, 0.4f, 0.2f), "CubeMaterial");
            cubeMeshRenderer.sharedMaterial = cubeMaterial;

            // Make cube grabbable (XRI will be available after package install)
            try
            {
                var pickableType = System.Type.GetType("Unity.XR.Interaction.Toolkit.Pickers.ObjectTargetPicker, Unity.XR.Interaction.Toolkit");
                if (pickableType != null)
                {
                    exampleCube.AddComponent(pickableType);
                }
            }
            catch { }

            // Transform controller
            var transformCtrl = exampleCube.AddComponent<ModelTransformController>();
            var workspaceScale = new WorkspaceScale();
            transformCtrl.workspaceScale = workspaceScale;
        }

        private static void CreateBedMesh(GameObject bedGO, float widthMm, float depthMm, float heightMm)
        {
            var bedBox = new GameObject("BedVisual");
            bedBox.transform.parent = bedGO.transform;
            bedBox.transform.localPosition = Vector3.zero;

            var size = new Vector3(
                UnitConversion.MmToM(widthMm),
                UnitConversion.MmToM(heightMm),
                UnitConversion.MmToM(depthMm)
            );

            var mesh = new Mesh { name = "PrintBed_Mesh" };
            mesh.SetVertices(CreateBoxVertices(size));
            mesh.SetIndices(CreateBoxIndices(), MeshTopology.Triangles, 0);
            mesh.SetNormals(CreateBoxNormals());
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);

            var bedFilter = bedBox.AddComponent<MeshFilter>();
            bedFilter.sharedMesh = mesh;

            var bedRenderer = bedBox.AddComponent<Renderer>();
            var bedMaterial = CreateOrGetMaterial(new Color(0.3f, 0.3f, 0.3f), "BedMaterial");
            bedRenderer.sharedMaterial = bedMaterial;
        }

        private static void SetupXROrigins(Scene scene)
        {
            // XR Origin placeholder structure
            var xrOrigin = new GameObject("XR Origin (XR Rig)");
            xrOrigin.transform.position = new Vector3(0, 0, -1);

            var cameraRig = new GameObject("Camera Offset");
            cameraRig.transform.parent = xrOrigin.transform;
            cameraRig.transform.localPosition = new Vector3(0, 1.6f, 0);

            var mainCamera = cameraRig.AddComponent<Camera>();
            mainCamera.gameObject.name = "XR Origin Camera";

            // Left controller anchor
            var leftAnchor = new GameObject("Left Controller");
            leftAnchor.transform.parent = cameraRig.transform;
            leftAnchor.transform.localPosition = new Vector3(-0.15f, -0.05f, -0.2f);

            // Right controller anchor
            var rightAnchor = new GameObject("Right Controller");
            rightAnchor.transform.parent = cameraRig.transform;
            rightAnchor.transform.localPosition = new Vector3(0.15f, -0.05f, -0.2f);

            // XR Origin component will be added by XR package setup after install
        }

        private static void SetupLighting(Scene scene)
        {
            var sun = new GameObject("Sun");
            var sunLight = sun.AddComponent<UnityEngine.Light>();
            sunLight.type = UnityEngine.LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sunLight.shadows = UnityEngine.LightShadows.Soft;
        }

        private static void SetupDebugCamera(Scene scene)
        {
            var debugCamera = new GameObject("Debug Camera");
            debugCamera.transform.position = new Vector3(0, 2, -4);
            debugCamera.transform.LookAt(Vector3.zero);

            var cam = debugCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.fieldOfView = 60f;
            cam.stereoTargetEye = StereoTargetEyeMask.None; // Desktop camera, not XR

            debugCamera.AddComponent<DesktopTestController>();
        }

        private static Material CreateOrGetMaterial(Color color, string name)
        {
            // Try to find existing material
            var renderers = Object.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                if (r.sharedMaterial != null && r.sharedMaterial.name == name)
                {
                    return r.sharedMaterial;
                }
            }

            // Use basic built-in Lit shader
            Shader litShader = Shader.Find("Universal RenderPipeline/Lit")
                           ?? Shader.Find("Standard")
                           ?? Shader.Find("Debug");

            var newMat = new Material(litShader);
            newMat.name = name;
            newMat.color = color;

            return newMat;
        }

        private static Vector3[] CreateBoxVertices(Vector3 size)
        {
            float hx = size.x * 0.5f, hy = size.y * 0.5f, hz = size.z * 0.5f;
            return new[]
            {
                new Vector3(-hx, -hy, hz), new Vector3(hx, -hy, hz), new Vector3(hx, hy, hz), new Vector3(-hx, hy, hz),
                new Vector3(-hx, -hy, -hz), new Vector3(hx, -hy, -hz), new Vector3(hx, hy, -hz), new Vector3(-hx, hy, -hz),
            };
        }

        private static int[] CreateBoxIndices()
        {
            return new[]
            {
                0, 1, 2, 0, 2, 3, // front
                5, 4, 7, 5, 7, 6, // back
                3, 2, 6, 3, 6, 7, // top
                4, 5, 1, 4, 1, 0, // bottom
                1, 5, 6, 1, 6, 2, // right
                4, 0, 3, 4, 3, 7, // left
            };
        }

        private static Vector3[] CreateBoxNormals()
        {
            var n = new Vector3[24];
            for (int i = 0; i < 4; i++) n[i] = new Vector3(0, 0, 1);
            for (int i = 4; i < 8; i++) n[i] = new Vector3(0, 0, -1);
            for (int i = 8; i < 12; i++) n[i] = new Vector3(0, 1, 0);
            for (int i = 12; i < 16; i++) n[i] = new Vector3(0, -1, 0);
            for (int i = 16; i < 20; i++) n[i] = new Vector3(1, 0, 0);
            for (int i = 20; i < 24; i++) n[i] = new Vector3(-1, 0, 0);
            return n;
        }
    }
}
