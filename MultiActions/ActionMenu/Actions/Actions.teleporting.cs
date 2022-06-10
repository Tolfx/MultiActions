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

namespace MultiActions.ActionMenu.Actions
{
    public static class MaActionsTeleporting
    {
        public static void Init()
        {

            var savePosition = new MaAction(
                "Save Position",
                MaHigherActions.Teleporting,
                true,
                () =>
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
                            MultiActionsMod.SavedPointsTeleport.AddItem(
                                msg,
                                player.transform.position
                            );
                        },
                        () => controller.enabled = true
                    );
                }
            );

            MaActionsRender.AddAction(savePosition);

            var saved = new MaAction(
                "Saved",
                MaHigherActions.Teleporting,
                true,
                () =>
                {
                    CustomSubMenu.AddButton("Clear all", () =>
                    {
                        TeleportHandler.ClearAllSavePoints();
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
                }
            );

            MaActionsRender.AddAction(saved);
        }
    }

}

