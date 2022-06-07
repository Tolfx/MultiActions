using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;


namespace MultiActions.Utils
{
    public static class Players
    {
        public static VRCPlayer getLocalPlayer()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }
        public static Il2CppSystem.Collections.Generic.List<Player> getAllPlayers()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null) return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }
        public static APIUser getAPI(this Player p)
        {
            return p.field_Private_APIUser_0;
        }
        public static Player getPlayer(string id)
        {
            var t = getAllPlayers();
            for (var c=0;c<t.Count;c++)
            {
                var p = t[c]; if (p == null) continue;
                if (p.getAPI().id == id) return p;
            }
            return null;
        }
        public static Player getPlayer(int local_id)
        {
            var t = getAllPlayers();
            return t[local_id];
        }
        public static Player getSelectedPlayer(this QuickMenu inst)
        {
            if (QuickMenu.prop_QuickMenu_0 == null ||
                QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0 == null ||
                PlayerManager.prop_PlayerManager_0 == null) return null;
            return getPlayer(QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0.id);
        }
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
        public static string getInstanceId()
        {
            return APIUser.CurrentUser.location;
        }
        public static void respawnLocalPlayer()
        {
            var p = getLocalPlayer().field_Private_VRCPlayerApi_0;
            if (p == null) return;
            p.Respawn();
        }
        public static void respawnPlayer(Player p)
        {
            if (p == null) return;
            p.field_Private_VRCPlayerApi_0.Respawn();
        }
    }
}
