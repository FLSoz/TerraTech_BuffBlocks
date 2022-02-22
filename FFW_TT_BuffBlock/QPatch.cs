using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    internal class QPatch
    {
        public static void Main()
        {
            BuffBlocks.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class BuffBlocks : ModBase
    {
        const string HarmonyID = "ffw.ttmm.buffblock.mod";
        internal static Harmony harmony = new Harmony(HarmonyID);

        public override void DeInit()
        {
            harmony.UnpatchAll(HarmonyID);
        }

        public override void Init()
        {
            QPatch.Main();
        }
    }
}
