//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotNode.cs at 2022-11-05 02:51:07
//
//======================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using GalForUnity.Core;
using GalForUnity.Core.Block;
using UnityEngine;

namespace GalForUnity.Graph.Nodes{
    public class PlotNode : RuntimeNode{

        [SerializeReference]
        public List<IGalBlock> config=new List<IGalBlock>();

        private int _index = 0;

        public override async Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset){
            if (_index >= config.Count){
                _index = 0;
                return galNodeAsset.outputPort[0].connections[0].input.node;
            }
            config[_index].RuntimeNode = this;
            await config[_index++].Process(GalCore.ActiveCore);
            return galNodeAsset;
        }
    }
}
