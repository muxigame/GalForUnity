//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  RenameEditorData.cs
//
//        Created by 半世癫(Roc) at 2021-11-16 14:36:59
//
//======================================================================

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalForUnity.Editor{
    public class RenameEditorData:ScriptableObject{
        public Dictionary<Object,bool> Foldout=new Dictionary<Object, bool>();
    }
}