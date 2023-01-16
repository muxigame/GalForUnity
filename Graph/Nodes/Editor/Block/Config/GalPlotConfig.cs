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
using System.Threading.Tasks;
using GalForUnity.Core;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Build;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config{
    [Serializable]
    public class GalPlotConfig : IGalBlock{
        public string name;
        public string word;
        private RuntimeNode _runtimeNode;

        RuntimeNode IGalBlock.RuntimeNode{
            get => _runtimeNode;
            set => _runtimeNode = value;
        }

        public async Task Process(GalCore galCore){
            while (!_runtimeNode.GalGraph.GraphProvider.Next.Invoke()){
                await Task.Yield();
            }
            galCore.SetName(name);
            galCore.SetSay(word);
            await Task.Yield();
        }
    }
}
