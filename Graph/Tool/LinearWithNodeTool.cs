//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  LinearWithNodeTool.cs
//
//        Created by 半世癫(Roc) at 2021-01-17 19:06:37
//
//======================================================================

using GalForUnity.System.Event;
using UnityEngine;

namespace GalForUnity.Graph.Tool{
    public class LinearWithNodeTool : MonoBehaviour
    {
        static LinearWithNodeTool(){
            EventCenter.GetInstance().OnPlotItemExecutedEvent+=PlotItemExecuted;
        }
        public static void PlotItemExecuted(){
        }
        
        
    }
}
