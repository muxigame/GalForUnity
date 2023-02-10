

using System.Collections.Generic;
using GalForUnity.Graph.Editor.Builder;

namespace GalForUnity.Graph.Editor.Block{
    public interface IGalBlockEditor{
        public IEnumerable<(GalPort, GalPortAsset)> OnSavePort(GalNodeAsset galNodeAsset);
        public IEnumerable<(GalPortAsset,GalPort)> OnLoadPort(GalNodeAsset galNodeAsset);
    }
}
