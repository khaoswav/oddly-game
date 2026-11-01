using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Applies a predictable desktop performance baseline and a conservative
    /// profile on machines with 8 GB of system memory or less.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class WorldPresentationDirector : MonoBehaviour
    {
        public bool IsConservativeProfile { get; private set; }

        private void Awake()
        {
            IsConservativeProfile = SystemInfo.systemMemorySize <= 8192;

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = IsConservativeProfile ? 38f : 62f;
            QualitySettings.shadowResolution = IsConservativeProfile
                ? ShadowResolution.Medium : ShadowResolution.High;
            QualitySettings.pixelLightCount = IsConservativeProfile ? 2 : 4;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.softParticles = false;
        }
    }
}