//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DoubleExitNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-16 19:28:25
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Model;

namespace GalForUnity.Graph.GFUNode.Base{
    public class DoubleExitNode : EnterNode
    {
        [NodeRename(nameof(Exit)+"1", typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;

        [NodeRename(nameof(Exit2), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit2;
    }
}
