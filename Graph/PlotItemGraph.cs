//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotItemGraph.cs
//
//        Created by 半世癫(Roc) at 2021-01-26 13:57:13
//
//======================================================================

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.Graph.Data;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GalForUnity.Graph{
    [Serializable]
    public class PlotItemGraph : GfuGraph{
        public PlotItemGraph(){
            GameSystem.GraphData.currentGraph = this;
        }

        public PlotItemGraph(GraphCallChain graphCallChain):base(graphCallChain){
            
        }
        public PlotItemGraph(Data.GraphData graphData) : base(graphData){
            GameSystem.GraphData.currentGraph = this;
#if UNITY_EDITOR
            EditorWindow = GameSystem.GraphData.currentGfuPlotItemEditorWindow;
#endif
        }

#if UNITY_EDITOR
        public PlotItemGraph(EditorWindow editorWindow, string path) : base(path){
            EditorWindow = editorWindow;
            Init();
        }
#endif
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected PlotItemGraph(SerializationInfo info, StreamingContext context)
        {
            // #region 如果基类也实现了ISerializable接口，则序列化器会自动调用基类的该构造函数，就不需要本段代码
            //             Type basetype = this.GetType().BaseType;
            //             MemberInfo[] mi = FormatterServices.GetSerializableMembers(basetype, context);
            //             for (int i = 0; i < mi.Length; i++)
            //             {
            //                 //由于AddValue不能添加重名值，为了避免子类变量名与基类变量名相同，将基类序列化的变量名加上基类类名
            //                 FieldInfo fi = (FieldInfo)mi[0];
            //                 object objValue = info.GetValue(basetype.FullName + "+" + fi.Name, fi.FieldType);
            //                 fi.SetValue(this, objValue);
            //             }
            // #endregion
            // Type type = GetType();
            // var serializationInfoEnumerator = info.GetEnumerator();
            // while (serializationInfoEnumerator.MoveNext()){
            //     type.GetField(serializationInfoEnumerator.Name).SetValue(this,serializationInfoEnumerator.Value);
            // }
        }
    }
}
