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

        [MenuItem("Oddly/Rebuild Prototype Scene")]
        public static void RebuildPrototypeScene()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            Directory.CreateDirectory("Assets/Scenes");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            BuildScene();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuild();
            Debug.Log("ODDLY: Prototype scene rebuilt from the latest gameplay scripts.");
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
            FallRecovery fallRecovery = player.AddComponent<FallRecovery>();

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
            relayInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            relayInstruction.text = "TWO SIZES / İKİ BOYUT\nTWO PLACES / İKİ YER";
            TextMesh smallPadLabel = new GameObject("Small pad label").AddComponent<TextMesh>();
            smallPadLabel.transform.position = new Vector3(smallPadPosition.x, 1.65f, smallPadPosition.z);
            smallPadLabel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            smallPadLabel.text = "SMALL / KÜÇÜK";
            smallPadLabel.fontSize = 34;
            smallPadLabel.characterSize = 0.04f;
            smallPadLabel.anchor = TextAnchor.MiddleCenter;
            smallPadLabel.alignment = TextAlignment.Center;
            smallPadLabel.color = new Color(0.22f, 0.25f, 0.32f);
            TextMesh largePadLabel = new GameObject("Large pad label").AddComponent<TextMesh>();
            largePadLabel.transform.position = new Vector3(largePadPosition.x, 1.65f, largePadPosition.z);
            largePadLabel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            largePadLabel.text = "LARGE / BÜYÜK";
            largePadLabel.fontSize = 34;
            largePadLabel.characterSize = 0.04f;
            largePadLabel.anchor = TextAnchor.MiddleCenter;
            largePadLabel.alignment = TextAlignment.Center;
            largePadLabel.color = new Color(0.22f, 0.25f, 0.32f);
            relayInstruction.fontSize = 48;
            relayInstruction.characterSize = 0.055f;
            relayInstruction.anchor = TextAnchor.MiddleCenter;
            relayInstruction.alignment = TextAlignment.Center;
            relayInstruction.color = new Color(0.22f, 0.25f, 0.32f);

            // A new chamber turns the next exit into a literal forced-perspective
            // sightline: separated fragments resolve into one symbol from a marked spot.
            CreatePrimitive("Sightline chamber floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 67), new Vector3(10, 0.5f, 20), floorMat);
            CreatePrimitive("Sightline chamber left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 67), new Vector3(0.25f, 5, 20), wallMat);
            CreatePrimitive("Sightline chamber right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 67), new Vector3(0.25f, 5, 20), wallMat);
            CreatePrimitive("Sightline far wall left", PrimitiveType.Cube,
                new Vector3(-4.2f, 2.5f, 77), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Sightline far wall right", PrimitiveType.Cube,
                new Vector3(4.2f, 2.5f, 77), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Sightline far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.65f, 77), new Vector3(6.8f, 0.7f, 0.5f), wallMat);
            GameObject sightlineExit = CreatePrimitive("Sightline chamber exit",
                PrimitiveType.Cube, new Vector3(0, 2, 76.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController sightlineExitController = sightlineExit.AddComponent<DoorController>();

            Vector3 viewpointPosition = new Vector3(-2f, 0.08f, 61f);
            GameObject viewpointMarker = CreatePrimitive("Correct viewpoint marker",
                PrimitiveType.Cylinder, viewpointPosition, new Vector3(1.35f, 0.08f, 1.35f), accentMat);
            GameObject sightlineTarget = new GameObject("Sightline target");
            sightlineTarget.transform.position = new Vector3(-0.125f, 1.65f, 76f);

            // Two pieces occupy different depths but share the same viewing ray.
            // From the marker they overlap into a single impossible doorway glyph.
            GameObject nearSightlineFragment = CreatePrimitive("Near sightline fragment", PrimitiveType.Cube,
                new Vector3(-1.5f, 1.65f, 65f), new Vector3(0.32f, 1.65f, 0.18f), accentMat);
            GameObject farSightlineFragment = CreatePrimitive("Far sightline fragment", PrimitiveType.Cube,
                new Vector3(-0.5f, 1.65f, 73f), new Vector3(0.32f, 1.65f, 0.18f), cubeMat);
            TextMesh sightlineInstruction = new GameObject("Sightline instruction").AddComponent<TextMesh>();
            sightlineInstruction.transform.position = new Vector3(0, 3.35f, 62.5f);
            sightlineInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            sightlineInstruction.text = "ONE VIEW / TEK BAKIŞ\nFIND THE MARK / İŞARETİ BUL";
            sightlineInstruction.fontSize = 42;
            sightlineInstruction.characterSize = 0.05f;
            sightlineInstruction.anchor = TextAnchor.MiddleCenter;
            sightlineInstruction.alignment = TextAlignment.Center;
            sightlineInstruction.color = new Color(0.22f, 0.25f, 0.32f);

            // Chapter 2 begins immediately beyond the forced-perspective exit.
            // A real floor gap makes the resized plank a physical bridge, not a
            // decorative alignment check.
            CreatePrimitive("Chapter 2 near platform", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 83.5f), new Vector3(10, 0.5f, 13), floorMat);
            CreatePrimitive("Chapter 2 far platform", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 101.5f), new Vector3(10, 0.5f, 15), floorMat);
            CreatePrimitive("Chapter 2 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 93), new Vector3(0.25f, 5, 32), wallMat);
            CreatePrimitive("Chapter 2 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 93), new Vector3(0.25f, 5, 32), wallMat);
            CreatePrimitive("Chapter 2 far wall left", PrimitiveType.Cube,
                new Vector3(-4.2f, 2.5f, 109), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 2 far wall right", PrimitiveType.Cube,
                new Vector3(4.2f, 2.5f, 109), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 2 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.65f, 109), new Vector3(6.8f, 0.7f, 0.5f), wallMat);
            GameObject chapterTwoExit = CreatePrimitive("Chapter 2 bridge exit",
                PrimitiveType.Cube, new Vector3(0, 2, 108.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterTwoExitController = chapterTwoExit.AddComponent<DoorController>();

            // A short reveal bay beyond the bridge keeps the unlocked door from
            // opening into unfinished space and previews the next impossible test.
            CreatePrimitive("Chapter 2 reveal bay floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 115), new Vector3(10, 0.5f, 12), floorMat);
            CreatePrimitive("Chapter 2 reveal bay left", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 115), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Chapter 2 reveal bay right", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 115), new Vector3(0.25f, 5, 12), wallMat);
            // Chapter 3: the player cannot fit through the wall's narrow slot,
            // but a correctly resized object can. This changes the puzzle verb
            // from crossing a gap to sending an object into another dimension.
            CreatePrimitive("Chapter 2 reveal bay end wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 121), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 2 reveal bay end wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 121), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 2 reveal bay end wall header", PrimitiveType.Cube,
                new Vector3(0, 4.45f, 121), new Vector3(6.6f, 1.1f, 0.5f), wallMat);

            CreatePrimitive("Chapter 3 dimension chamber floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 133), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 3 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 133), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 3 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 133), new Vector3(0.25f, 5, 24), wallMat);
            // Slot opening: 1.3m wide and 0.9m high, too small for the player.
            CreatePrimitive("Dimension wall left", PrimitiveType.Cube,
                new Vector3(-2.825f, 2.5f, 132.5f), new Vector3(4.35f, 5, 0.5f), wallMat);
            CreatePrimitive("Dimension wall right", PrimitiveType.Cube,
                new Vector3(2.825f, 2.5f, 132.5f), new Vector3(4.35f, 5, 0.5f), wallMat);
            CreatePrimitive("Dimension slot lower sill", PrimitiveType.Cube,
                new Vector3(0, 0.35f, 132.5f), new Vector3(1.3f, 0.7f, 0.5f), accentMat);
            CreatePrimitive("Dimension slot upper lintel", PrimitiveType.Cube,
                new Vector3(0, 3.3f, 132.5f), new Vector3(1.3f, 3.4f, 0.5f), accentMat);
            CreatePrimitive("Chapter 3 far wall left", PrimitiveType.Cube,
                new Vector3(-4.2f, 2.5f, 145), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 3 far wall right", PrimitiveType.Cube,
                new Vector3(4.2f, 2.5f, 145), new Vector3(1.6f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 3 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.65f, 145), new Vector3(6.8f, 0.7f, 0.5f), wallMat);
            GameObject chapterThreeExit = CreatePrimitive("Chapter 3 dimension exit",
                PrimitiveType.Cube, new Vector3(0, 2, 144.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterThreeExitController = chapterThreeExit.AddComponent<DoorController>();

            GameObject dimensionObject = CreatePrimitive("Dimension slot cube",
                PrimitiveType.Cube, new Vector3(0, 0.62f, 126), Vector3.one, cubeMat);
            dimensionObject.AddComponent<Rigidbody>().mass = 1.1f;
            PerspectiveObject dimensionPerspectiveObject = dimensionObject.AddComponent<PerspectiveObject>();
            GameObject dimensionLandingTarget = new GameObject("Dimension landing target");
            dimensionLandingTarget.transform.position = new Vector3(0, 0.5f, 136.1f);

            TextMesh dimensionInstruction = new GameObject("Dimension slot instruction").AddComponent<TextMesh>();
            dimensionInstruction.transform.position = new Vector3(0, 3.25f, 125f);
            dimensionInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            dimensionInstruction.text = "YOU DON'T FIT / SEN SIĞMAZSIN";
            dimensionInstruction.fontSize = 42;
            dimensionInstruction.characterSize = 0.05f;
            dimensionInstruction.anchor = TextAnchor.MiddleCenter;
            dimensionInstruction.alignment = TextAlignment.Center;
            dimensionInstruction.color = new Color(0.22f, 0.25f, 0.32f);

            GameObject chapterThreeEntry = new GameObject("Chapter 3 entry trigger");
            chapterThreeEntry.transform.position = new Vector3(0, 1.2f, 122.2f);
            BoxCollider chapterThreeEntryCollider = chapterThreeEntry.AddComponent<BoxCollider>();
            chapterThreeEntryCollider.isTrigger = true;
            chapterThreeEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterThreeEntryTrigger = chapterThreeEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterThreeCheckpoint = new GameObject("Chapter 3 checkpoint");
            chapterThreeCheckpoint.transform.position = new Vector3(0f, 0.15f, 123.5f);
            TextMesh chapterThreeSign = new GameObject("Chapter 3 title sign").AddComponent<TextMesh>();
            chapterThreeSign.transform.position = new Vector3(0, 3.65f, 124f);
            chapterThreeSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterThreeSign.text = "CHAPTER 03 / WRONG DIMENSION";
            chapterThreeSign.fontSize = 42;
            chapterThreeSign.characterSize = 0.055f;
            chapterThreeSign.anchor = TextAnchor.MiddleCenter;
            chapterThreeSign.alignment = TextAlignment.Center;
            chapterThreeSign.color = new Color(0.22f, 0.25f, 0.32f);

            // Chapter 4: two voices give contradictory switch orders. The room
            // deliberately makes the disagreement part of the puzzle language.
            CreatePrimitive("Chapter 4 system error floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 159), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 4 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 159), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 4 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 159), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 4 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 171), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 4 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 171), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 4 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 171), new Vector3(6.6f, 0.9f, 0.5f), wallMat);

            GameObject chapterFourExit = CreatePrimitive("Chapter 4 system error exit",
                PrimitiveType.Cube, new Vector3(0, 2, 170.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterFourExitController = chapterFourExit.AddComponent<DoorController>();

            GameObject chapterFourEntry = new GameObject("Chapter 4 entry trigger");
            chapterFourEntry.transform.position = new Vector3(0, 1.2f, 147.2f);
            BoxCollider chapterFourEntryCollider = chapterFourEntry.AddComponent<BoxCollider>();
            chapterFourEntryCollider.isTrigger = true;
            chapterFourEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterFourEntryTrigger = chapterFourEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterFourCheckpoint = new GameObject("Chapter 4 checkpoint");
            chapterFourCheckpoint.transform.position = new Vector3(0f, 0.15f, 148.5f);
            TextMesh chapterFourSign = new GameObject("Chapter 4 title sign").AddComponent<TextMesh>();
            chapterFourSign.transform.position = new Vector3(0, 3.65f, 150f);
            chapterFourSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterFourSign.text = "CHAPTER 04 / SYSTEM ERROR";
            chapterFourSign.fontSize = 42;
            chapterFourSign.characterSize = 0.055f;
            chapterFourSign.anchor = TextAnchor.MiddleCenter;
            chapterFourSign.alignment = TextAlignment.Center;
            chapterFourSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh narratorInstruction = new GameObject("Narrator instruction").AddComponent<TextMesh>();
            narratorInstruction.transform.position = new Vector3(-2.2f, 3.1f, 157f);
            narratorInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            narratorInstruction.text = "NARRATOR: LEFT / RIGHT / LEFT";
            narratorInstruction.fontSize = 30;
            narratorInstruction.characterSize = 0.045f;
            narratorInstruction.anchor = TextAnchor.MiddleCenter;
            narratorInstruction.alignment = TextAlignment.Center;
            narratorInstruction.color = new Color(0.82f, 0.28f, 0.42f);

            TextMesh systemInstruction = new GameObject("System instruction").AddComponent<TextMesh>();
            systemInstruction.transform.position = new Vector3(2.2f, 3.1f, 157f);
            systemInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            systemInstruction.text = "SYSTEM-02: RIGHT / LEFT / RIGHT";
            systemInstruction.fontSize = 30;
            systemInstruction.characterSize = 0.045f;
            systemInstruction.anchor = TextAnchor.MiddleCenter;
            systemInstruction.alignment = TextAlignment.Center;
            systemInstruction.color = new Color(0.20f, 0.42f, 0.78f);

            GameObject systemErrorRoot = new GameObject("System Error puzzle controller");
            SystemErrorPuzzle systemErrorPuzzle = systemErrorRoot.AddComponent<SystemErrorPuzzle>();
            SetPrivate(systemErrorPuzzle, "linkedDoor", chapterFourExitController);

            GameObject narratorSwitch = CreatePrimitive("Narrator switch", PrimitiveType.Cube,
                new Vector3(-2.2f, 0.62f, 163f), new Vector3(1.25f, 0.42f, 1.25f), accentMat);
            MetaPuzzleNode narratorSwitchNode = narratorSwitch.AddComponent<MetaPuzzleNode>();
            SetPrivate(narratorSwitchNode, "action", MetaPuzzleAction.PressNarratorSwitch);
            SetPrivate(narratorSwitchNode, "systemErrorPuzzle", systemErrorPuzzle);

            GameObject systemSwitch = CreatePrimitive("SYSTEM-02 switch", PrimitiveType.Cube,
                new Vector3(2.2f, 0.62f, 163f), new Vector3(1.25f, 0.42f, 1.25f), doorMat);
            MetaPuzzleNode systemSwitchNode = systemSwitch.AddComponent<MetaPuzzleNode>();
            SetPrivate(systemSwitchNode, "action", MetaPuzzleAction.PressSystemSwitch);
            SetPrivate(systemSwitchNode, "systemErrorPuzzle", systemErrorPuzzle);

            TextMesh sequenceHint = new GameObject("Corrupted sequence hint").AddComponent<TextMesh>();
            sequenceHint.transform.position = new Vector3(0, 2.65f, 166f);
            sequenceHint.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            sequenceHint.text = "RECORDED: LEFT / RIGHT / LEFT";
            sequenceHint.fontSize = 34;
            sequenceHint.characterSize = 0.05f;
            sequenceHint.anchor = TextAnchor.MiddleCenter;
            sequenceHint.alignment = TextAlignment.Center;
            sequenceHint.color = new Color(0.22f, 0.25f, 0.32f);

            // Chapter 5: the exit is behind the player. Looking directly at it
            // keeps it locked; the player must stand on the floor mark and turn away.
            CreatePrimitive("Chapter 5 blind spot floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 183), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 5 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 183), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 5 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 183), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 5 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 195), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 5 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 195), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 5 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 195), new Vector3(6.6f, 0.9f, 0.5f), wallMat);

            GameObject chapterFiveExit = CreatePrimitive("Chapter 5 hidden exit",
                PrimitiveType.Cube, new Vector3(0, 2, 194.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterFiveExitController = chapterFiveExit.AddComponent<DoorController>();

            GameObject chapterFiveEntry = new GameObject("Chapter 5 entry trigger");
            chapterFiveEntry.transform.position = new Vector3(0, 1.2f, 172.5f);
            BoxCollider chapterFiveEntryCollider = chapterFiveEntry.AddComponent<BoxCollider>();
            chapterFiveEntryCollider.isTrigger = true;
            chapterFiveEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterFiveEntryTrigger = chapterFiveEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterFiveCheckpoint = new GameObject("Chapter 5 checkpoint");
            chapterFiveCheckpoint.transform.position = new Vector3(0f, 0.15f, 174.5f);

            TextMesh chapterFiveSign = new GameObject("Chapter 5 title sign").AddComponent<TextMesh>();
            chapterFiveSign.transform.position = new Vector3(0, 3.65f, 176f);
            chapterFiveSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterFiveSign.text = "CHAPTER 05 / DON'T TRUST YOUR EYES";
            chapterFiveSign.fontSize = 38;
            chapterFiveSign.characterSize = 0.05f;
            chapterFiveSign.anchor = TextAnchor.MiddleCenter;
            chapterFiveSign.alignment = TextAlignment.Center;
            chapterFiveSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh blindSpotInstruction = new GameObject("Blind spot instruction").AddComponent<TextMesh>();
            blindSpotInstruction.transform.position = new Vector3(0, 2.8f, 179f);
            blindSpotInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            blindSpotInstruction.text = "THE EXIT IS BEHIND YOU / ÇIKIŞ ARKANDA";
            blindSpotInstruction.fontSize = 34;
            blindSpotInstruction.characterSize = 0.05f;
            blindSpotInstruction.anchor = TextAnchor.MiddleCenter;
            blindSpotInstruction.alignment = TextAlignment.Center;
            blindSpotInstruction.color = new Color(0.82f, 0.28f, 0.42f);

            GameObject blindSpotMarker = CreatePrimitive("Blind spot floor marker",
                PrimitiveType.Cylinder, new Vector3(0, 0.015f, 183f), new Vector3(1.35f, 0.025f, 1.35f), accentMat);
            GameObject blindSpotExitDirection = new GameObject("Blind spot exit direction");
            blindSpotExitDirection.transform.position = new Vector3(0, 1.5f, 194.7f);

            // Chapter 6: the facility has counted the player's habits and
            // asks them to identify the action they performed most often.
            CreatePrimitive("Chapter 6 memory floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 207), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 6 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 207), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 6 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 207), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 6 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 219), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 6 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 219), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 6 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 219), new Vector3(6.6f, 0.9f, 0.5f), wallMat);

            GameObject chapterSixExit = CreatePrimitive("Chapter 6 memory exit",
                PrimitiveType.Cube, new Vector3(0, 2, 218.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterSixExitController = chapterSixExit.AddComponent<DoorController>();

            GameObject chapterSixEntry = new GameObject("Chapter 6 entry trigger");
            chapterSixEntry.transform.position = new Vector3(0, 1.2f, 196.5f);
            BoxCollider chapterSixEntryCollider = chapterSixEntry.AddComponent<BoxCollider>();
            chapterSixEntryCollider.isTrigger = true;
            chapterSixEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterSixEntryTrigger = chapterSixEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterSixCheckpoint = new GameObject("Chapter 6 checkpoint");
            chapterSixCheckpoint.transform.position = new Vector3(0f, 0.15f, 198.5f);

            TextMesh chapterSixSign = new GameObject("Chapter 6 title sign").AddComponent<TextMesh>();
            chapterSixSign.transform.position = new Vector3(0, 3.65f, 200f);
            chapterSixSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterSixSign.text = "CHAPTER 06 / YOU AGAIN?";
            chapterSixSign.fontSize = 42;
            chapterSixSign.characterSize = 0.055f;
            chapterSixSign.anchor = TextAnchor.MiddleCenter;
            chapterSixSign.alignment = TextAlignment.Center;
            chapterSixSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh memoryInstruction = new GameObject("Memory instruction").AddComponent<TextMesh>();
            memoryInstruction.transform.position = new Vector3(0, 3.0f, 203f);
            memoryInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            memoryInstruction.text = "WE HAVE BEEN KEEPING COUNT / SAYGIYLA TAKİP EDİYORUZ";
            memoryInstruction.fontSize = 30;
            memoryInstruction.characterSize = 0.045f;
            memoryInstruction.anchor = TextAnchor.MiddleCenter;
            memoryInstruction.alignment = TextAlignment.Center;
            memoryInstruction.color = new Color(0.22f, 0.25f, 0.32f);

            GameObject memoryRoot = new GameObject("Behavior memory puzzle controller");
            BehaviorMemoryPuzzle behaviorMemoryPuzzle = memoryRoot.AddComponent<BehaviorMemoryPuzzle>();
            DoorController chapterSixDoor = chapterSixExitController;

            TextMesh dropLabel = new GameObject("Drop habit label").AddComponent<TextMesh>();
            dropLabel.transform.position = new Vector3(-3f, 2.4f, 211f);
            dropLabel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            dropLabel.text = "DROP";
            dropLabel.fontSize = 38; dropLabel.characterSize = 0.055f;
            dropLabel.anchor = TextAnchor.MiddleCenter; dropLabel.alignment = TextAlignment.Center;
            TextMesh retryLabel = new GameObject("Retry habit label").AddComponent<TextMesh>();
            retryLabel.transform.position = new Vector3(0f, 2.4f, 211f);
            retryLabel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            retryLabel.text = "RETRY";
            retryLabel.fontSize = 38; retryLabel.characterSize = 0.055f;
            retryLabel.anchor = TextAnchor.MiddleCenter; retryLabel.alignment = TextAlignment.Center;
            TextMesh waitLabel = new GameObject("Wait habit label").AddComponent<TextMesh>();
            waitLabel.transform.position = new Vector3(3f, 2.4f, 211f);
            waitLabel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            waitLabel.text = "WAIT";
            waitLabel.fontSize = 38; waitLabel.characterSize = 0.055f;
            waitLabel.anchor = TextAnchor.MiddleCenter; waitLabel.alignment = TextAlignment.Center;

            GameObject dropChoice = CreatePrimitive("Drop habit choice", PrimitiveType.Cube,
                new Vector3(-3f, 0.65f, 211f), new Vector3(1.8f, 0.45f, 1.8f), accentMat);
            MetaPuzzleNode dropChoiceNode = dropChoice.AddComponent<MetaPuzzleNode>();
            SetPrivate(dropChoiceNode, "action", MetaPuzzleAction.ChooseHabitDrop);
            SetPrivate(dropChoiceNode, "behaviorMemoryPuzzle", behaviorMemoryPuzzle);
            GameObject retryChoice = CreatePrimitive("Retry habit choice", PrimitiveType.Cube,
                new Vector3(0f, 0.65f, 211f), new Vector3(1.8f, 0.45f, 1.8f), cubeMat);
            MetaPuzzleNode retryChoiceNode = retryChoice.AddComponent<MetaPuzzleNode>();
            SetPrivate(retryChoiceNode, "action", MetaPuzzleAction.ChooseHabitRetry);
            SetPrivate(retryChoiceNode, "behaviorMemoryPuzzle", behaviorMemoryPuzzle);
            GameObject waitChoice = CreatePrimitive("Wait habit choice", PrimitiveType.Cube,
                new Vector3(3f, 0.65f, 211f), new Vector3(1.8f, 0.45f, 1.8f), doorMat);
            MetaPuzzleNode waitChoiceNode = waitChoice.AddComponent<MetaPuzzleNode>();
            SetPrivate(waitChoiceNode, "action", MetaPuzzleAction.ChooseHabitWait);
            SetPrivate(waitChoiceNode, "behaviorMemoryPuzzle", behaviorMemoryPuzzle);

            // Short connector keeps the Chapter 6 exit physically connected to Chapter 7.
            CreatePrimitive("Chapter 6 to 7 connector floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 225), new Vector3(10, 0.5f, 12), floorMat);
            CreatePrimitive("Chapter 6 to 7 connector left rail", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 225), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Chapter 6 to 7 connector right rail", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 225), new Vector3(0.25f, 5, 12), wallMat);

            // Chapter 7: the room reacts to the player's own repeated actions.
            CreatePrimitive("Chapter 7 laughing room floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 243), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 7 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 243), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 7 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 243), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 7 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 255), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 7 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 255), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 7 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 255), new Vector3(6.6f, 0.9f, 0.5f), wallMat);
            GameObject chapterSevenExit = CreatePrimitive("Chapter 7 laughing exit",
                PrimitiveType.Cube, new Vector3(0, 2, 254.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterSevenExitController = chapterSevenExit.AddComponent<DoorController>();

            GameObject chapterSevenEntry = new GameObject("Chapter 7 entry trigger");
            chapterSevenEntry.transform.position = new Vector3(0, 1.2f, 221.5f);
            BoxCollider chapterSevenEntryCollider = chapterSevenEntry.AddComponent<BoxCollider>();
            chapterSevenEntryCollider.isTrigger = true;
            chapterSevenEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterSevenEntryTrigger = chapterSevenEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterSevenCheckpoint = new GameObject("Chapter 7 checkpoint");
            chapterSevenCheckpoint.transform.position = new Vector3(0f, 0.15f, 223.5f);

            TextMesh chapterSevenSign = new GameObject("Chapter 7 title sign").AddComponent<TextMesh>();
            chapterSevenSign.transform.position = new Vector3(0, 3.65f, 225f);
            chapterSevenSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterSevenSign.text = "CHAPTER 07 / THE ROOM THAT LAUGHS";
            chapterSevenSign.fontSize = 36;
            chapterSevenSign.characterSize = 0.05f;
            chapterSevenSign.anchor = TextAnchor.MiddleCenter;
            chapterSevenSign.alignment = TextAlignment.Center;
            chapterSevenSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh laughingInstruction = new GameObject("Laughing room instruction").AddComponent<TextMesh>();
            laughingInstruction.transform.position = new Vector3(0, 2.8f, 228f);
            laughingInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            laughingInstruction.text = "GO ON. MAKE A MISTAKE. / HAYDİ. BİR HATA YAP.";
            laughingInstruction.fontSize = 32;
            laughingInstruction.characterSize = 0.05f;
            laughingInstruction.anchor = TextAnchor.MiddleCenter;
            laughingInstruction.alignment = TextAlignment.Center;
            laughingInstruction.color = new Color(0.82f, 0.28f, 0.42f);

            GameObject prankOne = CreatePrimitive("Laughing prop one", PrimitiveType.Cube,
                new Vector3(-2.5f, 0.8f, 239f), Vector3.one * 1.2f, accentMat);
            GameObject prankTwo = CreatePrimitive("Laughing prop two", PrimitiveType.Sphere,
                new Vector3(0f, 0.8f, 239f), Vector3.one * 1.2f, cubeMat);
            GameObject prankThree = CreatePrimitive("Laughing prop three", PrimitiveType.Capsule,
                new Vector3(2.5f, 0.8f, 239f), Vector3.one * 1.2f, doorMat);
            GameObject movingPrankWall = CreatePrimitive("Laughing room shifting panel",
                PrimitiveType.Cube, new Vector3(-4.4f, 2.1f, 243f), new Vector3(0.18f, 4.2f, 7f), accentMat);

            GameObject laughLightObject = new GameObject("Laughing room reactive light");
            laughLightObject.transform.position = new Vector3(0, 4.2f, 241f);
            Light laughLight = laughLightObject.AddComponent<Light>();
            laughLight.type = LightType.Point;
            laughLight.range = 15f;
            laughLight.intensity = 1.1f;

            GameObject laughingRoomTrigger = new GameObject("Laughing room behavior trigger");
            laughingRoomTrigger.transform.position = new Vector3(0, 2f, 243f);
            BoxCollider laughingRoomCollider = laughingRoomTrigger.AddComponent<BoxCollider>();
            laughingRoomCollider.isTrigger = true;
            laughingRoomCollider.size = new Vector3(9f, 4f, 22f);
            LaughingRoomDirector laughingRoomDirector = laughingRoomTrigger.AddComponent<LaughingRoomDirector>();

            // Connector between the laughing room and the loop corridor.
            CreatePrimitive("Chapter 7 to 8 connector floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 261), new Vector3(10, 0.5f, 12), floorMat);
            CreatePrimitive("Chapter 7 to 8 connector left rail", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 261), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Chapter 7 to 8 connector right rail", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 261), new Vector3(0.25f, 5, 12), wallMat);

            // Chapter 8: each apparent exit returns the player to a subtly altered room.
            CreatePrimitive("Chapter 8 loop floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 279), new Vector3(10, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 8 left wall", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 279), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 8 right wall", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 279), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 8 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 291), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 8 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 291), new Vector3(1.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 8 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 291), new Vector3(6.6f, 0.9f, 0.5f), wallMat);
            GameObject chapterEightExit = CreatePrimitive("Chapter 8 loop exit",
                PrimitiveType.Cube, new Vector3(0, 2, 290.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterEightExitController = chapterEightExit.AddComponent<DoorController>();

            GameObject chapterEightEntry = new GameObject("Chapter 8 entry trigger");
            chapterEightEntry.transform.position = new Vector3(0, 1.2f, 265.5f);
            BoxCollider chapterEightEntryCollider = chapterEightEntry.AddComponent<BoxCollider>();
            chapterEightEntryCollider.isTrigger = true;
            chapterEightEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterEightEntryTrigger = chapterEightEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterEightCheckpoint = new GameObject("Chapter 8 checkpoint");
            chapterEightCheckpoint.transform.position = new Vector3(0f, 0.15f, 267.5f);

            TextMesh chapterEightSign = new GameObject("Chapter 8 title sign").AddComponent<TextMesh>();
            chapterEightSign.transform.position = new Vector3(0, 3.65f, 269f);
            chapterEightSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterEightSign.text = "CHAPTER 08 / THE LOOP";
            chapterEightSign.fontSize = 42;
            chapterEightSign.characterSize = 0.055f;
            chapterEightSign.anchor = TextAnchor.MiddleCenter;
            chapterEightSign.alignment = TextAlignment.Center;
            chapterEightSign.color = new Color(0.22f, 0.25f, 0.32f);

            TextMesh loopSign = new GameObject("Loop counter sign").AddComponent<TextMesh>();
            loopSign.transform.position = new Vector3(0, 2.9f, 273f);
            loopSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            loopSign.fontSize = 34;
            loopSign.characterSize = 0.05f;
            loopSign.anchor = TextAnchor.MiddleCenter;
            loopSign.alignment = TextAlignment.Center;
            loopSign.color = new Color(0.82f, 0.28f, 0.42f);

            GameObject loopPropOne = CreatePrimitive("Loop prop one", PrimitiveType.Cube,
                new Vector3(-2.5f, 0.8f, 278f), Vector3.one * 1.1f, accentMat);
            GameObject loopPropTwo = CreatePrimitive("Loop prop two", PrimitiveType.Sphere,
                new Vector3(0f, 0.8f, 278f), Vector3.one * 1.1f, cubeMat);
            GameObject loopPropThree = CreatePrimitive("Loop prop three", PrimitiveType.Capsule,
                new Vector3(2.5f, 0.8f, 278f), Vector3.one * 1.1f, doorMat);

            GameObject loopLightObject = new GameObject("Loop room reactive light");
            loopLightObject.transform.position = new Vector3(0, 4.2f, 280f);
            Light loopLight = loopLightObject.AddComponent<Light>();
            loopLight.type = LightType.Point;
            loopLight.range = 16f;
            loopLight.intensity = 0.9f;

            GameObject loopReturnPoint = new GameObject("Loop return point");
            loopReturnPoint.transform.position = new Vector3(0f, 0.1f, 269.5f);
            loopReturnPoint.transform.rotation = Quaternion.identity;

            GameObject loopTrigger = new GameObject("Loop traversal trigger");
            loopTrigger.transform.position = new Vector3(0f, 1.5f, 283f);
            BoxCollider loopTriggerCollider = loopTrigger.AddComponent<BoxCollider>();
            loopTriggerCollider.isTrigger = true;
            loopTriggerCollider.size = new Vector3(8.8f, 3f, 0.8f);
            LoopRoomDirector loopRoomDirector = loopTrigger.AddComponent<LoopRoomDirector>();

            // Chapter 9: a stage that rewards the player for refusing the audience.
            CreatePrimitive("Chapter 9 theatre floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 315), new Vector3(12, 0.5f, 24), floorMat);
            CreatePrimitive("Chapter 9 left wall", PrimitiveType.Cube,
                new Vector3(-6, 2.5f, 315), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 9 right wall", PrimitiveType.Cube,
                new Vector3(6, 2.5f, 315), new Vector3(0.25f, 5, 24), wallMat);
            CreatePrimitive("Chapter 9 far wall left", PrimitiveType.Cube,
                new Vector3(-4.15f, 2.5f, 327), new Vector3(3.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 9 far wall right", PrimitiveType.Cube,
                new Vector3(4.15f, 2.5f, 327), new Vector3(3.7f, 5, 0.5f), wallMat);
            CreatePrimitive("Chapter 9 far wall header", PrimitiveType.Cube,
                new Vector3(0, 4.55f, 327), new Vector3(4.6f, 0.9f, 0.5f), wallMat);
            GameObject chapterNineExit = CreatePrimitive("Chapter 9 theatre exit",
                PrimitiveType.Cube, new Vector3(0, 2, 326.7f), new Vector3(2.8f, 4, 0.35f), doorMat);
            DoorController chapterNineExitController = chapterNineExit.AddComponent<DoorController>();

            GameObject chapterNineEntry = new GameObject("Chapter 9 entry trigger");
            chapterNineEntry.transform.position = new Vector3(0, 1.2f, 303.5f);
            BoxCollider chapterNineEntryCollider = chapterNineEntry.AddComponent<BoxCollider>();
            chapterNineEntryCollider.isTrigger = true;
            chapterNineEntryCollider.size = new Vector3(10f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterNineEntryTrigger = chapterNineEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterNineCheckpoint = new GameObject("Chapter 9 checkpoint");
            chapterNineCheckpoint.transform.position = new Vector3(0f, 0.15f, 305.5f);

            TextMesh chapterNineSign = new GameObject("Chapter 9 title sign").AddComponent<TextMesh>();
            chapterNineSign.transform.position = new Vector3(0, 3.65f, 307f);
            chapterNineSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterNineSign.text = "CHAPTER 09 / THE WRONG AUDIENCE";
            chapterNineSign.fontSize = 38;
            chapterNineSign.characterSize = 0.05f;
            chapterNineSign.anchor = TextAnchor.MiddleCenter;
            chapterNineSign.alignment = TextAlignment.Center;
            chapterNineSign.color = new Color(0.22f, 0.25f, 0.32f);

            CreatePrimitive("Theatre stage platform", PrimitiveType.Cube,
                new Vector3(0, 0.12f, 316f), new Vector3(5.5f, 0.24f, 3.5f), accentMat);
            TextMesh audienceStageSign = new GameObject("Audience stage instruction").AddComponent<TextMesh>();
            audienceStageSign.transform.position = new Vector3(0, 2.7f, 312.5f);
            audienceStageSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            audienceStageSign.fontSize = 30;
            audienceStageSign.characterSize = 0.05f;
            audienceStageSign.anchor = TextAnchor.MiddleCenter;
            audienceStageSign.alignment = TextAlignment.Center;
            audienceStageSign.color = new Color(0.82f, 0.28f, 0.42f);

            GameObject audienceOne = CreatePrimitive("Audience member one", PrimitiveType.Capsule,
                new Vector3(-3f, 1.1f, 322f), new Vector3(0.9f, 2.2f, 0.9f), cubeMat);
            GameObject audienceTwo = CreatePrimitive("Audience member two", PrimitiveType.Capsule,
                new Vector3(0f, 1.1f, 322f), new Vector3(0.9f, 2.2f, 0.9f), accentMat);
            GameObject audienceThree = CreatePrimitive("Audience member three", PrimitiveType.Capsule,
                new Vector3(3f, 1.1f, 322f), new Vector3(0.9f, 2.2f, 0.9f), doorMat);

            GameObject stageLightObject = new GameObject("Audience stage spotlight");
            stageLightObject.transform.position = new Vector3(0, 4.2f, 316f);
            Light stageLight = stageLightObject.AddComponent<Light>();
            stageLight.type = LightType.Spot;
            stageLight.range = 13f;
            stageLight.spotAngle = 62f;
            stageLight.intensity = 2f;

            GameObject audienceStageTrigger = new GameObject("Wrong audience stage trigger");
            audienceStageTrigger.transform.position = new Vector3(0f, 1.5f, 316f);
            BoxCollider audienceStageCollider = audienceStageTrigger.AddComponent<BoxCollider>();
            audienceStageCollider.isTrigger = true;
            audienceStageCollider.size = new Vector3(5.5f, 3f, 4f);
            WrongAudiencePuzzle audiencePuzzle = audienceStageTrigger.AddComponent<WrongAudiencePuzzle>();

            // Connector from the loop corridor into the theatre.
            CreatePrimitive("Chapter 8 to 9 connector floor", PrimitiveType.Cube,
                new Vector3(0, -0.25f, 297), new Vector3(10, 0.5f, 12), floorMat);
            CreatePrimitive("Chapter 8 to 9 connector left rail", PrimitiveType.Cube,
                new Vector3(-5, 2.5f, 297), new Vector3(0.25f, 5, 12), wallMat);
            CreatePrimitive("Chapter 8 to 9 connector right rail", PrimitiveType.Cube,
                new Vector3(5, 2.5f, 297), new Vector3(0.25f, 5, 12), wallMat);

            GameObject chapterTwoEntry = new GameObject("Chapter 2 entry trigger");
            chapterTwoEntry.transform.position = new Vector3(0, 1.2f, 79.5f);
            BoxCollider chapterTwoEntryCollider = chapterTwoEntry.AddComponent<BoxCollider>();
            chapterTwoEntryCollider.isTrigger = true;
            chapterTwoEntryCollider.size = new Vector3(8.5f, 2.4f, 1.2f);
            ChapterEntryTrigger chapterTwoEntryTrigger = chapterTwoEntry.AddComponent<ChapterEntryTrigger>();
            GameObject chapterTwoCheckpoint = new GameObject("Chapter 2 checkpoint");
            chapterTwoCheckpoint.transform.position = new Vector3(0f, 0.15f, 81.5f);

            TextMesh chapterTwoSign = new GameObject("Chapter 2 title sign").AddComponent<TextMesh>();
            chapterTwoSign.transform.position = new Vector3(0, 3.65f, 82f);
            chapterTwoSign.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            chapterTwoSign.text = "CHAPTER 02 / SIZE DOES MATTER";
            chapterTwoSign.fontSize = 42;
            chapterTwoSign.characterSize = 0.055f;
            chapterTwoSign.anchor = TextAnchor.MiddleCenter;
            chapterTwoSign.alignment = TextAlignment.Center;
            chapterTwoSign.color = new Color(0.22f, 0.25f, 0.32f);

            CreatePrimitive("Bridge plank pedestal",
                PrimitiveType.Cube, new Vector3(0, 0.25f, 85f), new Vector3(2.8f, 0.5f, 2.6f), accentMat);
            GameObject bridgePlank = CreatePrimitive("Perspective bridge plank",
                PrimitiveType.Cube, new Vector3(0, 0.62f, 85f), new Vector3(1f, 0.1f, 1.8f), cubeMat);
            Rigidbody bridgeBody = bridgePlank.AddComponent<Rigidbody>();
            bridgeBody.mass = 2.4f;
            bridgeBody.interpolation = RigidbodyInterpolation.Interpolate;
            PerspectiveObject bridgePerspectiveObject = bridgePlank.AddComponent<PerspectiveObject>();

            GameObject bridgeTarget = new GameObject("Bridge landing target");
            bridgeTarget.transform.position = new Vector3(0, 0.1275f, 92f);

            TextMesh bridgeInstruction = new GameObject("Bridge instruction").AddComponent<TextMesh>();
            bridgeInstruction.transform.position = new Vector3(0, 3.2f, 86.8f);
            bridgeInstruction.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            bridgeInstruction.text = "MAKE IT LONG ENOUGH / YETERİNCE UZAT";
            bridgeInstruction.fontSize = 42;
            bridgeInstruction.characterSize = 0.05f;
            bridgeInstruction.anchor = TextAnchor.MiddleCenter;
            bridgeInstruction.alignment = TextAlignment.Center;
            bridgeInstruction.color = new Color(0.22f, 0.25f, 0.32f);

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
            SightlinePuzzle sightlinePuzzle = player.AddComponent<SightlinePuzzle>();
            SetPrivate(sightlinePuzzle, "viewpoint", viewpointMarker.transform);
            SetPrivate(sightlinePuzzle, "sightlineTarget", sightlineTarget.transform);
            SetPrivate(sightlinePuzzle, "nearFragment", nearSightlineFragment.transform);
            SetPrivate(sightlinePuzzle, "farFragment", farSightlineFragment.transform);
            SetPrivate(sightlinePuzzle, "playerCamera", camera);
            SetPrivate(sightlinePuzzle, "linkedDoor", sightlineExitController);
            SetPrivate(sightlinePuzzle, "dialogue", director);
            SetPrivate(chapterTwoEntryTrigger, "dialogue", director);
            SetPrivate(chapterTwoEntryTrigger, "chapterSign", chapterTwoSign);
            SetPrivate(chapterTwoEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterTwoEntryTrigger, "checkpoint", chapterTwoCheckpoint.transform);
            fallRecovery.SetDialogue(director);
            SetPrivate(chapterTwoExitController, "dialogue", director);
            SetPrivate(chapterThreeEntryTrigger, "dialogue", director);
            SetPrivate(chapterThreeEntryTrigger, "chapterSign", chapterThreeSign);
            SetPrivate(chapterThreeEntryTrigger, "dialogueId", "chapter_three_entry");
            SetPrivate(chapterThreeEntryTrigger, "englishSign", "CHAPTER 03  /  WRONG DIMENSION");
            SetPrivate(chapterThreeEntryTrigger, "turkishSign", "BÖLÜM 03  /  YANLIŞ BOYUT");
            SetPrivate(chapterThreeEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterThreeEntryTrigger, "checkpoint", chapterThreeCheckpoint.transform);
            DimensionSlotPuzzle dimensionPuzzle = player.AddComponent<DimensionSlotPuzzle>();
            SetPrivate(dimensionPuzzle, "slotObject", dimensionPerspectiveObject);
            SetPrivate(dimensionPuzzle, "landingTarget", dimensionLandingTarget.transform);
            SetPrivate(dimensionPuzzle, "linkedDoor", chapterThreeExitController);
            SetPrivate(dimensionPuzzle, "dialogue", director);
            SetPrivate(chapterThreeExitController, "dialogue", director);
            SetPrivate(chapterFourEntryTrigger, "dialogue", director);
            SetPrivate(chapterFourEntryTrigger, "chapterSign", chapterFourSign);
            SetPrivate(chapterFourEntryTrigger, "dialogueId", "chapter_four_entry");
            SetPrivate(chapterFourEntryTrigger, "englishSign", "CHAPTER 04  /  SYSTEM ERROR");
            SetPrivate(chapterFourEntryTrigger, "turkishSign", "BÖLÜM 04  /  SİSTEM HATASI");
            SetPrivate(chapterFourEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterFourEntryTrigger, "checkpoint", chapterFourCheckpoint.transform);
            SetPrivate(systemErrorPuzzle, "dialogue", director);
            SetPrivate(chapterFourExitController, "dialogue", director);
            SetPrivate(chapterFiveEntryTrigger, "dialogue", director);
            SetPrivate(chapterFiveEntryTrigger, "chapterSign", chapterFiveSign);
            SetPrivate(chapterFiveEntryTrigger, "dialogueId", "chapter_five_entry");
            SetPrivate(chapterFiveEntryTrigger, "englishSign", "CHAPTER 05  /  DON'T TRUST YOUR EYES");
            SetPrivate(chapterFiveEntryTrigger, "turkishSign", "BÖLÜM 05  /  GÖZLERİNE GÜVENME");
            SetPrivate(chapterFiveEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterFiveEntryTrigger, "checkpoint", chapterFiveCheckpoint.transform);
            BlindSpotPuzzle blindSpotPuzzle = player.AddComponent<BlindSpotPuzzle>();
            SetPrivate(blindSpotPuzzle, "viewpoint", blindSpotMarker.transform);
            SetPrivate(blindSpotPuzzle, "exitDirection", blindSpotExitDirection.transform);
            SetPrivate(blindSpotPuzzle, "playerCamera", camera);
            SetPrivate(blindSpotPuzzle, "linkedDoor", chapterFiveExitController);
            SetPrivate(blindSpotPuzzle, "dialogue", director);
            SetPrivate(chapterFiveExitController, "dialogue", director);
            SetPrivate(chapterSixEntryTrigger, "dialogue", director);
            SetPrivate(chapterSixEntryTrigger, "chapterSign", chapterSixSign);
            SetPrivate(chapterSixEntryTrigger, "dialogueId", "chapter_six_entry");
            SetPrivate(chapterSixEntryTrigger, "englishSign", "CHAPTER 06  /  YOU AGAIN?");
            SetPrivate(chapterSixEntryTrigger, "turkishSign", "BÖLÜM 06  /  YİNE SEN?");
            SetPrivate(chapterSixEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterSixEntryTrigger, "checkpoint", chapterSixCheckpoint.transform);
            SetPrivate(behaviorMemoryPuzzle, "behavior", tracker);
            SetPrivate(behaviorMemoryPuzzle, "linkedDoor", chapterSixDoor);
            SetPrivate(behaviorMemoryPuzzle, "dialogue", director);
            SetPrivate(behaviorMemoryPuzzle, "dropCountLabel", dropLabel);
            SetPrivate(behaviorMemoryPuzzle, "retryCountLabel", retryLabel);
            SetPrivate(behaviorMemoryPuzzle, "waitCountLabel", waitLabel);
            SetPrivate(chapterSixExitController, "dialogue", director);
            SetPrivate(chapterSevenEntryTrigger, "dialogue", director);
            SetPrivate(chapterSevenEntryTrigger, "chapterSign", chapterSevenSign);
            SetPrivate(chapterSevenEntryTrigger, "dialogueId", "chapter_seven_entry");
            SetPrivate(chapterSevenEntryTrigger, "englishSign", "CHAPTER 07  /  THE ROOM THAT LAUGHS");
            SetPrivate(chapterSevenEntryTrigger, "turkishSign", "BÖLÜM 07  /  GÜLEN ODA");
            SetPrivate(chapterSevenEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterSevenEntryTrigger, "checkpoint", chapterSevenCheckpoint.transform);
            SetPrivate(laughingRoomDirector, "behavior", tracker);
            SetPrivate(laughingRoomDirector, "dialogue", director);
            SetPrivate(laughingRoomDirector, "prankObjects", new Transform[] { prankOne.transform, prankTwo.transform, prankThree.transform });
            SetPrivate(laughingRoomDirector, "roomLight", laughLight);
            SetPrivate(laughingRoomDirector, "movingWall", movingPrankWall.transform);
            SetPrivate(laughingRoomDirector, "linkedDoor", chapterSevenExitController);
            SetPrivate(chapterSevenExitController, "dialogue", director);
            SetPrivate(chapterEightEntryTrigger, "dialogue", director);
            SetPrivate(chapterEightEntryTrigger, "chapterSign", chapterEightSign);
            SetPrivate(chapterEightEntryTrigger, "dialogueId", "chapter_eight_entry");
            SetPrivate(chapterEightEntryTrigger, "englishSign", "CHAPTER 08  /  THE LOOP");
            SetPrivate(chapterEightEntryTrigger, "turkishSign", "BÖLÜM 08  /  DÖNGÜ");
            SetPrivate(chapterEightEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterEightEntryTrigger, "checkpoint", chapterEightCheckpoint.transform);
            SetPrivate(loopRoomDirector, "returnPoint", loopReturnPoint.transform);
            SetPrivate(loopRoomDirector, "exitDoor", chapterEightExitController);
            SetPrivate(loopRoomDirector, "dialogue", director);
            SetPrivate(loopRoomDirector, "loopSign", loopSign);
            SetPrivate(loopRoomDirector, "changingProps", new Transform[] { loopPropOne.transform, loopPropTwo.transform, loopPropThree.transform });
            SetPrivate(loopRoomDirector, "roomLight", loopLight);
            SetPrivate(chapterEightExitController, "dialogue", director);
            SetPrivate(chapterNineEntryTrigger, "dialogue", director);
            SetPrivate(chapterNineEntryTrigger, "chapterSign", chapterNineSign);
            SetPrivate(chapterNineEntryTrigger, "dialogueId", "chapter_nine_entry");
            SetPrivate(chapterNineEntryTrigger, "englishSign", "CHAPTER 09  /  THE WRONG AUDIENCE");
            SetPrivate(chapterNineEntryTrigger, "turkishSign", "BÖLÜM 09  /  YANLIŞ SEYİRCİ");
            SetPrivate(chapterNineEntryTrigger, "fallRecovery", fallRecovery);
            SetPrivate(chapterNineEntryTrigger, "checkpoint", chapterNineCheckpoint.transform);
            SetPrivate(audiencePuzzle, "exitDoor", chapterNineExitController);
            SetPrivate(audiencePuzzle, "dialogue", director);
            SetPrivate(audiencePuzzle, "stageSign", audienceStageSign);
            SetPrivate(audiencePuzzle, "audienceMembers", new Transform[] { audienceOne.transform, audienceTwo.transform, audienceThree.transform });
            SetPrivate(audiencePuzzle, "stageLight", stageLight);
            SetPrivate(chapterNineExitController, "dialogue", director);
            PerspectiveBridgePuzzle bridgePuzzle = player.AddComponent<PerspectiveBridgePuzzle>();
            SetPrivate(bridgePuzzle, "bridge", bridgePerspectiveObject);
            SetPrivate(bridgePuzzle, "landingTarget", bridgeTarget.transform);
            SetPrivate(bridgePuzzle, "linkedDoor", chapterTwoExitController);
            SetPrivate(bridgePuzzle, "dialogue", director);
            SetPrivate(sightlineExitController, "dialogue", director);
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
