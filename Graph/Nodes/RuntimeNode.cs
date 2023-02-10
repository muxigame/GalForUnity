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

namespace GalForUnity.Graph.Nodes{
    [Serializable]
    public abstract class RuntimeNode{
        
        public GalGraph GalGraph{ get; internal set; }
        public abstract Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset);
    }

    public abstract class OperationNode : RuntimeNode{
        protected GalNodeAsset NodeAsset;

        public override async Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset){
            NodeAsset = galNodeAsset;
            foreach (var gfuConnectionAsset in galNodeAsset.inputPort.Where(x => x.HasConnection).SelectMany(x => x.connections)){
                var outputNode = gfuConnectionAsset.output.node;
                if (outputNode?.runtimeNode == null) continue;
                await outputNode.runtimeNode.OnNodeEnter(outputNode);
            }

            return galNodeAsset;
        }

        public abstract (T value, bool over) GetValueFromOutput<T>(int index);

        public (T value, bool over) GetValueFromInput<T>(int index){ return NodeAsset.inputPort[index].GetValueIfExist<T>(); }
    }

    public interface IPortOverride{
        public IReadOnlyList<PortDefinition> InputPortOverride{ get; }
        public IReadOnlyList<PortDefinition> OutputPortOverride{ get; }
    }
}