using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Central routing for music, ambience and one-shot effects.
    /// The prototype supplies a quiet synthesized ambience and original
    /// placeholder cues until the final music and sound assets are produced.
    /// </summary>
    public sealed class WorldAudioDirector : MonoBehaviour
    {
        public static WorldAudioDirector Instance { get; private set; }

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource effectsSource;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.75f;
        [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.55f;
        [SerializeField, Range(0f, 1f)] private float effectsVolume = 0.9f;
        [SerializeField, Min(0.05f)] private float crossfadeDuration = 1.4f;

        private AudioClip generatedAmbient;
        private AudioClip doorCue;
        private AudioClip revealCue;
        private AudioClip surpriseCue;
        private Coroutine musicTransition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CreateSourcesIfNeeded();
            musicVolume = PlayerPrefs.GetFloat("oddly.audio.music", musicVolume);
            ambienceVolume = PlayerPrefs.GetFloat("oddly.audio.ambience", ambienceVolume);
            effectsVolume = PlayerPrefs.GetFloat("oddly.audio.effects", effectsVolume);
            generatedAmbient = ProceduralAudioPalette.CreateAmbientBed();
            doorCue = ProceduralAudioPalette.CreateDoorCue();
            revealCue = ProceduralAudioPalette.CreateRevealCue();
            surpriseCue = ProceduralAudioPalette.CreateSurpriseCue();
            ApplyVolumes();

            if (ambienceSource.clip == null)
                ambienceSource.clip = generatedAmbient;
            ambienceSource.loop = true;
            if (!ambienceSource.isPlaying)
                ambienceSource.Play();
        }

        private void CreateSourcesIfNeeded()
        {
            if (musicSource == null) musicSource = CreateSource("Music Bus", true);
            if (ambienceSource == null) ambienceSource = CreateSource("Ambience Bus", true);
            if (effectsSource == null) effectsSource = CreateSource("Effects Bus", false);
        }

        private AudioSource CreateSource(string sourceName, bool loop)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }

        public void SetMusic(AudioClip clip, bool restart = false)
        {
            if (musicSource == null || clip == null) return;
            if (!restart && musicSource.clip == clip && musicSource.isPlaying) return;
            if (musicTransition != null) StopCoroutine(musicTransition);
            musicTransition = StartCoroutine(CrossfadeMusic(clip));
        }

        public void SetAmbience(AudioClip clip)
        {
            if (ambienceSource == null || clip == null) return;
            if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;
            ambienceSource.clip = clip;
            ambienceSource.loop = true;
            ambienceSource.volume = ambienceVolume;
            ambienceSource.Play();
        }

        public void PlayEffect(AudioClip clip, float pitch = 1f)
        {
            if (effectsSource == null || clip == null) return;
            effectsSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
            effectsSource.PlayOneShot(clip, effectsVolume);
        }

        public void PlayDoorCue() { PlayEffect(doorCue); }
        public void PlayRevealCue() { PlayEffect(revealCue, 1.04f); }
        public void PlaySurpriseCue() { PlayEffect(surpriseCue, 0.92f); }

        public void PlayEndingCue(int endingIndex)
        {
            float pitch = endingIndex == 0 ? 0.78f : endingIndex == 1 ? 1.22f : 1.04f;
            PlayEffect(revealCue, pitch);
        }

        public void SetMusicVolume(float value) { musicVolume = Mathf.Clamp01(value); ApplyVolumes(); SaveVolumes(); }
        public void SetAmbienceVolume(float value) { ambienceVolume = Mathf.Clamp01(value); ApplyVolumes(); SaveVolumes(); }
        public void SetEffectsVolume(float value) { effectsVolume = Mathf.Clamp01(value); ApplyVolumes(); SaveVolumes(); }

        public void StopMusic()
        {
            if (musicTransition != null) StopCoroutine(musicTransition);
            musicTransition = StartCoroutine(FadeOutMusic());
        }

        private IEnumerator CrossfadeMusic(AudioClip nextClip)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(elapsed / crossfadeDuration));
                yield return null;
            }

            musicSource.Stop();
            musicSource.clip = nextClip;
            musicSource.loop = true;
            musicSource.Play();

            elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, musicVolume, Mathf.Clamp01(elapsed / crossfadeDuration));
                yield return null;
            }
            musicSource.volume = musicVolume;
            musicTransition = null;
        }

        private IEnumerator FadeOutMusic()
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(elapsed / crossfadeDuration));
                yield return null;
            }
            musicSource.Stop();
            musicSource.volume = musicVolume;
            musicTransition = null;
        }

        private void ApplyVolumes()
        {
            if (musicSource != null) musicSource.volume = musicVolume;
            if (ambienceSource != null) ambienceSource.volume = ambienceVolume;
        }

        private void SaveVolumes()
        {
            PlayerPrefs.SetFloat("oddly.audio.music", musicVolume);
            PlayerPrefs.SetFloat("oddly.audio.ambience", ambienceVolume);
            PlayerPrefs.SetFloat("oddly.audio.effects", effectsVolume);
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (generatedAmbient != null) Destroy(generatedAmbient);
            if (doorCue != null) Destroy(doorCue);
            if (revealCue != null) Destroy(revealCue);
            if (surpriseCue != null) Destroy(surpriseCue);
        }
    }
}