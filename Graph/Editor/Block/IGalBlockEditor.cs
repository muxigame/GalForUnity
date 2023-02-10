//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  IBlock.cs Created at 2022-09-30 19:55:16
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Graph.Editor.Builder;

namespace GalForUnity.Graph.Editor.Block{
    public interface IGalBlockEditor{
        public IEnumerable<(GalPort, GalPortAsset)> OnSavePort(GalNodeAsset galNodeAsset);
        public IEnumerable<(GalPortAsset,GalPort)> OnLoadPort(GalNodeAsset galNodeAsset);
    }
}
