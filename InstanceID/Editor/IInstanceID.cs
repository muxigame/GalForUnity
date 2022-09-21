//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  IInstanceID.cs
//
//        Created by 半世癫(Roc) at 2021-12-15 20:12:52
//
//======================================================================

namespace GalForUnity.InstanceID{
    public interface IInstanceID{
        long InstanceID{ get; set; }
        void RegisterInstanceID();
    }
}
