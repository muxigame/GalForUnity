//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  UxmlHandler.cs 2022-10-10 22:48:44
//
//======================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    [CreateAssetMenu(menuName = "UxmlHandler", fileName = "UxmlHandler")]
    [FilePath("Assets/GalForUnity/Graph/UxmlHandler.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UxmlHandler : ScriptableSingleton<UxmlHandler>{
        public StyleSheet gfuTogglePortUss;
        public StyleSheet galGraphWindowUss;
        public StyleSheet gfuConfigFieldUss;
        public StyleSheet plotNodeUss;
        public StyleSheet plotBlockUss;
        public StyleSheet dragObjectFieldUss;
        public VisualTreeAsset draggableBlockUxml;
        public VisualTreeAsset plotBlockUxml;
        public VisualTreeAsset galGraphWindowUxml;
        public VisualTreeAsset poseBindingPointUxml;
        public VisualTreeAsset poseBindingItem;
        public VisualTreeAsset configAddition;
    }
}