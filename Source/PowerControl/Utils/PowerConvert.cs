using Verse;

namespace PowerControl.Utils
{
    public static class PowerConvert
    {
        public static string GetPrettyPower(float watts, bool includePositiveSign = true)
        {
            const float KW = 1000;
            const float MW = 1_000_000;
            const float GW = 1_000_000_000;

            bool isPositive = watts > 0f;
            bool isNegative = watts < 0;
            watts = isNegative ? -watts : watts;

            string sign = isPositive ? includePositiveSign ? "+" : "" : "-";

            if (watts >= GW)
            {
                return $"{sign}{watts/GW:F1} " + "PC.GW".Translate();
            }
            if (watts >= MW)
            {
                return $"{sign}{watts / MW:F1} " + "PC.MW".Translate();
            }
            if(watts >= KW)
            {
                return $"{sign}{watts / KW:F1} " + "PC.KW".Translate();
            }

            return $"{sign}{watts:F0} " + "PC.W".Translate();
        }
    }
}
