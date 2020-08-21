using UnityEngine;
using Verse;

namespace PowerControl
{
    public class ModCore : Mod
    {
        public static bool DoTrace { get; private set; }

        public ModCore(ModContentPack content) : base(content)
        {
            Init();
        }

        private void Init()
        {
            string name = SteamUtility.SteamPersonaName;
            DoTrace = name == "Epicguru";
            Trace("Trace enabled.");

            CreateHook();

            Log($"Init complete. Hi there, {name}!");
        }

        private void CreateHook()
        {
            var go = new GameObject("PowerControl Hook");
            go.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(go);
            go.AddComponent<LoopHook>();
        }

        public static void Trace(object msg)
        {
            if(DoTrace)
                Verse.Log.Message($"[PC] {msg ?? "<null>"}");
        }

        public static void Log(object msg)
        {
            Verse.Log.Message($"[PC] {msg ?? "<null>"}");
        }

        public static void Warn(object msg)
        {
            Verse.Log.Warning($"[PC] {msg ?? "<null>"}");
        }

        public static void Error(object msg)
        {
            Verse.Log.Error($"[PC] {msg ?? "<null>"}");
        }
    }
}
