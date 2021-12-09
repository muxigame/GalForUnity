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
using UnityEngine;

namespace GalForUnity.Graph.Data{
    [Serializable]
    public class GraphCallChain{
        [SerializeField]
        private List<CallInfo> CallInfos = new List<CallInfo>();

        public void Add(CallInfo callInfo){
            CallInfos.Insert(0,callInfo);
        }
        public CallInfo Pop(){
            var info = CallInfos[CallInfos.Count-1];
            CallInfos.Remove(info);
            return info;
        }
        public CallInfo Peek(){
            var info = CallInfos[CallInfos.Count -1];
            return info;
        }
        public bool HasNext(){
            return CallInfos!=null&&CallInfos.Count != 0;
        }
        public bool Next(out CallInfo callInfo){
            callInfo = null;
            if(HasNext()) callInfo = Pop();
            return HasNext();
        }
        
        public override string ToString(){
            string str = "";
            foreach (var callInfo in CallInfos){
                str += callInfo + "==>";
            }
            return str.Substring(0,str.Length>3?str.Length-3:str.Length);
        }
    }

    [Serializable]
    public class CallInfo{
        public ScriptableObject callerGraphData;
        public ScriptableObject callerNodeData;
        public ScriptableObject callToGraphData;

        public override string ToString(){
            return callerGraphData.name+"-->"+callerNodeData.name;
        }
    }
}
