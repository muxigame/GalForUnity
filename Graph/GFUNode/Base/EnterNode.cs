//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GEnterNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-13 21:50:35
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Model;

namespace GalForUnity.Graph.GFUNode.Base{
    public class EnterNode : GfuNode
    {
#if UNITY_EDITOR
        [NodeRename(nameof(Enter),typeof(RoleData),NodeDirection.Input,NodeCapacity.Multi)]
        public GfuPort Enter;
#endif
        
    }
}
