using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oddly.Editor
{
    /// <summary>
    /// Rebuilds, validates and packages the generated ODDLY prototype scene.
    /// Run these commands from Unity 6.3 after scripts finish compiling.
    /// </summary>
    public static class OddlyReleaseTools
    {
        private const string ScenePath = "Assets/Scenes/Prototype.unity";

        [MenuItem("Oddly/Release/Validate Generated Prototype")]
        public static void ValidateGeneratedPrototype()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogError("ODDLY validation blocked: wait for Unity compilation and asset import to finish.");
                return;
            }

            PrototypeSceneBuilder.RebuildPrototypeScene();
            ValidateActiveSceneOrThrow();
            Debug.Log("ODDLY validation passed. Generated scene and required gameplay systems are present.");
        }

        [MenuItem("Oddly/Release/Build macOS")]
        public static void BuildMacOS()
        {
            BuildStandalone(BuildTarget.StandaloneOSX, "Builds/macOS/ODDLY.app");
        }

        [MenuItem("Oddly/Release/Build Windows")]
        public static void BuildWindows()
        {
            BuildStandalone(BuildTarget.StandaloneWindows64, "Builds/Windows/ODDLY.exe");
        }

        private static void BuildStandalone(BuildTarget target, string outputPath)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                throw new System.InvalidOperationException("Wait for Unity compilation and asset import to finish.");

            PrototypeSceneBuilder.RebuildPrototypeScene();
            ValidateActiveSceneOrThrow();

            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0 || !scenes.Contains(ScenePath))
                throw new System.InvalidOperationException("The generated prototype scene is missing from Build Settings.");

            string directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            Debug.Log("ODDLY build result: " + summary.result +
                      " | target: " + target +
                      " | errors: " + summary.totalErrors +
                      " | warnings: " + summary.totalWarnings +
                      " | output: " + outputPath);

            if (summary.result != BuildResult.Succeeded)
                throw new System.InvalidOperationException("ODDLY build failed. Inspect the Unity Editor build log.");
        }

        private static void RequireComponent<T>(string objectName) where T : Component
        {
            GameObject target = GameObject.Find(objectName);
            if (target == null || target.GetComponent<T>() == null)
                throw new System.InvalidOperationException(
                    "Required generated scene component is missing: " + objectName + " (" + typeof(T).Name + ")");

            BoxCollider trigger = target.GetComponent<BoxCollider>();
            if (trigger == null || !trigger.isTrigger)
                throw new System.InvalidOperationException("Required trigger collider is missing or disabled on: " + objectName);
        }

        private static void RequireReference(SerializedObject serialized, string propertyName, string owner)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null || property.objectReferenceValue == null)
                throw new System.InvalidOperationException(
                    "Required serialized reference is missing: " + owner + "." + propertyName);
        }

        private static void ValidateActiveSceneOrThrow()
        {
            Scene active = SceneManager.GetActiveScene();
            if (!active.IsValid() || active.path != ScenePath)
                throw new System.InvalidOperationException("The generated prototype scene is not the active scene.");

            RequireComponent<FinalEpilogueTrigger>("Final epilogue trigger");
            RequireComponent<AfterChoiceMemoryRoomTrigger>("After-choice memory room entry trigger");
            RequireComponent<AfterChoiceNinthRecordTrigger>("Ninth record convergence trigger");
            RequireComponent<AfterChoiceUnwrittenEndingTrigger>("Unwritten ending finale interaction");
            RequireComponent<AfterChoiceUnwrittenEpilogueTrigger>("Unwritten ending epilogue interaction");
            RequireComponent<AfterChoiceLastEchoTrigger>("The Last Echo reward interaction");

            GameObject player = GameObject.Find("Player");
            if (player == null || player.GetComponent<CharacterController>() == null ||
                player.GetComponent<FirstPersonController>() == null ||
                player.GetComponent<FallRecovery>() == null)
                throw new System.InvalidOperationException(
                    "Player must contain CharacterController, FirstPersonController and FallRecovery.");

            Camera playerCamera = player.GetComponentInChildren<Camera>(true);
            if (playerCamera == null || playerCamera.GetComponent<AudioListener>() == null)
                throw new System.InvalidOperationException("Player camera or its AudioListener is missing.");

            InteractionController interaction = player.GetComponent<InteractionController>();
            if (interaction == null)
                throw new System.InvalidOperationException("Player InteractionController is missing.");

            SerializedObject interactionData = new SerializedObject(interaction);
            RequireReference(interactionData, "playerCamera", "Player InteractionController");
            RequireReference(interactionData, "holdPoint", "Player InteractionController");
            if (interactionData.FindProperty("playerCamera").objectReferenceValue != playerCamera)
                throw new System.InvalidOperationException("InteractionController is not wired to the active player camera.");

            GameObject gameUi = GameObject.Find("Game UI");
            if (gameUi == null || gameUi.GetComponent<WorldPresentationDirector>() == null)
                throw new System.InvalidOperationException("WorldPresentationDirector is missing from Game UI.");

            if (gameUi.GetComponent<WorldAudioDirector>() == null)
                throw new System.InvalidOperationException("WorldAudioDirector is missing from Game UI.");

            DialogueDirector dialogue = Object.FindObjectOfType<DialogueDirector>();
            if (dialogue == null)
                throw new System.InvalidOperationException("DialogueDirector is missing from the generated scene.");

            AfterChoiceEchoPillar[] echoes = Object.FindObjectsOfType<AfterChoiceEchoPillar>();
            if (echoes.Length != 8)
                throw new System.InvalidOperationException("The generated memory chamber must contain exactly eight echo pillars.");

            bool[] echoNumbers = new bool[8];
            foreach (AfterChoiceEchoPillar echo in echoes)
            {
                RequireTriggerOn(echo.gameObject, "Echo pillar " + echo.name);
                SerializedObject echoData = new SerializedObject(echo);
                SerializedProperty chapter = echoData.FindProperty("chapterNumber");
                if (chapter == null || chapter.intValue < 1 || chapter.intValue > 8 ||
                    echoNumbers[chapter.intValue - 1])
                    throw new System.InvalidOperationException("Echo chapter numbers must be unique values from 1 to 8.");
                echoNumbers[chapter.intValue - 1] = true;
                RequireReference(echoData, "pillarVisual", echo.name);
                RequireReference(echoData, "statusSign", echo.name);
            }

            GameObject memoryEntryObject = GameObject.Find("After-choice memory room entry trigger");
            SerializedObject memoryData = new SerializedObject(memoryEntryObject.GetComponent<AfterChoiceMemoryRoomTrigger>());
            SerializedProperty pillars = memoryData.FindProperty("memoryPillars");
            if (pillars == null || pillars.arraySize != 8)
                throw new System.InvalidOperationException("The memory room must reference exactly eight visual pillars.");
            for (int i = 0; i < pillars.arraySize; i++)
                if (pillars.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    throw new System.InvalidOperationException("Memory pillar reference " + (i + 1) + " is missing.");
            RequireReference(memoryData, "statusSign", "After-choice memory room");

            FinalChoiceTrigger[] choices = Object.FindObjectsOfType<FinalChoiceTrigger>();
            if (choices.Length != 3)
                throw new System.InvalidOperationException("The final decision chamber must contain exactly three ending choices.");

            bool[] endingIndices = new bool[3];
            foreach (FinalChoiceTrigger choice in choices)
            {
                RequireTriggerOn(choice.gameObject, "Final choice " + choice.name);
                SerializedObject choiceData = new SerializedObject(choice);
                RequireReference(choiceData, "puzzle", choice.name);
                SerializedProperty endingIndex = choiceData.FindProperty("endingIndex");
                if (endingIndex == null || endingIndex.intValue < 0 || endingIndex.intValue > 2 ||
                    endingIndices[endingIndex.intValue])
                    throw new System.InvalidOperationException("Final choice indices must be unique values from 0 to 2.");
                endingIndices[endingIndex.intValue] = true;
            }

            GameObject finalPageObject = GameObject.Find("Unwritten ending epilogue interaction");
            SerializedObject finalPageData =
                new SerializedObject(finalPageObject.GetComponent<AfterChoiceUnwrittenEpilogueTrigger>());
            RequireReference(finalPageData, "statusSign", "The Last Page epilogue");

            bool sceneEnabled = EditorBuildSettings.scenes.Any(scene =>
                scene.path == ScenePath && scene.enabled);
            if (!sceneEnabled)
                throw new System.InvalidOperationException("Prototype scene is not enabled in Build Settings.");
        }

        private static void RequireTriggerOn(GameObject target, string description)
        {
            BoxCollider collider = target.GetComponent<BoxCollider>();
            if (collider == null || !collider.isTrigger)
                throw new System.InvalidOperationException("A trigger collider is required on " + description + ".");
        }
    }
}