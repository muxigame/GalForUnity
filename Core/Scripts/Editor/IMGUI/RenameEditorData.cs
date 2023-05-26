using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.Core.Editor{
    public class RenameEditorData:ScriptableObject{
        public Dictionary<Object,bool> Foldout=new Dictionary<Object, bool>();
    }
}