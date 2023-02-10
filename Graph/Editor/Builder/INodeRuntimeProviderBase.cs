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

namespace GalForUnity.Graph.Editor.Builder{
    public interface INodeRuntimeProviderBase{
        GalNodeAsset GetInputNode(int portIndex, int connectionIndex);
        List<GalNodeAsset> GetInputNodes(int portIndex);
        GalNodeAsset GetOutputNode(int portIndex, int connectionIndex);
        List<GalNodeAsset>  GetOutputNodes(int portIndex);
        GalNodeAsset GetNode(long instanceID);
        bool IsInputPortConnected(int portIndex);
        int InputPortCount();
        bool IsOutputPortConnected(int portIndex);
        int OutputPortCount();
        
        int GetInputPortConnectionCount(int portIndex);
        
        int GetOutputPortConnectionCount(int portIndex);
    }

}
