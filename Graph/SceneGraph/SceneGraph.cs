//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SceneGraph.cs
//
//        Created by 半世癫(Roc) at 2022-04-13 23:22:10
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Graph.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.GFUNode.Operation;
using UnityEditor;

#endif

namespace GalForUnity.Graph.SceneGraph{
    [ExecuteAlways]
    public class SceneGraph : MonoBehaviour{
        public static UnityEvent<GfuSceneGraphView> OnSceneGraphSave=new UnityEvent<GfuSceneGraphView>();
        [SerializeField] public GfuGraphAsset graph;

        
        public List<GfuNodeData> allNodeData = new List<GfuNodeData>();

        public Dictionary<long, GfuNodeData> nodeDatas = new Dictionary<long, GfuNodeData>();

        // Start is called before the first frame update
        private void Awake(){
            GfuSceneGraphHandler.Register(this);
        }
        private void OnValidate(){ GfuSceneGraphHandler.Register(this); }

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

        private void OnDestroy(){
            GfuSceneGraphHandler.CancelRegister(this);
        }
    }

    [Serializable]
    public class GfuNodeData{
        public long instanceID;
        public List<FieldData<Object>> objects;
        public List<FieldData<string>> values;

        public static implicit operator bool(GfuNodeData gfuNode){
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
    public class SceneGraphEditor : Editor{
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            var sceneGraph = (SceneGraph) target;
            if (GUILayout.Button("创建")){
                NodeType.Init();
                var gfuGraphAsset = GfuGraphCreator.Create();
                GfuSceneGraphHandler.CancelRegister(sceneGraph);
                sceneGraph.graph = gfuGraphAsset;
                GfuSceneGraphHandler.Register(sceneGraph);
            }

            if (GUILayout.Button("打开")) SceneGraphEditorWindow.CreateWindow(sceneGraph);
        }
        public static void Save(SceneGraphEditorWindow sceneGraphEditorWindow){
            var findObjectsOfType = Object.FindObjectsOfType<SceneGraph>();
            foreach (var sceneGraph in findObjectsOfType){
                if (sceneGraph.graph == sceneGraphEditorWindow.gfuGraphAsset){
                    sceneGraph.ClearNodeData();
                    foreach (var keyValuePair in sceneGraphEditorWindow.GraphView.Nodes){
                        var fieldInfos = keyValuePair.Value.GetType().GetFields(BindingFlags.Instance |BindingFlags.Public);
                        var gfuNodeData = new GfuNodeData();
                        gfuNodeData.instanceID = keyValuePair.Key;
                        gfuNodeData.objects = new List<GfuNodeData.FieldData<Object>>();
                        gfuNodeData.values = new List<GfuNodeData.FieldData<string>>();
                        if (keyValuePair.Value is GfuOperationNode gfuOperationNode){
                            for (var i = 0; i < gfuOperationNode.GfuInputViews.Count; i++){
                                var gfuInputView = gfuOperationNode.GfuInputViews[i];
                                ParseData(gfuNodeData,GalGraph.PortViewName+i,gfuInputView.portType,gfuInputView.Value);
                            }
                        }
                        foreach (var fieldInfo in fieldInfos)
                            if (fieldInfo.GetCustomAttribute<NonSerializedAttribute>()==null
                                &&!fieldInfo.FieldType.IsSubclassOf(typeof(GfuPort))
                                &&!(fieldInfo.FieldType==typeof(GfuPort))
                                &&!(fieldInfo.FieldType==typeof(List<GfuPort>))
                                ){
                                ParseData(gfuNodeData, fieldInfo, keyValuePair.Value);
                            }
                        sceneGraph.allNodeData.Add(gfuNodeData);
                    }
                }
            }
            Assembly.GetAssembly(typeof(SceneGraph));
        }

        private static void ParseData(GfuNodeData gfuNodeData,FieldInfo fieldInfo,GfuNode gfuNode){
            ParseData(gfuNodeData,fieldInfo.Name,fieldInfo.FieldType,fieldInfo.GetValue(gfuNode));
            // if (fieldInfo.FieldType.IsSubclassOf(typeof(Object)))
            //     gfuNodeData.objects.Add(new GfuNodeData.FieldData<Object> {
            //         fieldName = fieldInfo.Name, FieldValue = (Object) fieldInfo.GetValue(gfuNode)
            //     });
            // else if (fieldInfo.FieldType.IsPrimitive ||fieldInfo.FieldType ==typeof(string)){
            //     gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
            //         fieldName = fieldInfo.Name, FieldValue = fieldInfo.GetValue(gfuNode)?.ToString()
            //     });
            // }
            // else
            //     gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
            //         fieldName = fieldInfo.Name, FieldValue = JsonUtility.ToJson(fieldInfo.GetValue(gfuNode))
            //     });
        }    
        private static void ParseData(GfuNodeData gfuNodeData,string name,Type type,object value){
            if (type.IsSubclassOf(typeof(Object)))
                gfuNodeData.objects.Add(new GfuNodeData.FieldData<Object> {
                    fieldName = name, FieldValue = (Object) value
                });
            else if (type.IsPrimitive ||type ==typeof(string)){
                gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
                    fieldName = name, FieldValue = value?.ToString()??""
                });
            }
            else
                gfuNodeData.values.Add(new GfuNodeData.FieldData<string> {
                    fieldName = name, FieldValue = JsonUtility.ToJson(value)
                });
        }
    }
#endif
}