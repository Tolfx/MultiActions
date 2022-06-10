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
    public static class MaActionsMain
    {
        public static void Init()
        {

            var camera = new MaAction(
                Utils.Camera.isCameraOn ? "Take in camera" : "Take out camera",
                MaHigherActions.Main,
                false,
                Utils.Camera.CameraToggle
            );
                
            MaActionsRender.AddAction(camera);

            var respawn = new MaAction(
                "Respawn",
                MaHigherActions.Main,
                false,
                Utils.Players.respawnLocalPlayer
            );

            MaActionsRender.AddAction(respawn);

            var quitGame = new MaAction(
                "Quit Game",
                MaHigherActions.Main,
                false,
                () =>
                {
                    CustomSubMenu.AddButton(
                        "Yes",
                        UnityEngine.Application.Quit,
                        null,
                        !MultiActionSettings.IsModEnabled()
                    );
                });

            MaActionsRender.AddAction(quitGame);
        }
    }

}

