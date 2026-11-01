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
            // Leave a real opening around the main exit, then continue the room
            // into a narrow corridor so the player has somewhere to go.
            CreatePrimitive("Back wall left", PrimitiveType.Cube, new Vector3(-5.2f, 2.5f, 9), new Vector3(7.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Back wall right", PrimitiveType.Cube, new Vector3(5.2f, 2.5f, 9), new Vector3(7.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Back wall above exit", PrimitiveType.Cube, new Vector3(0, 4.65f, 9), new Vector3(3.2f, 0.7f, 0.5f), wallMat);
            CreatePrimitive("Exit corridor floor", PrimitiveType.Cube, new Vector3(0, -0.25f, 17), new Vector3(3.6f, 0.5f, 16), floorMat);
            CreatePrimitive("Exit corridor left", PrimitiveType.Cube, new Vector3(-1.8f, 2.5f, 17), new Vector3(0.25f, 5, 16), wallMat);
            CreatePrimitive("Exit corridor right", PrimitiveType.Cube, new Vector3(1.8f, 2.5f, 17), new Vector3(0.25f, 5, 16), wallMat);
            CreatePrimitive("Exit corridor ceiling", PrimitiveType.Cube, new Vector3(0, 5.05f, 17), new Vector3(3.6f, 0.2f, 16), wallMat);
            GameObject corridorEndPanel = CreatePrimitive("Corridor end wall panel", PrimitiveType.Cube,
                new Vector3(0, 2.5f, 25), new Vector3(3.6f, 5, 0.5f), wallMat);

            // The false exit is a moving wall. Behind it sits a larger,
            // deliberately over-bright waiting room that becomes the next space.
            CreatePrimitive("Waiting room floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 31), new Vector3(8, 0.5f, 12), floorMat);
            CreatePrimitive("Waiting room left wall", PrimitiveType.Cube,
                new Vector3(-4, 2.5f, 31), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Waiting room right wall", PrimitiveType.Cube,
                new Vector3(4, 2.5f, 31), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Waiting room far wall left", PrimitiveType.Cube,
                new Vector3(-2.65f, 2.5f, 37), new Vector3(2.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Waiting room far wall right", PrimitiveType.Cube,
                new Vector3(2.65f, 2.5f, 37), new Vector3(2.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Waiting room far wall above exit", PrimitiveType.Cube,
                new Vector3(0, 4.65f, 37), new Vector3(2.6f, 0.7f, 0.5f), wallMat);
            GameObject waitingRoomDoor = CreatePrimitive("Waiting room perspective door",
                PrimitiveType.Cube, new Vector3(0, 2, 36.7f), new Vector3(2.6f, 4, 0.35f), doorMat);
            DoorController waitingRoomDoorController = waitingRoomDoor.AddComponent<DoorController>();
            CreatePrimitive("Waiting room welcome plinth", PrimitiveType.Cube,
                new Vector3(0, 0.55f, 32.8f), new Vector3(2.4f, 1.1f, 2.4f), accentMat);
            TextMesh waitingRoomSign = new GameObject("Waiting room sign").AddComponent<TextMesh>();
            waitingRoomSign.transform.position = new Vector3(0, 3.4f, 36.65f);
            waitingRoomSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            waitingRoomSign.text = "WAITING ROOM";
            waitingRoomSign.fontSize = 56;
            waitingRoomSign.characterSize = 0.06f;
            waitingRoomSign.anchor = TextAnchor.MiddleCenter;
            waitingRoomSign.alignment = TextAlignment.Center;
            waitingRoomSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh falseExitSign = new GameObject("False exit sign").AddComponent<TextMesh>();
            falseExitSign.transform.position = new Vector3(0, 3.15f, 23.8f);
            falseExitSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            falseExitSign.text = "EXIT";
            falseExitSign.fontSize = 64;
            falseExitSign.characterSize = 0.075f;
            falseExitSign.anchor = TextAnchor.MiddleCenter;
            falseExitSign.alignment = TextAlignment.Center;
            falseExitSign.color = new Color(0.88f, 0.25f, 0.39f);

            GameObject corridorBeat = new GameObject("False exit reveal trigger");
            corridorBeat.transform.position = new Vector3(0, 1.2f, 20.5f);
            BoxCollider corridorBeatCollider = corridorBeat.AddComponent<BoxCollider>();
            corridorBeatCollider.isTrigger = true;
            corridorBeatCollider.size = new Vector3(3.2f, 2.4f, 0.8f);
            ChapterBeatTrigger corridorBeatTrigger = corridorBeat.AddComponent<ChapterBeatTrigger>();
            SetPrivate(corridorBeatTrigger, "sign", falseExitSign);
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

            // The waiting room's exit is visible immediately, but its frame
            // only accepts the carried object when its screen projection matches.
            GameObject waitingCube = CreatePrimitive("Waiting room perspective cube",
                PrimitiveType.Cube, new Vector3(-2.55f, 0.65f, 30.1f), Vector3.one, cubeMat);
            waitingCube.AddComponent<Rigidbody>().mass = 1.2f;
            PerspectiveObject waitingPerspectiveObject = waitingCube.AddComponent<PerspectiveObject>();

            Vector3 waitingFrameCenter = new Vector3(0f, 1.2f, 36.42f);
            CreatePrimitive("Waiting frame top", PrimitiveType.Cube,
                waitingFrameCenter + Vector3.up * 0.78f, new Vector3(1.7f, 0.14f, 0.18f), accentMat);
            CreatePrimitive("Waiting frame bottom", PrimitiveType.Cube,
                waitingFrameCenter - Vector3.up * 0.78f, new Vector3(1.7f, 0.14f, 0.18f), accentMat);
            CreatePrimitive("Waiting frame left", PrimitiveType.Cube,
                waitingFrameCenter + Vector3.left * 0.78f, new Vector3(0.14f, 1.42f, 0.18f), accentMat);
            CreatePrimitive("Waiting frame right", PrimitiveType.Cube,
                waitingFrameCenter + Vector3.right * 0.78f, new Vector3(0.14f, 1.42f, 0.18f), accentMat);
            GameObject waitingTarget = new GameObject("Waiting room perspective target");
            waitingTarget.transform.position = waitingFrameCenter;
            waitingTarget.transform.localScale = Vector3.one * 0.72f;
            GameObject waitingTargetVisual = CreatePrimitive("Waiting room target marker",
                PrimitiveType.Cube, waitingFrameCenter, Vector3.one * 0.72f, cubeMat);
            waitingTargetVisual.GetComponent<Collider>().enabled = false;

            // The next chamber teaches a two-object scale relay: one object
            // must remain small while the other becomes oversized.
            CreatePrimitive("Scale relay chamber floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 47), new Vector3(10, 0.5f, 20), floorMat);
            CreatePrimitive("Scale relay chamber left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 47), new Vector3(0.25f, 5, 20), wallMat);
            CreatePrimitive("Scale relay chamber right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 47), new Vector3(0.25f, 5, 20), wallMat);
            CreatePrimitive("Scale relay far wall left", PrimitiveType.Cube,
                new Vector3(-4.2f, 2.5f, 57), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Scale relay far wall right", PrimitiveType.Cube,
                new Vector3(4.2f, 2.5f, 57), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Scale relay far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.65f, 57), new Vector3(6.8f, 0.7f, 0.5f), wallMat);
            GameObject relayExit = CreatePrimitive("Dual scale relay exit",
                PrimitiveType.Cube, new Vector3(0, 2, 56.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController relayExitController = relayExit.AddComponent<DoorController>();

            Vector3 smallPadPosition = new Vector3(-2.25f, 0.08f, 52.2f);
            Vector3 largePadPosition = new Vector3(2.25f, 0.08f, 52.2f);
            CreatePrimitive("Small scale relay pad", PrimitiveType.Cylinder,
                smallPadPosition, new Vector3(1.8f, 0.16f, 1.8f), accentMat);
            CreatePrimitive("Large scale relay pad", PrimitiveType.Cylinder,
                largePadPosition, new Vector3(1.8f, 0.16f, 1.8f), accentMat);
            GameObject smallRelayTarget = new GameObject("Small relay target");
            smallRelayTarget.transform.position = new Vector3(smallPadPosition.x, 0.55f, smallPadPosition.z);
            GameObject largeRelayTarget = new GameObject("Large relay target");
            largeRelayTarget.transform.position = new Vector3(largePadPosition.x, 0.9f, largePadPosition.z);

            GameObject smallRelayCube = CreatePrimitive("Small relay cube",
                PrimitiveType.Cube, new Vector3(-2.25f, 0.6f, 40.5f), Vector3.one, cubeMat);
            smallRelayCube.AddComponent<Rigidbody>().mass = 1.2f;
            PerspectiveObject smallRelayObject = smallRelayCube.AddComponent<PerspectiveObject>();
            GameObject largeRelaySphere = CreatePrimitive("Large relay sphere",
                PrimitiveType.Sphere, new Vector3(2.25f, 0.6f, 40.5f), Vector3.one, doorMat);
            largeRelaySphere.AddComponent<Rigidbody>().mass = 1.2f;
            PerspectiveObject largeRelayObject = largeRelaySphere.AddComponent<PerspectiveObject>();

            TextMesh relayInstruction = new GameObject("Scale relay instruction").AddComponent<TextMesh>();
            relayInstruction.transform.position = new Vector3(0, 3.2f, 41.5f);
            relayInstruction.text = "TWO SIZES. TWO PLACES.";
            relayInstruction.fontSize = 48;
            relayInstruction.characterSize = 0.055f;
            relayInstruction.anchor = TextAnchor.MiddleCenter;
            relayInstruction.alignment = TextAlignment.Center;
            relayInstruction.color = new Color(0.22f, 0.25f, 0.32f);

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

            PerspectiveFramePuzzle waitingRoomPuzzle = player.AddComponent<PerspectiveFramePuzzle>();
            SetPrivate(waitingRoomPuzzle, "playerCamera", camera);
            SetPrivate(waitingRoomPuzzle, "puzzleObject", waitingPerspectiveObject);
            SetPrivate(waitingRoomPuzzle, "target", waitingTarget.transform);
            SetPrivate(waitingRoomPuzzle, "linkedDoor", waitingRoomDoorController);

            DualScaleRelayPuzzle relayPuzzle = player.AddComponent<DualScaleRelayPuzzle>();
            SetPrivate(relayPuzzle, "smallObject", smallRelayObject);
            SetPrivate(relayPuzzle, "smallTarget", smallRelayTarget.transform);
            SetPrivate(relayPuzzle, "largeObject", largeRelayObject);
            SetPrivate(relayPuzzle, "largeTarget", largeRelayTarget.transform);
            SetPrivate(relayPuzzle, "linkedDoor", relayExitController);

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
            noticeText.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            noticeText.text = "BASMA / DO NOT\nKIRMIZI / RED";
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
            SetPrivate(corridorBeatTrigger, "dialogue", director);
            SetPrivate(corridorBeatTrigger, "revealPanel", corridorEndPanel.transform);
            SetPrivate(goalZone, "dialogue", director);
            SetPrivate(doorController, "dialogue", director);
            SetPrivate(waitingRoomDoorController, "dialogue", director);
            SetPrivate(relayExitController, "dialogue", director);
            SetPrivate(relayPuzzle, "dialogue", director);
            SetPrivate(waitingRoomPuzzle, "dialogue", director);
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

            ChapterIntroDirector chapterIntro = canvas.gameObject.AddComponent<ChapterIntroDirector>();
            SetPrivate(chapterIntro, "movement", movement);
            SetPrivate(chapterIntro, "interaction", interaction);
            SetPrivate(chapterIntro, "behavior", tracker);
            SetPrivate(chapterIntro, "dialogue", director);

            Button start = MakeButton(menu.transform, "START", new Vector2(0.35f, 0.34f), new Vector2(0.65f, 0.43f));
            start.onClick.AddListener(() => {
                menu.SetActive(false);
                chapterIntro.Begin();
            });

            Button language = MakeButton(menu.transform, "LANGUAGE: EN / TR", new Vector2(0.35f, 0.22f), new Vector2(0.65f, 0.31f));
            language.onClick.AddListener(() => {
                bool currentlyEnglish = PlayerPrefs.GetInt("oddly.language", 1) == 1;
                director.SetLanguage(currentlyEnglish ? SystemLanguage.Turkish : SystemLanguage.English);
                language.GetComponentInChildren<Text>().text = currentlyEnglish ? "DİL: TÜRKÇE" : "LANGUAGE: ENGLISH";
            });

            GameObject optionsPanel = new GameObject("Options Panel");
            optionsPanel.transform.SetParent(canvas.transform, false);
            Image optionsBackground = optionsPanel.AddComponent<Image>();
            optionsBackground.color = new Color(0.10f, 0.12f, 0.18f, 0.98f);
            SetRect(optionsPanel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
            optionsPanel.SetActive(false);

            Text optionsTitle = MakeText(optionsPanel.transform, "Options Title", "OPTIONS / AYARLAR", 36, TextAnchor.MiddleCenter);
            SetRect(optionsTitle.rectTransform, new Vector2(0.15f, 0.72f), new Vector2(0.85f, 0.84f));
            Text sensitivityLabel = MakeText(optionsPanel.transform, "Sensitivity Label", "MOUSE SENSITIVITY / FARE HASSASİYETİ", 18, TextAnchor.MiddleCenter);
            SetRect(sensitivityLabel.rectTransform, new Vector2(0.18f, 0.57f), new Vector2(0.82f, 0.66f));
            Text sensitivityValue = MakeText(optionsPanel.transform, "Sensitivity Value", "2.0", 22, TextAnchor.MiddleCenter);
            SetRect(sensitivityValue.rectTransform, new Vector2(0.43f, 0.47f), new Vector2(0.57f, 0.56f));
            Text fovLabel = MakeText(optionsPanel.transform, "FOV Label", "FIELD OF VIEW / GÖRÜŞ AÇISI", 18, TextAnchor.MiddleCenter);
            SetRect(fovLabel.rectTransform, new Vector2(0.18f, 0.34f), new Vector2(0.82f, 0.43f));
            Text fovValue = MakeText(optionsPanel.transform, "FOV Value", "78", 22, TextAnchor.MiddleCenter);
            SetRect(fovValue.rectTransform, new Vector2(0.43f, 0.24f), new Vector2(0.57f, 0.33f));

            OptionsMenuController optionsController = canvas.gameObject.AddComponent<OptionsMenuController>();
            SetPrivate(optionsController, "titlePanel", menu);
            SetPrivate(optionsController, "optionsPanel", optionsPanel);
            SetPrivate(optionsController, "movement", movement);
            SetPrivate(optionsController, "playerCamera", camera);
            SetPrivate(optionsController, "sensitivityValue", sensitivityValue);
            SetPrivate(optionsController, "fieldOfViewValue", fovValue);

            Button sensitivityDown = MakeButton(optionsPanel.transform, "−", new Vector2(0.28f, 0.47f), new Vector2(0.39f, 0.56f));
            sensitivityDown.onClick.AddListener(() => optionsController.AdjustSensitivity(-0.2f));
            Button sensitivityUp = MakeButton(optionsPanel.transform, "+", new Vector2(0.61f, 0.47f), new Vector2(0.72f, 0.56f));
            sensitivityUp.onClick.AddListener(() => optionsController.AdjustSensitivity(0.2f));
            Button fovDown = MakeButton(optionsPanel.transform, "−", new Vector2(0.28f, 0.24f), new Vector2(0.39f, 0.33f));
            fovDown.onClick.AddListener(() => optionsController.AdjustFieldOfView(-2f));
            Button fovUp = MakeButton(optionsPanel.transform, "+", new Vector2(0.61f, 0.24f), new Vector2(0.72f, 0.33f));
            fovUp.onClick.AddListener(() => optionsController.AdjustFieldOfView(2f));
            Button back = MakeButton(optionsPanel.transform, "BACK / GERİ", new Vector2(0.35f, 0.08f), new Vector2(0.65f, 0.17f));
            back.onClick.AddListener(optionsController.Close);

            Button options = MakeButton(menu.transform, "OPTIONS / AYARLAR", new Vector2(0.35f, 0.10f), new Vector2(0.65f, 0.19f));
            options.onClick.AddListener(optionsController.Open);
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
