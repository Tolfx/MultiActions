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

namespace MultiActions.ActionMenu
{
    public static class MaActionsRender
    {
        // Lets have a list that we can store all of our MaActions
        public static List<MaAction> MaActionsList = new List<MaAction>();

        public static void Init()
        {
            MultiActions.ActionMenu.Actions.MaActionsMain.Init();
            MultiActions.ActionMenu.Actions.MaActionsOpen.Init();
            MultiActions.ActionMenu.Actions.MaActionTeleport.Init();
            MultiActions.ActionMenu.Actions.MaActionsTeleporting.Init();
        }

        public static void Render()
        {
            // Check if mod is enabled
            if (!MultiActionSettings.IsModEnabled())
                return;

            // Let's create our "categories"
            VRCActionMenuPage.AddSubMenu(
                ActionMenuPage.Main, 
                MultiActionSettings.ModName,
                () =>
                {

                    // Let's render our "Main" actions
                    CustomSubMenu.AddSubMenu("Main", () =>
                    {
                        RenderMainActions();
                    });

                    CustomSubMenu.AddSubMenu("Open", () => 
                    {
                        RenderOpenActions();
                    });

                    CustomSubMenu.AddSubMenu("Teleport to", () =>
                    {
                        RenderTeleportActions();
                    });

                    CustomSubMenu.AddSubMenu("Teleporting", () =>
                    {
                        RenderTeleportingActions();
                    });

                }, null
            );

        }

        public static void AddAction(MaAction action)
        {
            MaActionsList.Add(action);
        }

        private static void RenderMainActions()
        {
            // Lets get all MaActionsList with the "Main" category
            var mainActions = MaActionsList.Where(x => x.HigherAction == MaHigherActions.Main).ToList();
            // Let's loop through all of our actions
            foreach (var action in mainActions)
            {
                // Let's add the button
                action.AddButton();
            }
        }

        private static void RenderTeleportingActions()
        {
            // Lets get all MaActionsList with the "Teleporting" category
            var teleportingActions = MaActionsList.Where(x => x.HigherAction == MaHigherActions.Teleporting).ToList();
            // Let's loop through all of our actions
            foreach (var action in teleportingActions)
            {
                // Let's add the button
                action.AddButton();
            }
        }

        private static void RenderTeleportActions()
        {
            // Lets get all MaActionsList with the "Teleport" category
            var teleportActions = MaActionsList.Where(x => x.HigherAction == MaHigherActions.Teleport).ToList();
            // Let's loop through all of our actions
            foreach (var action in teleportActions)
            {
                // Let's add the button
                action.AddButton();
            }
        }

        private static void RenderOpenActions()
        {
            // Lets get all MaActionsList with the "Open" category
            var openActions = MaActionsList.Where(x => x.HigherAction == MaHigherActions.Open).ToList();
            // Let's loop through all of our actions
            foreach (var action in openActions)
            {
                // Let's add the button
                action.AddButton();
            }
        }
    }

}

