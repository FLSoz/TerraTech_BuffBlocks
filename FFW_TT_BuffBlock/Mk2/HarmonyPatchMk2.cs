using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace FFW_TT_BuffBlock
{
    public static class HarmonyPatchMk2
    {
        [HarmonyPatch(typeof(ManWheels.Wheel), "UpdateAttachData")] // Thanks Aceba!
        private static class FixUpdateAttachData
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                codes = codes.Skip(2).ToList();
                BuffBlocks.logger.Trace("FFW: Transpiled ManWheels.Wheel.UpdateAttachData()");
                return codes;
            }
        }
    }
}
