using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Oddly.Editor
{
    [InitializeOnLoad]
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype.unity";

        static PrototypeSceneBuilder()
        {
            EditorApplication.delayCall += EnsurePrototype;
        }

        [MenuItem("Oddly/Create / Repair Prototype Scene")]
        public static void EnsurePrototype()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (File.Exists(ScenePath))
            {
                AddSceneToBuild();
                return;
            }

            Directory.CreateDirectory("Assets/Scenes");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            BuildScene();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuild();
            Debug.Log("ODDLY: Prototype scene created. Open Assets/Scenes/Prototype.unity and press Play.");
        }

        private static void AddSceneToBuild()
        {
            EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene entry in current)
                if (entry.path == ScenePath) return;

            var updated = new EditorBuildSettingsScene[current.Length + 1];
            current.CopyTo(updated, 0);
            updated[current.Length] = new EditorBuildSettingsScene(ScenePath, true);
            EditorBuildSettings.scenes = updated;
        }

        private static void BuildScene()
        {
            Material floorMat = MakeMaterial("Floor", new Color(0.78f, 0.83f, 0.88f));
            Material wallMat = MakeMaterial("Warm Walls", new Color(0.93f, 0.88f, 0.79f));
            Material accentMat = MakeMaterial("Oddly Coral", new Color(0.88f, 0.25f, 0.39f));
            Material cubeMat = MakeMaterial("Puzzle Blue", new Color(0.22f, 0.48f, 0.82f));
            Material doorMat = MakeMaterial("Exit Cream", new Color(0.98f, 0.72f, 0.35f));

            CreatePrimitive("Soft-lit floor", PrimitiveType.Cube, new Vector3(0, -0.25f, 0), new Vector3(18, 0.5f, 18), floorMat);
            CreatePrimitive("Back wall", PrimitiveType.Cube, new Vector3(0, 2.5f, 9), new Vector3(18, 5, 0.5f), wallMat);
            CreatePrimitive("Left wall", PrimitiveType.Cube, new Vector3(-9, 2.5f, 0), new Vector3(0.5f, 5, 18), wallMat);
            CreatePrimitive("Right wall", PrimitiveType.Cube, new Vector3(9, 2.5f, 0), new Vector3(0.5f, 5, 18), wallMat);
            CreatePrimitive("Front wall left", PrimitiveType.Cube, new Vector3(-5.2f, 2.5f, -9), new Vector3(7.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Front wall right", PrimitiveType.Cube, new Vector3(5.2f, 2.5f, -9), new Vector3(7.6f, 5, 0.5f), wallMat);

            CreatePrimitive("Puzzle pedestal", PrimitiveType.Cube, new Vector3(3, 0.55f, 3), new Vector3(2.2f, 1.1f, 2.2f), accentMat);
            GameObject goal = new GameObject("Place the cube here");
            goal.transform.position = new Vector3(3, 1.35f, 3);
            BoxCollider goalCollider = goal.AddComponent<BoxCollider>();
            goalCollider.isTrigger = true;
            goalCollider.size = new Vector3(1.3f, 1.0f, 1.3f);
            GoalZone goalZone = goal.AddComponent<GoalZone>();

            GameObject cube = CreatePrimitive("Perspective cube", PrimitiveType.Cube, new Vector3(-2.9f, 0.6f, 6.5f), Vector3.one, cubeMat);
            cube.AddComponent<Rigidbody>().mass = 1.2f;
            cube.AddComponent<PerspectiveObject>();

            GameObject door = CreatePrimitive("Exit door", PrimitiveType.Cube, new Vector3(0, 2, 8.7f), new Vector3(2.6f, 4, 0.35f), doorMat);
            DoorController doorController = door.AddComponent<DoorController>();
            SetPrivate(goalZone, "linkedDoor", doorController);

            // A second, viewpoint-dependent solution: line the dropped cube up
            // with the wall frame and match its apparent size from the camera.
            Vector3 frameCenter = new Vector3(-1.5f, 1.1f, 0.5f);
            CreatePrimitive("Perspective frame top", PrimitiveType.Cube,
                frameCenter + Vector3.up * 0.78f, new Vector3(1.7f, 0.14f, 0.18f), accentMat);
            CreatePrimitive("Perspective frame bottom", PrimitiveType.Cube,
                frameCenter - Vector3.up * 0.78f, new Vector3(1.7f, 0.14f, 0.18f), accentMat);
            CreatePrimitive("Perspective frame left", PrimitiveType.Cube,
                frameCenter + Vector3.left * 0.78f, new Vector3(0.14f, 1.42f, 0.18f), accentMat);
            CreatePrimitive("Perspective frame right", PrimitiveType.Cube,
                frameCenter + Vector3.right * 0.78f, new Vector3(0.14f, 1.42f, 0.18f), accentMat);
            GameObject target = new GameObject("Perspective target");
            target.transform.position = new Vector3(-1.5f, 1.1f, 0.37f);
            target.transform.localScale = Vector3.one * 0.72f;
            GameObject targetVisual = CreatePrimitive("Target marker", PrimitiveType.Cube,
                target.transform.position, Vector3.one * 0.72f, cubeMat);
            targetVisual.GetComponent<Collider>().enabled = false;

            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0, 0.15f, -5.8f);
            CharacterController character = player.AddComponent<CharacterController>();
            character.height = 1.8f;
            character.radius = 0.34f;
            character.center = new Vector3(0, 0.9f, 0);
            FirstPersonController movement = player.AddComponent<FirstPersonController>();

            GameObject cameraPivot = new GameObject("Camera Pivot");
            cameraPivot.transform.SetParent(player.transform);
            cameraPivot.transform.localPosition = new Vector3(0, 1.58f, 0);
            Camera camera = cameraPivot.AddComponent<Camera>();
            camera.fieldOfView = 78f;
            camera.nearClipPlane = 0.05f;
            cameraPivot.AddComponent<AudioListener>();
            SetPrivate(movement, "cameraPivot", cameraPivot.transform);

            GameObject holdPoint = new GameObject("Hold Point");
            holdPoint.transform.SetParent(cameraPivot.transform);
            holdPoint.transform.localPosition = new Vector3(0, -0.12f, 1.8f);
            InteractionController interaction = player.AddComponent<InteractionController>();
            SetPrivate(interaction, "playerCamera", camera);
            SetPrivate(interaction, "holdPoint", holdPoint.transform);

            PerspectiveFramePuzzle perspectivePuzzle = player.AddComponent<PerspectiveFramePuzzle>();
            SetPrivate(perspectivePuzzle, "playerCamera", camera);
            SetPrivate(perspectivePuzzle, "puzzleObject", cube.GetComponent<PerspectiveObject>());
            SetPrivate(perspectivePuzzle, "target", target.transform);
            SetPrivate(perspectivePuzzle, "linkedDoor", doorController);

            // Two matching freestanding doors create an impossible loop:
            // crossing one relocates the player to the other, which faces back
            // into the same room from a different side.
            Transform portalA = BuildImpossiblePortal("Doorway A", new Vector3(-6.4f, 0f, 2.5f),
                Quaternion.Euler(0f, 90f, 0f), accentMat, doorMat);
            Transform portalB = BuildImpossiblePortal("Doorway B", new Vector3(6.4f, 0f, 2.5f),
                Quaternion.Euler(0f, 270f, 0f), accentMat, doorMat);
            ImpossibleRoomPortal portalAController = portalA.gameObject.AddComponent<ImpossibleRoomPortal>();
            ImpossibleRoomPortal portalBController = portalB.gameObject.AddComponent<ImpossibleRoomPortal>();
            SetPrivate(portalAController, "destination", portalB);
            SetPrivate(portalBController, "destination", portalA);
            SetPrivate(portalAController, "pairedPortal", portalBController);
            SetPrivate(portalBController, "pairedPortal", portalAController);
            SetPrivate(portalAController, "sign", portalA.GetComponentInChildren<TextMesh>());
            SetPrivate(portalBController, "sign", portalB.GetComponentInChildren<TextMesh>());

            // Meta puzzle: the warning explicitly forbids the red button.
            // Reading it is the prerequisite for intentionally disobeying it.
            MetaPuzzleController metaPuzzle = player.AddComponent<MetaPuzzleController>();
            SetPrivate(metaPuzzle, "linkedDoor", doorController);

            GameObject notice = CreatePrimitive("Forbidden button notice", PrimitiveType.Cube,
                new Vector3(5.9f, 1.65f, -2.8f), new Vector3(1.25f, 1.15f, 0.12f), doorMat);
            MetaPuzzleNode noticeNode = notice.AddComponent<MetaPuzzleNode>();
            SetPrivate(noticeNode, "puzzle", metaPuzzle);
            SetPrivate(noticeNode, "action", MetaPuzzleAction.ReadNotice);
            TextMesh noticeText = new GameObject("Notice text").AddComponent<TextMesh>();
            noticeText.transform.SetParent(notice.transform, false);
            noticeText.transform.localPosition = new Vector3(0f, 0f, -0.56f);
            noticeText.transform.localRotation = Quaternion.identity;
            noticeText.text = "DO NOT\\nPRESS RED";
            noticeText.fontSize = 44;
            noticeText.characterSize = 0.035f;
            noticeText.anchor = TextAnchor.MiddleCenter;
            noticeText.alignment = TextAlignment.Center;
            noticeText.color = new Color(0.2f, 0.16f, 0.18f);

            GameObject forbiddenButton = CreatePrimitive("Forbidden red button", PrimitiveType.Sphere,
                new Vector3(5.9f, 0.95f, -1.6f), Vector3.one * 0.58f, accentMat);
            MetaPuzzleNode buttonNode = forbiddenButton.AddComponent<MetaPuzzleNode>();
            SetPrivate(buttonNode, "puzzle", metaPuzzle);
            SetPrivate(buttonNode, "action", MetaPuzzleAction.PressForbiddenButton);

            CreateLighting();
            Canvas canvas = CreateCanvas();

            // UI buttons require an EventSystem to receive pointer input.
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Text prompt = MakeText(canvas.transform, "Prompt", "WASD MOVE     MOUSE LOOK", 18, TextAnchor.UpperCenter);
            SetRect(prompt.rectTransform, new Vector2(0.15f, 0.91f), new Vector2(0.85f, 0.98f));

            Text speaker = MakeText(canvas.transform, "Speaker", "NARRATOR", 15, TextAnchor.MiddleLeft);
            SetRect(speaker.rectTransform, new Vector2(0.06f, 0.17f), new Vector2(0.94f, 0.22f));
            speaker.color = new Color(1f, 0.65f, 0.72f);

            Text subtitle = MakeText(canvas.transform, "Subtitle", "Welcome. This test is easy.", 21, TextAnchor.UpperLeft);
            SetRect(subtitle.rectTransform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.17f));

            DialogueDirector director = canvas.gameObject.AddComponent<DialogueDirector>();
            SetPrivate(director, "speakerLabel", speaker);
            SetPrivate(director, "subtitleLabel", subtitle);
            BehaviorTracker tracker = player.AddComponent<BehaviorTracker>();
            PrototypeHUD hud = canvas.gameObject.AddComponent<PrototypeHUD>();
            SetPrivate(hud, "interaction", interaction);
            SetPrivate(hud, "prompt", prompt);
            SetPrivate(goalZone, "dialogue", director);
            SetPrivate(doorController, "dialogue", director);
            SetPrivate(perspectivePuzzle, "dialogue", director);
            SetPrivate(portalAController, "dialogue", director);
            SetPrivate(portalBController, "dialogue", director);
            SetPrivate(metaPuzzle, "dialogue", director);

            movement.enabled = false;
            interaction.enabled = false;
            tracker.enabled = false;

            GameObject menu = new GameObject("Title Screen");
            menu.transform.SetParent(canvas.transform, false);
            Image overlay = menu.AddComponent<Image>();
            overlay.color = new Color(0.10f, 0.12f, 0.18f, 0.96f);
            SetRect(menu.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

            Text title = MakeText(menu.transform, "Game Title", "ODDLY", 54, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.1f, 0.56f), new Vector2(0.9f, 0.78f));
            Text tagline = MakeText(menu.transform, "Tagline", "A VERY WRONG LITTLE WORLD", 16, TextAnchor.MiddleCenter);
            SetRect(tagline.rectTransform, new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.58f));

            Button start = MakeButton(menu.transform, "START", new Vector2(0.35f, 0.34f), new Vector2(0.65f, 0.43f));
            start.onClick.AddListener(() => {
                menu.SetActive(false);
                movement.enabled = true;
                interaction.enabled = true;
                tracker.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                director.Say("intro");
            });

            Button language = MakeButton(menu.transform, "LANGUAGE: EN / TR", new Vector2(0.35f, 0.22f), new Vector2(0.65f, 0.31f));
            language.onClick.AddListener(() => {
                bool currentlyEnglish = PlayerPrefs.GetInt("oddly.language", 1) == 1;
                director.SetLanguage(currentlyEnglish ? SystemLanguage.Turkish : SystemLanguage.English);
                language.GetComponentInChildren<Text>().text = currentlyEnglish ? "DİL: TÜRKÇE" : "LANGUAGE: ENGLISH";
            });
        }

        private static Transform BuildImpossiblePortal(string portalName, Vector3 position,
            Quaternion rotation, Material frameMaterial, Material panelMaterial)
        {
            GameObject root = new GameObject(portalName);
            root.transform.SetPositionAndRotation(position, rotation);

            CreatePortalPart(root.transform, "Left post", new Vector3(-0.78f, 1.15f, 0f),
                new Vector3(0.16f, 2.3f, 0.22f), frameMaterial);
            CreatePortalPart(root.transform, "Right post", new Vector3(0.78f, 1.15f, 0f),
                new Vector3(0.16f, 2.3f, 0.22f), frameMaterial);
            CreatePortalPart(root.transform, "Header", new Vector3(0f, 2.28f, 0f),
                new Vector3(1.72f, 0.16f, 0.22f), frameMaterial);
            CreatePortalPart(root.transform, "Threshold", new Vector3(0f, 0.06f, 0f),
                new Vector3(1.58f, 0.12f, 0.32f), panelMaterial);

            BoxCollider trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.35f, 2.05f, 0.55f);
            trigger.center = new Vector3(0f, 1.08f, 0f);

            TextMesh sign = new GameObject("Unhelpful sign").AddComponent<TextMesh>();
            sign.transform.SetParent(root.transform, false);
            sign.transform.localPosition = new Vector3(0f, 2.62f, 0f);
            sign.transform.localRotation = Quaternion.identity;
            sign.text = portalName == "Doorway A" ? "THIS WAY OUT" : "YOU WERE HERE";
            sign.fontSize = 48;
            sign.characterSize = 0.045f;
            sign.anchor = TextAnchor.MiddleCenter;
            sign.alignment = TextAlignment.Center;
            sign.color = new Color(0.22f, 0.25f, 0.32f);
            return root.transform;
        }

        private static void CreatePortalPart(Transform parent, string name, Vector3 localPosition,
            Vector3 localScale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.identity;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new GameObject("Soft Key Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.91f, 0.82f);
            lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
            RenderSettings.ambientLight = new Color(0.62f, 0.66f, 0.74f);
        }

        private static Canvas CreateCanvas()
        {
            GameObject go = new GameObject("Game UI");
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Text MakeText(Transform parent, string name, string value, int size, TextAnchor alignment)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button MakeButton(Transform parent, string label, Vector2 min, Vector2 max)
        {
            GameObject go = new GameObject(label + " Button");
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = new Color(0.88f, 0.25f, 0.39f);
            Button button = go.AddComponent<Button>();
            SetRect(go.GetComponent<RectTransform>(), min, max);
            Text text = MakeText(go.transform, "Label", label, 20, TextAnchor.MiddleCenter);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one);
            return button;
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static GameObject CreatePrimitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static Material MakeMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { name = name, color = color };
            return material;
        }

        private static void SetPrivate(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }
    }
}
