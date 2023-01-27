using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.Core
{
    public class RoleAssets : ScriptableObject
    {
        public new string name = "Alice";
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
    public class SpritePose:Pose
    {
        public Sprite sprite;
        public List<BindingPoint> bindingPoints=new List<BindingPoint>();
    }
    [Serializable]
    public class SpritePoseItem
    {
        public string name;
        public Vector2 offset;
        public Sprite sprite;
    }
    [Serializable]
    public class BindingPoint
    {
        public string name;
        public Vector2 point;
        public List<SpritePoseItem> spritePoseItems=new List<SpritePoseItem>();
    }
}