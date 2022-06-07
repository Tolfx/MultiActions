using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using ActionMenuApi.Api;
using VRC.UserCamera;
using VRC.SDKBase;
using VRC.Core;
using VRC.UI;
using UnityEngine.XR;
using ReMod.Core.VRChat;
using VRC;
using UnityEngine;
using MultiActions.Utils;

namespace MultiActions
{
    public class MultiActionsMod : MelonMod
    {

        public override void OnApplicationStart()
        {
            MultiActionSettings.RegisterSettings();
            SetupActionsButtons();
        }

        private void SetupActionsButtons()
        {
            VRCActionMenuPage.AddSubMenu(
                ActionMenuPage.Main, 
                MultiActionSettings.ModName,
                () =>
                {
                    CustomSubMenu.AddButton(
                        Utils.Camera.isCameraOn ? "Take in camera" : "Take out camera",
                        Utils.Camera.CameraToggle, 
                        null,
                        !MultiActionSettings.IsModEnabled()
                    );

                    CustomSubMenu.AddButton(
                        "Respawn", 
                        Utils.Players.respawnLocalPlayer, 
                        null, 
                        (!MultiActionSettings.IsModEnabled() || !MultiActionSettings.respawnButton.Value)
                    );

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

                    CustomSubMenu.AddSubMenu("Teleport to", () =>
                    {
                        var allPlayers = Utils.Players.getAllPlayers();
                        foreach (var player in allPlayers)
                        {
                            CustomSubMenu.AddSubMenu(
                                player.field_Private_APIUser_0.displayName,
                                () =>
                                {
                                    CustomSubMenu.AddButton($"TP to: {player.field_Private_APIUser_0.displayName}",
                                    () =>
                                    {
                                        var localPlayer = Utils.Players.getLocalPlayer().GetPlayerApi();
                                        if (localPlayer == null) return;
                                        localPlayer.TeleportTo(player.transform.position, localPlayer.gameObject.transform.rotation);
                                    }, null, !MultiActionSettings.IsModEnabled());
                                },
                                null,
                                !MultiActionSettings.IsModEnabled()
                            );
                        }
                    }, null, !MultiActionSettings.IsModEnabled());

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
                    }, null, !MultiActionSettings.IsModEnabled());
                });
        }
    }
}
