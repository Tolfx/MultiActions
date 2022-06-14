using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using ActionMenuApi.Api;
using ActionMenuApi.Helpers;
using ActionMenuApi.Managers;
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

namespace MultiActions.ActionMenu
{
    public static class MaActionsRender
    {
        // Lets have a list that we can store all of our MaActions
        public static List<MaAction> MaActionsList = new List<MaAction>();

        public static void Init()
        {
            // MultiActions.ActionMenu.Actions.MaActionsMain.Init();
            // MultiActions.ActionMenu.Actions.MaActionsOpen.Init();
            // MultiActions.ActionMenu.Actions.MaActionTeleport.Init();
            // MultiActions.ActionMenu.Actions.MaActionsTeleporting.Init();
        }

        public static void Render()
        {
            // Check if mod is enabled
            if (!MultiActionSettings.IsModEnabled())
                return;

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

                    // To the guidelines of VRCMG you need to check allowed or not.
                    // Lets simply not render if we don't want risky functions enabled
                    if (MultiActionSettings.riskyF.Value && MultiActionSettings.allowedForRisky())
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
                                            () => TeleportHandler.TeleportTo(player),
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
                                        TeleportHandler.AddSavePoint(msg, player.transform.position);
                                        var nButtonSaved = ReMenuButton.Create(
                                            msg,
                                            "Click to teleport",
                                            () =>
                                            {
                                                TeleportHandler.TeleportTo(TeleportHandler.GetSavePoint(msg));
                                            },
                                            MultiActionsMod.TeleportsCategory.RectTransform
                                        );
                                        MultiActionsMod.SavedPointsButtons.Add(msg, nButtonSaved);
                                    },
                                    () => controller.enabled = true
                                );
                            }, null, !MultiActionSettings.IsModEnabled());

                            CustomSubMenu.AddSubMenu("Saved", () =>
                            {
                                CustomSubMenu.AddButton("Clear all", () =>
                                {
                                    TeleportHandler.ClearAllSavePoints();
                                    // Loop through all buttons and Destroy them
                                    foreach (var button in MultiActionsMod.SavedPointsButtons)
                                    {
                                        button.Value.Destroy();
                                    }
                                    MultiActionsMod.SavedPointsButtons.Clear();
                                }, null, !MultiActionSettings.IsModEnabled());

                                // Display all of our saves
                                var points = TeleportHandler.GetSavePoints();
                                foreach(var point in points)
                                {
                                    var p = TeleportHandler.GetSavePoint(point);
                                    if (p == null) return;
                                    CustomSubMenu.AddSubMenu(point, () =>
                                    {
                                        CustomSubMenu.AddButton("Delete", () =>
                                        {
                                            TeleportHandler.RemoveSavePoint(point);
                                            var temp = MultiActionsMod.SavedPointsButtons[point];
                                            MultiActionsMod.SavedPointsButtons.Remove(point);
                                            temp.Destroy();
                                        });
                                        CustomSubMenu.AddButton("Teleport", () =>
                                        {
                                            // Teleport player to p
                                            var player = Utils.Players.getLocalPlayer();
                                            if (player == null) return;
                                            TeleportHandler.TeleportTo(p);
                                        }, null, !MultiActionSettings.IsModEnabled());
                                    });
                                }
                            });

                        }, null, !MultiActionSettings.IsModEnabled());

                    }
                    
                }, null, !MultiActionSettings.IsModEnabled());
        }

        public static void AddAction(MaAction action)
        {
            MaActionsList.Add(action);
        }

    }

}

