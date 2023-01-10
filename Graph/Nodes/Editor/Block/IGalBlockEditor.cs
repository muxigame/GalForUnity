//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  IBlock.cs Created at 2022-09-30 19:55:16
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.SceneGraph;

namespace GalForUnity.Graph.Block{
    public interface IGalBlockEditor{
        public IEnumerable<(GfuPort, GfuPortAsset)> OnSavePort(GfuNodeAsset gfuNodeAsset);
        public IEnumerable<(GfuPortAsset,GfuPort)> OnLoadPort(GfuNodeAsset gfuNodeAsset);
    }
}
