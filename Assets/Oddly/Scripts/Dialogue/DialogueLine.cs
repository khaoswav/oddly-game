using System;
using UnityEngine;

namespace Oddly
{
    [Serializable]
    public sealed class DialogueLine
    {
        public string id;
        [TextArea(2, 4)] public string turkish;
        [TextArea(2, 4)] public string english;
        public string speaker = "NARRATOR";

        public string Resolve(SystemLanguage language)
        {
            return language == SystemLanguage.Turkish ? turkish : english;
        }
    }
}
