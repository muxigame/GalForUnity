//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SaveFlagAttribute.cs
//
//        Created by 半世癫(Roc) at 2021-12-12 23:11:47
//
//======================================================================

using System;

namespace GalForUnity.System.Archive.Attributes{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    
    public class SaveFlagAttribute : Attribute{
        public long instanceID;
        public SaveFlagAttribute(){ }
    }
}
