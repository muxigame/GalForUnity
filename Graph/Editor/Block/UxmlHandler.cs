

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    [CreateAssetMenu(menuName = "UxmlHandler", fileName = "UxmlHandler")]
    [FilePath("Assets/GalForUnity/UxmlHandler.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UxmlHandler : ScriptableSingleton<UxmlHandler>{
        public StyleSheet gfuTogglePortUss;
        public StyleSheet galGraphWindowUss;
        public StyleSheet gfuConfigFieldUss;
        public StyleSheet plotNodeUss;
        public StyleSheet plotBlockUss;
        public StyleSheet dragObjectFieldUss;
        public StyleSheet previewSearchWindowUss;
        public VisualTreeAsset draggableBlockUxml;
        public VisualTreeAsset plotBlockUxml;
        public VisualTreeAsset galGraphWindowUxml;
        public VisualTreeAsset poseBindingPointUxml;
        public VisualTreeAsset poseBindingItem;
        public VisualTreeAsset configAddition;
        public VisualTreeAsset previewSearchWindow;
        public VisualTreeAsset searchProviderItem;
    }
}