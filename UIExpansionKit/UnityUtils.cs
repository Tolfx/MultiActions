using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace UIExpansionKit
{
    public static class UnityUtils
    {
        public static GameObject? FindInactiveObjectInActiveRoot(string path)
        {
            var split = path.Split(new char[]{'/'}, 2);
            var rootObject = GameObject.Find($"/{split[0]}")?.transform;
            if (rootObject == null) return null;
            return Transform.FindRelativeTransformWithPath(rootObject, split[1], false)?.gameObject;
        }

        public static void MakeBackgroundMoreSolid(this Image image)
        {
            var color = image.color;
            color.a = 1;
            image.color = color;
        }

        public static void LinkTextIntoTmp(GameObject root)
        {
            var normalText = root.GetComponentInChildren<Text>();
            if (ReferenceEquals(normalText, null)) return;
            var tmp = root.GetComponentInChildren<TMP_Text>();
            if (ReferenceEquals(tmp, null)) return;

            normalText.text = tmp.text;
            
            normalText.RegisterDirtyVerticesCallback(new Action(() => { tmp.SetText(normalText.text); }));
        }
    }
}