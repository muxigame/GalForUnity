//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotNode.cs at 2022-11-05 02:51:07
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;
using UnityEngine;

namespace GalForUnity.Graph.Nodes.Runtime{
    public class PlotNode : RuntimeNode{

        [SerializeReference]
        public List<IGalBlock> config=new List<IGalBlock>();

        private int _index = 0;

        public override GfuNodeAsset Execute(GfuNodeAsset gfuNodeAsset){
            _index++;
            if (_index > config.Count){
                _index = 0;
                return gfuNodeAsset.outputPort[0].connections[0].input.node;
            }
            return gfuNodeAsset;
        }
    }
}
