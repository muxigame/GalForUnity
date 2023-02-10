using UnityEditor;
using UnityEngine;

namespace GalForUnity.Graph.Editor.Block
{
    [CreateAssetMenu(menuName = "ResourceHandler", fileName = "ResourceHandler")]
    [FilePath("Assets/GalForUnity/ResourceHandler.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ResourceHandler :  ScriptableSingleton<ResourceHandler>
    {
        public Sprite poseBindingAnchor;
        public Sprite defaultPose;
    }
}
