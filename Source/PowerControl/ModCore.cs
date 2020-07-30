using Verse;

namespace PowerControl
{
    public class ModCore : Mod
    {
        public ModCore(ModContentPack content) : base(content)
        {
            Init();
        }

        private void Init()
        {
            Log("Init complete.");
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
