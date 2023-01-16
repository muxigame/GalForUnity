using System;
using UnityEditor;

namespace GalForUnity.Core.Scripts.Editor{
    [Serializable]
    [FilePath("ProjectSettings/GalPrefs.asset", FilePathAttribute.Location.ProjectFolder)]
    public class GalPrefs : ScriptableSingleton<GalPrefs>{
        public string globalID;
        public void Save(){ base.Save(true); }
    }
}