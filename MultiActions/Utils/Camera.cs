using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.UserCamera;

namespace MultiActions.Utils
{
    public static class Camera
    {
        public static bool isCameraOn = false;
        /*
            Github: https://github.com/PennyBunny/VRCMods/blob/0183ba68e53f5628921ce8d2f86ee51db6ac096d/ShortCuts/Actions.cs#L99
        */
        public static void CameraToggle()
        {
            if (UserCameraController.field_Internal_Static_UserCameraController_0.prop_UserCameraMode_0 == UserCameraMode.Off)
            {
                UserCameraController.field_Internal_Static_UserCameraController_0.prop_UserCameraMode_0 = UserCameraMode.Photo;
                UserCameraController.field_Internal_Static_UserCameraController_0.prop_UserCameraSpace_0 = UserCameraSpace.Attached;
                isCameraOn = true;
            }
            else
            {
                UserCameraController.field_Internal_Static_UserCameraController_0.prop_UserCameraMode_0 = UserCameraMode.Off;
                isCameraOn = false;
            }
        }
    }
}
