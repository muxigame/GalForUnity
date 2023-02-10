

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
