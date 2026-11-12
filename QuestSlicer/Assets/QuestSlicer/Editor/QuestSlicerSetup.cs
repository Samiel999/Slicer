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
    /// Erzeugt die Workspace-Szene nur, wenn die Zieldatei noch nicht existiert.
    /// Kein vollständiger Idempotenzschutz: eine bereits vorhandene Datei wird nie überschrieben.
    /// </summary>
    public static class QuestSlicerSetup
    {
        [MenuItem("Tools/QuestSlicer/Setup Scene")]
        public static void SetupScene()
        {
            string scenePath = "Assets/QuestSlicer/Scenes/SlicerWorkspace.unity";

            // Check if scene already exists
            var existingScene = SceneManager.GetSceneByPath(scenePath);
            if (existingScene.IsValid())
            {
                Debug.Log("[QuestSlicer] SlicerWorkspace.unity exists. Opening for review.");
                EditorSceneManager.OpenScene(scenePath);
                return;
            }

            // Abort before any setup if the scene file exists on disk but is not loaded
            string projectRoot = System.IO.Path.GetDirectoryName(Application.dataPath);
            if (System.IO.File.Exists(System.IO.Path.Combine(projectRoot, scenePath)))
            {
                Debug.LogError("[QuestSlicer] " + scenePath + " existiert bereits auf der Platte. Setup abgebrochen, Datei wurde nicht überschrieben.");
                return;
            }

            // Create scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "SlicerWorkspace";

            // Add root objects
            ModelTransformController cubeController;
            SetupWorkspace(scene, out cubeController);
            SetupXROrigins(scene);
            SetupLighting(scene);
            SetupDebugCamera(scene, cubeController);

            // Save scene
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

        private static void SetupWorkspace(Scene scene, out ModelTransformController cubeController)
        {
            // Workspace root
            var workspace = new GameObject("Workspace");
            workspace.transform.parent = null;
            var printBed = workspace.AddComponent<PrintBed>();
            var scaleHost = workspace.AddComponent<WorkspaceScaleHost>();

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

            var cubeMeshRenderer = exampleCube.AddComponent<MeshRenderer>();
            var cubeMaterial = CreateOrGetMaterial(new Color(0.8f, 0.4f, 0.2f), "CubeMaterial");
            if (cubeMaterial != null) cubeMeshRenderer.sharedMaterial = cubeMaterial; // null: ambiguous name, error already logged

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
            var transformSerialized = new SerializedObject(transformCtrl);
            transformSerialized.FindProperty("workspaceScaleHost").objectReferenceValue = scaleHost;
            transformSerialized.ApplyModifiedPropertiesWithoutUndo();
            cubeController = transformCtrl;
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

            var bedRenderer = bedBox.AddComponent<MeshRenderer>();
            var bedMaterial = CreateOrGetMaterial(new Color(0.3f, 0.3f, 0.3f), "BedMaterial");
            if (bedMaterial != null) bedRenderer.sharedMaterial = bedMaterial; // null: ambiguous name, error already logged
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

        private static void SetupDebugCamera(Scene scene, ModelTransformController modelController)
        {
            var debugCamera = new GameObject("Debug Camera");
            debugCamera.transform.position = new Vector3(0, 2, -4);
            debugCamera.transform.LookAt(Vector3.zero);

            var cam = debugCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.fieldOfView = 60f;
            cam.stereoTargetEye = StereoTargetEyeMask.None; // Desktop camera, not XR

            // Explicit bindings: model to control, camera as direction reference (camera itself is not moved)
            var desktopCtrl = debugCamera.AddComponent<DesktopTestController>();
            var desktopSerialized = new SerializedObject(desktopCtrl);
            desktopSerialized.FindProperty("modelController").objectReferenceValue = modelController;
            desktopSerialized.FindProperty("directionReference").objectReferenceValue = debugCamera.transform;
            desktopSerialized.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Liefert das vorhandene Material mit dem Namen oder erzeugt ein neues.
        /// Gibt null zurück, wenn mehrere verschiedene Materialobjekte denselben Namen tragen:
        /// dann ist die Auswahl mehrdeutig und wird als Fehler gemeldet, statt einen Treffer zu wählen.
        /// </summary>
        private static Material CreateOrGetMaterial(Color color, string name)
        {
            // Collect distinct material objects with this name (same object on several renderers is fine)
            var matches = new System.Collections.Generic.HashSet<Material>();
            foreach (var r in Object.FindObjectsByType<Renderer>())
            {
                if (r.sharedMaterial != null && r.sharedMaterial.name == name)
                {
                    matches.Add(r.sharedMaterial);
                }
            }

            if (matches.Count == 1)
            {
                foreach (var existing in matches) return existing;
            }
            if (matches.Count > 1)
            {
                Debug.LogError("[QuestSlicer] Mehrdeutig: " + matches.Count + " verschiedene Materialobjekte namens '" + name + "' in der Szene. Materialzuweisung abgebrochen.");
                return null;
            }

            // URP Lit (exact name: "Universal Render Pipeline/Lit"). Falls nicht verfügbar,
            // wird das laut gemeldet: ein Built-in-Fallback wäre mit der aktiven URP-Pipeline nicht kompatibel.
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null)
            {
                Debug.LogError("[QuestSlicer] Shader 'Universal Render Pipeline/Lit' nicht gefunden. Material '" + name + "' verwendet Fallback.");
                litShader = Shader.Find("Standard") ?? Shader.Find("Debug");
            }

            var newMat = new Material(litShader);
            newMat.name = name;
            newMat.color = color;

            return newMat;
        }


        private static Vector3[] CreateBoxVertices(Vector3 size)
        {
            // 24 vertices: four per face, so each face has its own normal
            float hx = size.x * 0.5f, hy = size.y * 0.5f, hz = size.z * 0.5f;
            return new[]
            {
                // Front (+Z)
                new Vector3(-hx, -hy, hz), new Vector3(hx, -hy, hz), new Vector3(hx, hy, hz), new Vector3(-hx, hy, hz),
                // Back (-Z)
                new Vector3(hx, -hy, -hz), new Vector3(-hx, -hy, -hz), new Vector3(-hx, hy, -hz), new Vector3(hx, hy, -hz),
                // Top (+Y)
                new Vector3(-hx, hy, hz), new Vector3(hx, hy, hz), new Vector3(hx, hy, -hz), new Vector3(-hx, hy, -hz),
                // Bottom (-Y)
                new Vector3(-hx, -hy, -hz), new Vector3(hx, -hy, -hz), new Vector3(hx, -hy, hz), new Vector3(-hx, -hy, hz),
                // Right (+X)
                new Vector3(hx, -hy, hz), new Vector3(hx, -hy, -hz), new Vector3(hx, hy, -hz), new Vector3(hx, hy, hz),
                // Left (-X)
                new Vector3(-hx, -hy, -hz), new Vector3(-hx, -hy, hz), new Vector3(-hx, hy, hz), new Vector3(-hx, hy, -hz),
            };
        }

        private static int[] CreateBoxIndices()
        {
            return new[]
            {
                0, 1, 2, 0, 2, 3,       // front
                4, 5, 6, 4, 6, 7,       // back
                8, 9, 10, 8, 10, 11,    // top
                12, 13, 14, 12, 14, 15, // bottom
                16, 17, 18, 16, 18, 19, // right
                20, 21, 22, 20, 22, 23, // left
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
