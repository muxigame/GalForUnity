

using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.Core.Editor.IMGUI{
    public class RenameEditorData:ScriptableObject{
        public Dictionary<Object,bool> Foldout=new Dictionary<Object, bool>();
    }
}