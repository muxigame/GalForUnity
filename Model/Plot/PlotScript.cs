//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotScript.cs
//
//        Created by 半世癫(Roc) at 2021-11-17 13:50:36
//
//======================================================================

using System;
using GalForUnity.Graph;
using GalForUnity.Graph.AssetGraph;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.GFUNode.Plot;
using GalForUnity.Graph.AssetGraph.Operation;
using UnityEngine;

namespace GalForUnity.Model.Plot{
    /// <summary>
    /// 剧情脚本继承自MonoBehaviour，拥有Mono生命周期，同时可以改在到PlotItemNode等节点上，对剧情进行相关操作。
    /// </summary>
    public class PlotScript : MonoBehaviour{
        [NonSerialized]
        public NodeData NodeData;
        public GfuNode GfuNode{
            get => _gfuNode;
            set{
                //如果想要运行中修改节点在技术上不是不可实现的，请通过修改GraphData中的链接来实现，但是暂时没有提供相关API来执行操作，随意修改极易造成框架异常。
                //TODO 未来会支持对运行时的节点更改，并提供相关API
                if (_gfuNode != null) throw new NotImplementedException("Node information cannot be modified using PlotScript");
                _gfuNode = value;
            }
        }
        private GfuNode _gfuNode;
        [NonSerialized]
        public RoleData RoleData;
        [NonSerialized]
        public PlotModel PlotModel;

        public PlotItemNode PlotItemNode{
            get{
                if (GfuNode is PlotItemNode plotItemNode) return plotItemNode;
                return null;
            }
        }
        
        [Obsolete]
        public virtual void OnPlotItemExecuted(){
            
        }
        public virtual void OnNodeWillExecute(){

        }
        public virtual void OnNodeExecuted(){

        }
        public virtual void OnGfuOperationUpdate(GfuOperation gfuOperation){

        }
        public virtual void OnGfuOperationStart(GfuOperation gfuOperation){

        }
    }

}
