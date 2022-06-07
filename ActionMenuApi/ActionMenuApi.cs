﻿using System;
using System.Collections;
using ActionMenuApi.Helpers;
using ActionMenuApi.Managers;
using MelonLoader;

#pragma warning disable 1591

[assembly: MelonInfo(typeof(ActionMenuApi.ActionMenuApi), "ActionMenuApi", "1.0.0", "gompo", "https://github.com/gompoc/VRChatMods/releases")]
[assembly: MelonGame("VRChat", "VRChat")]


namespace ActionMenuApi
{
    public partial class ActionMenuApi : MelonMod
    {
        
        public override void OnApplicationStart()
        {
            ResourcesManager.LoadTextures();
            MelonCoroutines.Start(WaitForActionMenuInit());
            try
            {
                Patches.PatchAll(HarmonyInstance);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Patching failed with exception: {e.Message}");
            }
        }

        private IEnumerator WaitForActionMenuInit()
        {
            while (ActionMenuDriver.prop_ActionMenuDriver_0 == null) //VRCUIManager Init is too early 
                yield return null;
            if (string.IsNullOrEmpty(ID)) yield break;
            ResourcesManager.InitLockGameObject();
            RadialPuppetManager.Setup();
            FourAxisPuppetManager.Setup();
        }
        
        public override void OnUpdate()
        {
            RadialPuppetManager.OnUpdate();
            FourAxisPuppetManager.OnUpdate();
        }

        private static string ID = "gompo";
    }
}
