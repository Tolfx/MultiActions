using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using ActionMenuApi.Api;
using VRC.UserCamera;
using VRC.SDKBase;
using VRC.Core;
using VRC.UI;
using UnityEngine.XR;
using ReMod.Core.VRChat;
using ReMod.Core.UI.QuickMenu;
using VRC;
using UnityEngine;
using UnityEngine.UI;
using MultiActions.Utils;
using UIExpansionKit.API;
using System.Reflection;
using HarmonyLib;
using MultiActions.ActionMenu;

namespace MultiActions
{
    public class MultiActionsMod : MelonMod
    {
        public bool isReady = false;
        private int _scenesLoaded = 0;
        public override void OnApplicationStart()
        {
            // Checking if we have all requirements
            // Otherwise won't this mod work at all.
            if(!hasAllRequirements()) return;
            MultiActionSettings.RegisterSettings();
            SetupActionsButtons();
        }

        public static void JoinRoomPatch(ApiWorld __0, ApiWorldInstance __1, bool __result)
        {
            MelonLogger.Msg("Joined a new world/club, checking tags.");
            var tags = RoomManager.field_Internal_Static_ApiWorld_0.tags;
            // Check if the tags has author_tag_game or author_tag_club
            var hasTags = tags.Contains("author_tag_game") || tags.Contains("author_tag_club");
            // If we are in a world with tags, we will check if we are allowed to use risky functions
            if (!hasTags)
            {
                MelonLogger.Msg("The world/club doesn't have tags, allowing risky functions.");
                MultiActionSettings.areWeAllowedToUseRiskyFunctions = true;
            }
            else
            {
                MelonLogger.Msg("The world/club has tags, force disabling risky functions.");
                // We are not in a world with tags, so we will disable the risky functions
                MultiActionSettings.areWeAllowedToUseRiskyFunctions = false;
            }
        }

        public bool hasAllRequirements(bool log = true)
        {
            // Check if we have the following:
            // - ReMod.Core
            // - UiExpansionKit
            // - ActionMenuApi

            // ReMod.Core should be located at UserLibs
            // UIExpansionKit should be located at Mods
            // ActionMenuApi should be located at Mods

            // We check by checking if the file exists
            var path = Path.GetFullPath(Path.Combine(Application.dataPath));
            // Assuming we are at VRChat\VRChat_Data, we want to remove \VRChat_Data
            path = path.Substring(0, path.LastIndexOf("\\"));
            var reModCorePath = Path.Combine(path, "UserLibs", "ReMod.Core.dll");
            // I've seen ReMod in root, so could be there too I guess?
            var reModCorePath2 = Path.Combine(path, "ReMod.Core.dll");
            var uiExpansionKitPath = Path.Combine(path, "Mods", "UIExpansionKit.dll");
            var actionMenuApiPath = Path.Combine(path, "Mods", "ActionMenuApi.dll");

            if (log)
            {
                MelonLogger.Msg("Checking if we have all the required mods...");
                MelonLogger.Msg($"ReMod.Core: {File.Exists(reModCorePath) || File.Exists(reModCorePath2)}");
                MelonLogger.Msg($"UiExpansionKit: {File.Exists(uiExpansionKitPath)}");
                MelonLogger.Msg($"ActionMenuApi: {File.Exists(actionMenuApiPath)}");
            }

            bool hasAll = (File.Exists(reModCorePath) || File.Exists(reModCorePath2)) && File.Exists(uiExpansionKitPath) && File.Exists(actionMenuApiPath);
            
            if(!hasAll)
                MelonLogger.Error("Missing one or more mods. Please make sure you have all the mods installed.");

            return hasAll;
        } 

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if(!hasAllRequirements(false)) return;

            if (_scenesLoaded <= 2)
            {
                _scenesLoaded++;
                if (_scenesLoaded == 2)
                {
                    MelonCoroutines.Start(CreateTabMenu());
                }
            }
        }

        private void SetupActionsButtons()
        {
            MaActionsRender.Render();
        }

        public static ReRadioTogglePage SavedPointsTeleport;
        public static ReMenuCategory TeleportsCategory;
        public static Dictionary<String, ReMenuButton> SavedPointsButtons = new Dictionary<String, ReMenuButton>();
        public IEnumerator<string> CreateTabMenu()
        {
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null)
                yield return null;

            var TeleportsTab = new ReCategoryPage("Teleports", true);
            ReTabButton.Create("Teleports", "Open Teleports", "Teleports", null);

            TeleportsCategory = TeleportsTab.AddCategory("Saved teleports");
        }
    }
}
