using UIExpansionKit.API;
using UnityEngine;

namespace UIExpansionKit
{
    internal class CustomQuickMenuPageImpl : CustomLayoutedPageWithOwnedMenuImpl
    {
        public CustomQuickMenuPageImpl(LayoutDescription? layoutDescription) : base(layoutDescription)
        {
            IsQuickMenu = true;
        }
        
        protected override Transform ParentTransform => UiExpansionKitMod.GetQuickMenu().transform.Find("Container");
        protected override GameObject MenuPrefab => UiExpansionKitMod.Instance.StuffBundle.GenericPopupWindow;
        protected override Transform GetContentRoot(Transform instantiatedMenu) => instantiatedMenu.Find("Content/Scroll View/Viewport/Content");
        protected override RectTransform GetTopLevelUiObject(Transform instantiatedMenu) => instantiatedMenu.Cast<RectTransform>();

        protected override void AdjustMenuTransform(Transform transform, int layer)
        {
            transform.localScale = Vector3.one * 3f;
            transform.Cast<RectTransform>().localPosition = new Vector3(55, 500, -15 + -5 * layer);
        }
    }
}