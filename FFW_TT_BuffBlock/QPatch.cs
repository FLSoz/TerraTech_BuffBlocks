using System;
using System.Reflection;
using HarmonyLib;
using BlockChangePatcher;
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

        public static Type[] LoadBefore()
        {
            return new Type[] { typeof(BlockChangePatcherMod) };
        }

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

        private static Change BuffWrapperPatcher = new Change
        {
            id = "ModuleBuffWrapperMk2",
            targetType = ChangeTargetType.GLOBAL,
            condition = null,
            patcher = new Action<BlockMetadata>(AddBuffWrapper)
        };

        private static void AddBuffWrapper(BlockMetadata blockData) {
            Transform editablePrefab = blockData.blockPrefab;
            ModuleBuffWrapperMk2 buffWrapper = editablePrefab.GetComponent<ModuleBuffWrapperMk2>();
            if (buffWrapper == null)
            {
                buffWrapper = editablePrefab.gameObject.AddComponent<ModuleBuffWrapperMk2>();
                buffWrapper.PrintDetails();
            }
        }

        public override void Init()
        {
            QPatch.Main();
            BlockChangePatcherMod.RegisterChange(BuffWrapperPatcher);
        }
    }
}
