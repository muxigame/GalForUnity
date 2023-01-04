using System.Collections;
using System.Collections.Generic;
using GalForUnity.Graph.Build;
using UnityEngine;

namespace GalForUnity.Graph.Nodes.Runtime{
     public interface INode{
     }
     public interface INode<T>:INode where T : RuntimeNode{
          T RuntimeNode{ get; set; }
     }
}