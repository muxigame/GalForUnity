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
using MUXIGame.Serializer;

namespace GalForUnity.Graph.Nodes.Runtime{
    public class PlotNode : RuntimeNode{

        public Field<List<IGalConfig>> config=new List<IGalConfig>();
        public Field<List<HashSet<string>>> portalizedData=new List<HashSet<string>>();

        private int _index = 0;

        public override GfuNodeAsset Execute(GfuNodeAsset gfuNodeAsset){
            _index++;
            if (_index > config.value.Count){
                _index = 0;
                return gfuNodeAsset.outputPort[0].connections[0].input.node;
            }
            return gfuNodeAsset;
        }
    }
}
