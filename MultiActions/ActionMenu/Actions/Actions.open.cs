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
    public static class MaActionsOpen
    {

        public static void Init()
        {

            var avatars = new MaAction(
                "Avatars",
                MaHigherActions.Open,
                false,
                () =>
                {
                    VRCUiManagerEx.Instance.ShowUi();
                    VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.AvatarMenu);
                }
            );

            MaActionsRender.AddAction(avatars);

            var settings = new MaAction(
                "Settings",
                MaHigherActions.Open,
                false,
                () =>
                {
                    VRCUiManagerEx.Instance.ShowUi();
                    VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.SettingsMenu);
                }
            );

            MaActionsRender.AddAction(settings);

            var safety = new MaAction(
                "Safety",
                MaHigherActions.Open,
                false,
                () =>
                {
                    VRCUiManagerEx.Instance.ShowUi();
                    VRCUiManagerEx.Instance.ShowScreen(QuickMenu.MainMenuScreenIndex.SettingsMenu);
                }
            );

            MaActionsRender.AddAction(safety);
        }

    }

}

