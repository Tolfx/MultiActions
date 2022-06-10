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
    public class MaAction
    {
        public string Name { get; set; }
        public bool isRisky { get; set; }

        private Action _action;

        public MaHigherActions HigherAction { get; set; }

        public MaAction(string name, MaHigherActions pos, bool isRisky, Action action)
        {
            Name = name;
            this.isRisky = isRisky;
            _action = action;
            HigherAction = pos;
        }

        public void AddButton()
        {

            // Check if risky functions are allowed
            bool rEnabled = MultiActionSettings.allowedForRisky();
            // Check if this action is risky
            bool isRisky = this.isRisky;

            // If rEnabled is enabled and isRisky is true, then we can add the button
            // Otherwise just return
            if (!rEnabled && isRisky)
                return;

            CustomSubMenu.AddButton(
                Name,
                _action, 
                null,
                !MultiActionSettings.IsModEnabled()
            );
        }
    }

}

