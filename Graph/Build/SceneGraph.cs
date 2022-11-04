//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SceneGraph.cs Created at 2022-04-13 23:22:10
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Graph.Build;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.GFUNode.Operation;
using UnityEditor;
#endif

namespace GalForUnity.Graph.SceneGraph{
    public class SceneGraph : MonoBehaviour,IGalGraph{
        [SerializeField]
        private GfuGraphAsset graphNode;
        [SerializeField]
        private GraphData graphData;

        public GfuGraphAsset GraphNode{
            get => graphNode;
            set => graphNode = value;
        }

        public GraphData GraphData{
            get => graphData;
            set => graphData = value;
        }

        private void Awake(){
            
        }

        public static UnityEvent<GfuSceneGraphView> OnSceneGraphSave=new UnityEvent<GfuSceneGraphView>();
        
    }

    [Serializable]
    public class GraphData{
        public List<GfuNodeData> allNodeData = new List<GfuNodeData>();

        public Dictionary<long, GfuNodeData> nodeDatas = new Dictionary<long, GfuNodeData>();
        public GfuNodeData GetNodeData(long instanceID){
            if (allNodeData == null) return null;
            if (nodeDatas == null || nodeDatas.Count != allNodeData.Count){
                nodeDatas = new Dictionary<long, GfuNodeData>();
                foreach (var gfuNodeData in allNodeData) nodeDatas.Add(gfuNodeData.instanceID, gfuNodeData);
            }

            if (!nodeDatas.ContainsKey(instanceID)) return null;
            return nodeDatas[instanceID];
        }       
        public void ClearNodeData(){
            allNodeData.Clear();
            nodeDatas.Clear();
        }
        public void Save(GfuSceneGraphView graphView){
            this.ClearNodeData();
            foreach (var keyValuePair in graphView.Nodes){
                var fieldInfos = keyValuePair.Value.GetType().GetFields(BindingFlags.Instance |BindingFlags.Public);
                var gfuNodeData = new GfuNodeData();
                gfuNodeData.instanceID = keyValuePair.Key;
                gfuNodeData.objects = new List<GfuNodeData.FieldData<Object>>();
                gfuNodeData.values = new List<GfuNodeData.FieldData<string>>();
                if (keyValuePair.Value is GfuOperationNode gfuOperationNode){
                    for (var i = 0; i < gfuOperationNode.GfuInputViews.Count; i++){
                        var gfuInputView = gfuOperationNode.GfuInputViews[i];
                        ParseData(gfuNodeData,GalGraph.PortViewName +i,gfuInputView.portType,gfuInputView.Value);
                    }
                }
                foreach (var fieldInfo in fieldInfos)
                    if (fieldInfo.GetCustomAttribute<NonSerializedAttribute>() ==null
                        &&!fieldInfo.FieldType.IsSubclassOf(typeof(GfuPort))
                        &&!(fieldInfo.FieldType ==typeof(GfuPort))
                        &&!(fieldInfo.FieldType ==typeof(List<GfuPort>))
                    ){
                        ParseData(gfuNodeData, fieldInfo, keyValuePair.Value);
                    }
                allNodeData.Add(gfuNodeData);
            }
        }
        private static void ParseData(GfuNodeData gfuNodeData,FieldInfo fieldInfo,GfuNode gfuNode){
            ParseData(gfuNodeData,fieldInfo.Name,fieldInfo.FieldType,fieldInfo.GetValue(gfuNode));
        }    
        private static void ParseData(GfuNodeData gfuNodeData,string name,Type type,object value){
            if (type.IsSubclassOf(typeof(Object)))
                gfuNodeData.objects.Add(new GfuNodeData.FieldData<Object> {
                    fieldName = name, FieldValue = (Object) value
                });
            else if (type.IsPrimitive ||type ==typeof(string)){
                gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
                    fieldName = name, FieldValue = value?.ToString() ??""
                });
            }
            else
                gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
                    fieldName = name, FieldValue = JsonUtility.ToJson(value)
                });
        }
    }
    [Serializable]
    public class GfuNodeData{
        public long instanceID;
        public List<FieldData<Object>> objects;
        public List<FieldData<string>> values;

        public static implicit operator bool(GfuNodeData gfuNode){
            SerializedObject serializedObject;
            if (gfuNode == null) return false;
            return true;
        }
        [Serializable]
        public class FieldData<T>{
            public string fieldName;
            public T FieldValue;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SceneGraph))]
    public class SceneGraphEditor : UnityEditor.Editor{
        public override void OnInspectorGUI(){
            this.serializedObject.Update();
            base.OnInspectorGUI();
            var sceneGraph = (SceneGraph) target;
            if (GUILayout.Button("创建")){

            }

            if (GUILayout.Button("打开")) GalGraphWindow.Open(sceneGraph);
        }
    }
#endif
}