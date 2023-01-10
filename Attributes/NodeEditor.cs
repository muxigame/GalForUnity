using System;
using UnityEngine;

namespace GalForUnity.Attributes{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeEditor:Attribute{
        public Type Type;
        public NodeEditor(Type type){
            this.Type = type;
        }
    }
}