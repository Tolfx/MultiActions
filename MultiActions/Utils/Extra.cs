using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;
using UnityEngine.XR;


namespace MultiActions.Utils
{
    public static class Extra
    {
        public static QuickMenu getQuickMenu()
        {
            return QuickMenu.prop_QuickMenu_0;
        }
        public static PlayerManager getPlayerManager()
        {
            return PlayerManager.prop_PlayerManager_0;
        }
        public static VRCUiManager getUiManager()
        {
            return VRCUiManager.prop_VRCUiManager_0;
        }
        public static UserInteractMenu getInteractMenu()
        {
            return Resources.FindObjectsOfTypeAll<UserInteractMenu>()[0];
        }
        public static void toggleOutlines(Renderer render, bool state)
        {
            if (HighlightsFX.prop_HighlightsFX_0 == null) return;
            HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(render, state);
        }

        public static bool isInXR()
        {
            return XRDevice.isPresent;
        }
    }
}
