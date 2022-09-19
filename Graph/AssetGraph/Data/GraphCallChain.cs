//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GraphCallChain.cs
//
//        Created by 半世癫(Roc) at 2021-12-08 23:15:54
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.InstanceID;
using GalForUnity.System.Archive.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Graph.AssetGraph.Data{
    [Serializable]
    public class GraphCallChain{
        [FormerlySerializedAs("CallInfos")] 
        [SerializeField]
        private List<CallInfo> callInfos = new List<CallInfo>();

        public void Add(CallInfo callInfo){
            callInfos.Insert(0,callInfo);
        }
        public CallInfo Pop(){
            var info = callInfos[callInfos.Count-1];
            callInfos.Remove(info);
            return info;
        }
        public CallInfo Peek(){
            var info = callInfos[callInfos.Count -1];
            return info;
        }
        public bool HasNext(){
            return callInfos!=null&&callInfos.Count != 0;
        }
        public bool Next(out CallInfo callInfo){
            callInfo = null;
            var hasNext = HasNext();
            if(hasNext) callInfo = Pop();
            return hasNext;
        }
        
        public override string ToString(){
            string str = "";
            foreach (var callInfo in callInfos){
                str += callInfo + "==>";
            }
            return str.Substring(0,str.Length>3?str.Length-3:str.Length);
        }
    }

    [Serializable]
    public class CallInfo{
        public GraphData CallerGraphData{
            get => _callerGraphData ? _callerGraphData : InstanceIDStorageLoadTool.Load<GraphData>(callerGraphInstanceID);
            set{
                callerGraphInstanceID = value.InstanceID;
                _callerGraphData = value;
            }
        }

        public NodeData CallerNodeData{
            get{
                return _callerNodeData ? _callerNodeData : CallerGraphData.Nodes.Find((x) => x.instanceID == callerNodeInstanceID);
            }
            set{
                callerNodeInstanceID = value.instanceID;
                _callerNodeData = value;
            }
        }

        [NonSerialized]
        private GraphData _callerGraphData;
        [NonSerialized]
        private NodeData _callerNodeData;

        [SaveFlag]
        [SerializeField]
        private long callerGraphInstanceID;
        [SaveFlag]
        [SerializeField]
        private long callerNodeInstanceID;

        public override string ToString(){
            return CallerGraphData.name+"-->"+CallerNodeData.name;
        }
    }
}
