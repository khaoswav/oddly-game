using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Small, original runtime-generated placeholder sounds for the greybox.
    /// These are intentionally lightweight synthesis cues, not a substitute
    /// for the final composed score or recorded sound-design pass.
    /// </summary>
    public static class ProceduralAudioPalette
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateAmbientBed()
        {
            const float duration = 8f;
            int count = Mathf.RoundToInt(SampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float time = i / (float)SampleRate;
                float slowPulse = 0.72f + 0.12f * Mathf.Sin(2f * Mathf.PI * 0.25f * time);
                float low = Mathf.Sin(2f * Mathf.PI * 55f * time) * 0.16f;
                float mid = Mathf.Sin(2f * Mathf.PI * 82.5f * time) * 0.075f;
                float high = Mathf.Sin(2f * Mathf.PI * 110f * time) * 0.025f;
                samples[i] = (low + mid + high) * slowPulse;
            }

            return CreateClip("ODDLY Procedural Ambient Bed", samples);
        }

        public static AudioClip CreateDoorCue()
        {
            const float duration = 0.42f;
            int count = Mathf.RoundToInt(SampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float time = i / (float)SampleRate;
                float progress = time / duration;
                float envelope = Mathf.Exp(-8.5f * progress);
                float frequency = Mathf.Lerp(92f, 38f, progress);
                float body = Mathf.Sin(2f * Mathf.PI * frequency * time);
                float click = Mathf.Sin(2f * Mathf.PI * 1450f * time) * Mathf.Exp(-65f * progress);
                samples[i] = (body * 0.32f + click * 0.08f) * envelope;
            }

            return CreateClip("ODDLY Soft Door Thump", samples);
        }

        public static AudioClip CreateRevealCue()
        {
            const float duration = 0.95f;
            int count = Mathf.RoundToInt(SampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float time = i / (float)SampleRate;
                float progress = time / duration;
                float envelope = Mathf.Pow(1f - progress, 1.8f);
                float fundamental = Mathf.Sin(2f * Mathf.PI * 523.25f * time);
                float overtone = Mathf.Sin(2f * Mathf.PI * 783.99f * time) * 0.32f;
                samples[i] = (fundamental + overtone) * envelope * 0.19f;
            }

            return CreateClip("ODDLY Chapter Reveal Chime", samples);
        }

        public static AudioClip CreateSurpriseCue()
        {
            const float duration = 0.72f;
            int count = Mathf.RoundToInt(SampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float time = i / (float)SampleRate;
                float progress = time / duration;
                float wobble = Mathf.Sin(2f * Mathf.PI * 4.2f * time);
                float frequency = 360f + wobble * 105f;
                float envelope = Mathf.Exp(-3.1f * progress);
                float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * time);
                float overtone = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * time) * 0.22f;
                samples[i] = (fundamental + overtone) * envelope * 0.16f;
            }

            return CreateClip("ODDLY Environmental Surprise", samples);
        }

        private static AudioClip CreateClip(string clipName, float[] samples)
        {
            AudioClip clip = AudioClip.Create(clipName, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}