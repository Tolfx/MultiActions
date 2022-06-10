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

    public static class MaActionTeleport
    {
        public static void Init()
        {
            var teleport = new MaAction(
                "Teleport",
                MaHigherActions.Main,
                true,
                () =>
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
                            }
                        );
                    }
                }
            );

            MaActionsRender.AddAction(teleport);
        }
    }

}
