//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SceneGraph.cs Created at 2022-04-13 23:22:10
//
//======================================================================

using GalForUnity.Graph.Build;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace GalForUnity.Graph.SceneGraph{
    public class SceneGraph : MonoBehaviour, IGalGraph{
        public static UnityEvent<GfuSceneGraphView> OnSceneGraphSave = new UnityEvent<GfuSceneGraphView>();

        [SerializeField] private GfuGraphAsset graphNode;

        public GfuGraphAsset GraphNode{
            get => graphNode;
            set => graphNode = value;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(SceneGraph))]
    public class SceneGraphEditor : UnityEditor.Editor{
        public override void OnInspectorGUI(){
            serializedObject.Update();
            base.OnInspectorGUI();
            var sceneGraph = (SceneGraph) target;
            if (GUILayout.Button("创建")){ }

            if (GUILayout.Button("打开")) GalGraphWindow.Open(sceneGraph);
        }
    }
#endif
}