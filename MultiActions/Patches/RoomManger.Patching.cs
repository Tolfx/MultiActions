using System.Collections.Generic;
using System.Linq;
using VRC.Core;
using System.Reflection;
using HarmonyLib;


namespace MultiActions.Patches 
{
    [HarmonyPatch]
    class RoomManagerPatches
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(RoomManager).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x =>
                    x.Name.Contains("Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_") &&
                    !x.Name.Contains("PDM"))
                .Cast<MethodBase>();
        }
 
        static void Postfix(ApiWorld __0, ApiWorldInstance __1, ref bool __result)
        {
            if (__result)
            {
                if (__1 != null)
                {
                    MultiActionsMod.JoinRoomPatch(__0, __1, __result);
                }
            }
        }
    }
}

