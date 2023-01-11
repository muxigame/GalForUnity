//======================================================================
//
//       CopyRight 2019-2023 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalPlotConfig.cs
//
//        Created by 半世癫(Roc) at 2023-01-11 20:35:03
//
//======================================================================

using System;
using GalForUnity.Core;
using GalForUnity.Graph.Block.Config;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config{
    [Serializable]
    public class GalPlotConfig : IGalBlock{
        public string name;
        public string word;
        public void Process(GalCore galCore){
            galCore.SetName(name);
            galCore.SetSay(word);
        }
    }
}
