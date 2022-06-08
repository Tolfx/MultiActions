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

        TeleportHandler teleportHandler = new TeleportHandler();
        private int _scenesLoaded = 0;
        public override void OnApplicationStart()
        {
            MultiActionSettings.RegisterSettings();
            SetupActionsButtons();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
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


                }, null, !MultiActionSettings.IsModEnabled());
        }

        private static ReRadioTogglePage SavedPointsTeleport;
        public IEnumerator<string> CreateTabMenu()
        {
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null)
                yield return null;

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
