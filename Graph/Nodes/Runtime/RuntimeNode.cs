//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  NodeData.cs at 2022-11-04 20:51:28
//
//======================================================================

using System;
using GalForUnity.Graph.SceneGraph;

namespace GalForUnity.Graph.Build{
    [Serializable]
    public abstract class RuntimeNode{
        public abstract GfuNodeAsset Execute(GfuNodeAsset gfuNodeAsset);
    }

    public abstract class OperationNode:RuntimeNode{
        public abstract (T value,bool over) GetValue<T>(int index);
    }
}