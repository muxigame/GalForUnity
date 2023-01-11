//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  INodeRuntimeProviderBase.cs
//
//        Created by 半世癫(Roc) at 2022-05-11 23:07:16
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;

namespace GalForUnity.Graph.SceneGraph{
    public interface INodeRuntimeProviderBase{
        GfuNodeAsset GetInputNode(int portIndex, int connectionIndex);
        List<GfuNodeAsset> GetInputNodes(int portIndex);
        GfuNodeAsset GetOutputNode(int portIndex, int connectionIndex);
        List<GfuNodeAsset>  GetOutputNodes(int portIndex);
        GfuNodeAsset GetNode(long instanceID);
        bool IsInputPortConnected(int portIndex);
        int InputPortCount();
        bool IsOutputPortConnected(int portIndex);
        int OutputPortCount();
        
        int GetInputPortConnectionCount(int portIndex);
        
        int GetOutputPortConnectionCount(int portIndex);
    }

}
