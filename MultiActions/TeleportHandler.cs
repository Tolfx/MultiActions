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

namespace MultiActions
{
    public static class TeleportHandler
    {
        public static Dictionary<string, Vector3> SavePoints = new Dictionary<string, Vector3>();

        public static void ClearAllSavePoints()
        {
            SavePoints.Clear();
        }

        public static void RemoveSavePoint(string name)
        {
            SavePoints.Remove(name);
        }

        public static void AddSavePoint(string name, Vector3 location)
        {
            SavePoints.Add(name, location);
        }

        public static List<string> GetSavePoints()
        {
            return SavePoints.Keys.ToList();
        }
        public static Vector3 GetSavePoint(string name)
        {
            return SavePoints[name];
        }

        public static void TeleportTo(Vector3 location)
        {
            var player = Utils.Players.getLocalPlayer().GetPlayerApi();
            if (player == null) return;
            player.TeleportTo(location, player.gameObject.transform.rotation);
        }

        public static void TeleportTo(Player p)
        {
            var player = Utils.Players.getLocalPlayer().GetPlayerApi();
            if (player == null) return;
            player.TeleportTo(p.transform.position, player.gameObject.transform.rotation);
        }
    }
}
