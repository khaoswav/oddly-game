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
            if (trigger != null && !trigger.isTrigger)
                throw new System.InvalidOperationException("Expected trigger collider on: " + objectName);
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

            if (Object.FindObjectOfType<CharacterController>() == null)
                throw new System.InvalidOperationException("No first-person CharacterController was found.");

            if (Object.FindObjectOfType<DialogueDirector>() == null)
                throw new System.InvalidOperationException("DialogueDirector is missing from the generated scene.");

            if (Object.FindObjectsOfType<AfterChoiceEchoPillar>().Length != 8)
                throw new System.InvalidOperationException("The generated memory chamber must contain exactly eight echo pillars.");

            if (Object.FindObjectsOfType<FinalChoiceTrigger>().Length != 3)
                throw new System.InvalidOperationException("The final decision chamber must contain exactly three ending choices.");

            bool sceneEnabled = EditorBuildSettings.scenes.Any(scene =>
                scene.path == ScenePath && scene.enabled);
            if (!sceneEnabled)
                throw new System.InvalidOperationException("Prototype scene is not enabled in Build Settings.");
        }
    }
}