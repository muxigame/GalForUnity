//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  NodeData.cs at 2022-11-04 20:51:28
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalForUnity.Graph.Nodes.Runtime;
using GalForUnity.Graph.SceneGraph;

namespace GalForUnity.Graph.Build{
    [Serializable]
    public abstract class RuntimeNode{
        
        public GalGraph GalGraph{ get; internal set; }
        public abstract Task<GfuNodeAsset> OnNodeEnter(GfuNodeAsset gfuNodeAsset);
    }

    public abstract class OperationNode : RuntimeNode{
        protected GfuNodeAsset NodeAsset;

        public override async Task<GfuNodeAsset> OnNodeEnter(GfuNodeAsset gfuNodeAsset){
            NodeAsset = gfuNodeAsset;
            foreach (var gfuConnectionAsset in gfuNodeAsset.inputPort.Where(x => x.HasConnection).SelectMany(x => x.connections)){
                var outputNode = gfuConnectionAsset.output.node;
                if (outputNode?.runtimeNode == null) continue;
                await outputNode.runtimeNode.OnNodeEnter(outputNode);
            }

            return gfuNodeAsset;
        }

        public abstract (T value, bool over) GetValueFromOutput<T>(int index);

        public (T value, bool over) GetValueFromInput<T>(int index){ return NodeAsset.inputPort[index].GetValueIfExist<T>(); }
    }

    public interface IPortOverride{
        public IReadOnlyList<PortDefinition> InputPortOverride{ get; }
        public IReadOnlyList<PortDefinition> OutputPortOverride{ get; }
    }
}