using GalForUnity.Controller;
using GalForUnity.Model.Plot;
using UnityEditor;

namespace GalForUnity.Editor{
    public class CustomRenameEditor{
        [CustomEditor(typeof(OptionController))][CanEditMultipleObjects]
        public class RenameTestEditor : RenameEditor { }      
        // [CustomEditor(typeof(PlotModel))][CanEditMultipleObjects]
                                                              
        public class RenameTestEditor2 : RenameEditor { }
        
        // [CustomEditor(typeof(PlotRequire))][CanEditMultipleObjects]
        public class RenameTestEditor3 : RenameEditor { }
    }
}