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

namespace MultiActions
{
    public class MultiActionsMod : MelonMod
    {
        public bool isReady = false;
        TeleportHandler teleportHandler = new TeleportHandler();
        private int _scenesLoaded = 0;
        public override void OnApplicationStart()
        {
            // Checking if we have all requirements
            // Otherwise won't this mod work at all.
            if(!hasAllRequirements()) return;
            MultiActionSettings.RegisterSettings();
            SetupActionsButtons();
        }

        public bool hasAllRequirements()
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

            MelonLogger.Msg("Checking if we have all the required mods...");
            MelonLogger.Msg($"ReMod.Core: {File.Exists(reModCorePath) || File.Exists(reModCorePath2)}");
            MelonLogger.Msg($"UiExpansionKit: {File.Exists(uiExpansionKitPath)}");
            MelonLogger.Msg($"ActionMenuApi: {File.Exists(actionMenuApiPath)}");

            bool hasAll = (File.Exists(reModCorePath) || File.Exists(reModCorePath2)) && File.Exists(uiExpansionKitPath) && File.Exists(actionMenuApiPath);
            
            if(!hasAll)
                MelonLogger.Error("Missing one or more mods. Please make sure you have all the mods installed.");

            return hasAll;
        } 

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if(!hasAllRequirements()) return;
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
            VRCActionMenuPage.AddSubMenu(
                ActionMenuPage.Main, 
                MultiActionSettings.ModName,
                () =>
                {
                    /// <summary>
                    /// Taking out/in camera button
                    /// </summary>
                    CustomSubMenu.AddButton(
                        Utils.Camera.isCameraOn ? "Take in camera" : "Take out camera",
                        Utils.Camera.CameraToggle, 
                        null,
                        !MultiActionSettings.IsModEnabled()
                    );

                    /// <summary>
                    /// Respawns our local player
                    /// </summary>
                    CustomSubMenu.AddButton(
                        "Respawn", 
                        Utils.Players.respawnLocalPlayer, 
                        null, 
                        (!MultiActionSettings.IsModEnabled() || !MultiActionSettings.respawnButton.Value)
                    );

                    /// <summary>
                    /// Quits game
                    /// </summary>
                    CustomSubMenu.AddSubMenu(
                        "Quit game", 
                        () =>
                        {
                            CustomSubMenu.AddButton(
                                "Yes",
                                UnityEngine.Application.Quit,
                                null,
                                !MultiActionSettings.IsModEnabled()
                            );
                        }, null,
                        (!MultiActionSettings.IsModEnabled() || !MultiActionSettings.quitButton.Value)
                    );

                    /// <summary>
                    /// Opens different menus for local player
                    /// </summary>
                    CustomSubMenu.AddSubMenu("Open", () => 
                    {
                        CustomSubMenu.AddButton("Avatars", () =>
                        {
                            VRCUiManagerEx.Instance.ShowUi();
                            VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.AvatarMenu);
                        }, null, !MultiActionSettings.IsModEnabled());

                        CustomSubMenu.AddButton("Settings", () =>
                        {
                            VRCUiManagerEx.Instance.ShowUi();
                            VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.SettingsMenu);
                        }, null, !MultiActionSettings.IsModEnabled());

                        CustomSubMenu.AddButton("Safety", () =>
                        {
                            VRCUiManagerEx.Instance.ShowUi();
                            VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.SafetyMenu);
                        }, null, !MultiActionSettings.IsModEnabled());
                    }, 
                    null,
                    // When using this custom submenu and we open a setting
                    // We the user who is in XRDevice will be stuck
                    // So just ensuring we are not in XRDevice
                    (!MultiActionSettings.IsModEnabled() || !Utils.Extra.isInXR()));

                    // Lets simply not render if we don't want risky functions enabled
                    if (MultiActionSettings.riskyF.Value)
                    {

                        /// <summary>
                        /// Teleports to player
                        /// </summary>
                        CustomSubMenu.AddSubMenu("Teleport to", () =>
                        {
                            var allPlayers = Utils.Players.getAllPlayers();
                            foreach (var player in allPlayers)
                            {
                                CustomSubMenu.AddSubMenu(
                                    player.field_Private_APIUser_0.displayName,
                                    () =>
                                    {
                                        CustomSubMenu.AddButton(
                                            $"TP to: {player.field_Private_APIUser_0.displayName}",
                                            () => teleportHandler.TeleportTo(player),
                                            null, 
                                            !MultiActionSettings.IsModEnabled()
                                        );
                                    },
                                    null,
                                    !MultiActionSettings.IsModEnabled()
                                );
                            }
                        }, null, !MultiActionSettings.IsModEnabled());

                        /// <summary>
                        /// Teleporting menu
                        /// </summary>
                        CustomSubMenu.AddSubMenu("Teleporting", () =>
                        {

                            CustomSubMenu.AddButton("Save position", () =>
                            {
                                var player = Utils.Players.getLocalPlayer();
                                if (player == null) return;

                                var controller = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>();
                                controller.enabled = false;
                                BuiltinUiUtils.ShowInputPopup(
                                    "Save Point Name", 
                                    "", 
                                    InputField.InputType.Standard, 
                                    false, 
                                    "Save", 
                                    (msg, _, _2) =>
                                    {
                                        controller.enabled = true;
                                        teleportHandler.AddSavePoint(msg, player.transform.position);
                                        SavedPointsTeleport.AddItem(
                                            msg, 
                                            null,
                                            () => teleportHandler.TeleportTo(teleportHandler.GetSavePoint(msg))
                                        );
                                    },
                                    () => controller.enabled = true
                                );
                            }, null, !MultiActionSettings.IsModEnabled());

                            CustomSubMenu.AddSubMenu("Saved", () =>
                            {
                                CustomSubMenu.AddButton("Clear all", () =>
                                {
                                    teleportHandler.ClearAllSavePoints();
                                }, null, !MultiActionSettings.IsModEnabled());

                                // Display all of our saves
                                var points = teleportHandler.GetSavePoints();
                                foreach(var point in points)
                                {
                                    var p = teleportHandler.GetSavePoint(point);
                                    if (p == null) return;
                                    CustomSubMenu.AddSubMenu(point, () =>
                                    {
                                        CustomSubMenu.AddButton("Delete", () =>
                                        {
                                            teleportHandler.RemoveSavePoint(point);
                                        });
                                        CustomSubMenu.AddButton("Teleport", () =>
                                        {
                                            // Teleport player to p
                                            var player = Utils.Players.getLocalPlayer();
                                            if (player == null) return;
                                            teleportHandler.TeleportTo(p);
                                        }, null, !MultiActionSettings.IsModEnabled());
                                    });
                                }
                            });

                        }, null, !MultiActionSettings.IsModEnabled());

                    }
                    
                }, null, !MultiActionSettings.IsModEnabled());
        }

        private static ReRadioTogglePage SavedPointsTeleport;
        public IEnumerator<string> CreateTabMenu()
        {
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null)
                yield return null;

            // TODO: Find a solution to disable if we don't have risky function on.

            var TeleportsTab = new ReCategoryPage("Teleports", true);
            ReTabButton.Create("Teleports", "Open Teleports", "Teleports", null);
            var TeleportsConfig = TeleportsTab.AddCategory("Teleports Config");
            SavedPointsTeleport = new ReRadioTogglePage("Saved points");
            SavedPointsTeleport.OnClose += () =>
            {
                SavedPointsTeleport.ClearItems();
                var points = teleportHandler.GetSavePoints();
                foreach (var point in points)
                {
                    var p = teleportHandler.GetSavePoint(point);
                    if (p == null) return;
                    SavedPointsTeleport.AddItem(
                        point,
                        null,
                        () => teleportHandler.TeleportTo(p)
                    );
                }
            };
            TeleportsConfig.AddButton("Saved points", "Open to see all saved points", SavedPointsTeleport.Open);
        }
    }

    public class TeleportHandler
    {
        public Dictionary<string, Vector3> SavePoints = new Dictionary<string, Vector3>();

        public void ClearAllSavePoints()
        {
            SavePoints.Clear();
        }

        public void RemoveSavePoint(string name)
        {
            SavePoints.Remove(name);
        }

        public void AddSavePoint(string name, Vector3 location)
        {
            SavePoints.Add(name, location);
        }

        public List<string> GetSavePoints()
        {
            return SavePoints.Keys.ToList();
        }
        public Vector3 GetSavePoint(string name)
        {
            return SavePoints[name];
        }

        public void TeleportTo(Vector3 location)
        {
            var player = Utils.Players.getLocalPlayer().GetPlayerApi();
            if (player == null) return;
            player.TeleportTo(location, player.gameObject.transform.rotation);
        }

        public void TeleportTo(Player p)
        {
            var player = Utils.Players.getLocalPlayer().GetPlayerApi();
            if (player == null) return;
            player.TeleportTo(p.transform.position, player.gameObject.transform.rotation);
        }
    }
}
