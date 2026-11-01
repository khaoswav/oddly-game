using UnityEngine;

namespace Oddly
{
    /// <summary>Persistent, non-identifying record of optional-room completion.</summary>
    public static class HiddenChapterArchive
    {
        private const string KeyPrefix = "oddly.hidden.completed.";

        public static void MarkCompleted(int chapter)
        {
            if (chapter < 1 || chapter > 8) return;
            PlayerPrefs.SetInt(KeyPrefix + chapter, 1);
            PlayerPrefs.Save();
        }

        public static bool IsCompleted(int chapter) =>
            chapter >= 1 && chapter <= 8 && PlayerPrefs.GetInt(KeyPrefix + chapter, 0) == 1;

        public static int CompletedCount
        {
            get
            {
                int count = 0;
                for (int i = 1; i <= 8; i++)
                    if (IsCompleted(i)) count++;
                return count;
            }
        }

        public static string GetCompletedRoomCodes()
        {
            string result = string.Empty;
            for (int i = 1; i <= 8; i++)
            {
                if (!IsCompleted(i)) continue;
                if (result.Length > 0) result += "  ";
                result += i.ToString("00");
            }
            return result.Length == 0 ? "--" : result;
        }

        public static int CompletionMask
        {
            get
            {
                int mask = 0;
                for (int i = 1; i <= 8; i++)
                    if (IsCompleted(i)) mask |= 1 << (i - 1);
                return mask;
            }
        }
    }
}
