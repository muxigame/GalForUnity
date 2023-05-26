using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Core
{
    public class GalObject : ScriptableObject
    {
        private void OnEnable()
        {
            if (!RoleDB.Contains(this))
#if UNITY_EDITOR
                if (UnityEditor.AssetDatabase.Contains(this))
#endif
                    RoleDB.Add(this);
        }

        public string objectName = "Alice";
        public Gender gender = Gender.Girl;
        public string age = "16";
        public string height = "165";
        public string weight = "55";
        [SerializeReference] public List<Pose> pose = new List<Pose>();
    }

    [Serializable]
    public class Pose
    {
        public string name;
    }

    [Serializable]
    public class SpritePose : Pose
    {
        public Sprite sprite;
        public List<Anchor> anchors = new List<Anchor>();
    }

    [Serializable]
    public class AnchorSprite
    {
        public string name;
        public Vector2 offset;
        public Sprite sprite;
    }

    [Serializable]
    public class Anchor
    {
        public string name;
        public Vector2 pivot;
        public List<AnchorSprite> sprites = new List<AnchorSprite>();
    }
}