//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuApplicationInfo.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 17:41:36
//
//======================================================================

namespace GalForUnity.System{
    public class GfuApplicationInfo{
        public static bool IsEditor{
#if UNITY_EDITOR
            get => true;

#else
        get => false;
#endif
        }

        //     public static bool IsAndroid{
        // #if Uni
        //         get => true;
        //
        // #else
        //         get => false;
        // #endif
        //     }
    }
}