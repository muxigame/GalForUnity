//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ISaveable.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 21:58:42
//
//======================================================================

using System.Runtime.Serialization;
using GalForUnity.Graph;
using GalForUnity.Graph.AssetGraph;

namespace GalForUnity.System.Archive.Data{
    public interface ISaveable:ISerializable,IRecoverable{
    }
    public interface IRecoverable{
        void Recover();
    }
    public interface INodeSaveable:ISerializable{
        void Recover(GfuGraph gfuGraph);
    }
}
