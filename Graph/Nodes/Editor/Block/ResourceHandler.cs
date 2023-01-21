using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor.Block
{
    [CreateAssetMenu(menuName = "ResourceHandler", fileName = "ResourceHandler")]
    [FilePath("Assets/GalForUnity/Graph/ResourceHandler.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ResourceHandler :  ScriptableSingleton<ResourceHandler>
    {
        public Sprite poseBindingAnchor;
        public Sprite defaultPose;
    }
}
