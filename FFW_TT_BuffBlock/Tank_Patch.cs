﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace FFW_TT_BuffBlock
{
    public static class WrappedDataHolder
    {
        [HarmonyPatch(typeof(ModuleWeaponGun), "OnAttach")]
        class ModuleWeaponGun_Attach_Patch
        {
            static bool Prefix(ref ModuleWeaponGun __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddWeapon(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleWeaponGun), "OnDetach")]
        class ModuleWeaponGun_Detach_Patch
        {
            static void Postfix(ref ModuleWeaponGun __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveWeapon(__instance);
            }
        }

        [HarmonyPatch(typeof(ModuleWheels), "OnAttach")]
        class ModuleWheels_Attach_Patch
        {
            static bool Prefix(ref ModuleWheels __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddWheels(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleWheels), "OnDetach")]
        class ModuleWheels_Detach_Patch
        {
            static bool Prefix(ref ModuleWheels __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveWheels(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleBooster), "OnAttach")]
        class ModuleBooster_Attach_Patch
        {
            static bool Prefix(ref ModuleBooster __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddBooster(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleBooster), "OnDetach")]
        class ModuleBooster_Detach_Patch
        {
            static bool Prefix(ref ModuleBooster __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveBooster(__instance);
                return true;
            }
        }
    }
}

