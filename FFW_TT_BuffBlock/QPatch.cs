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

        internal static Logger logger;
        private static bool Inited = false;

        internal static bool DEBUG = false;
        internal static void ConfigureLogger()
        {
            logger = new Logger("BuffBlocks");
            logger.Info("Logger is setup");
        }

        public override void EarlyInit()
        {
            if (!Inited)
            {
                ConfigureLogger();
                Inited = true;
            }
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

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
