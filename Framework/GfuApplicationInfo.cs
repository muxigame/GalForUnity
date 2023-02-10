

namespace GalForUnity.Framework{
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