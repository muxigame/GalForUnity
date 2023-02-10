


using GalForUnity.Graph.Editor.Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR

#endif

namespace GalForUnity.Graph{
    public class SceneGraph : MonoBehaviour, IGalGraph{
        public static UnityEvent<GalGraphView> OnSceneGraphSave = new UnityEvent<GalGraphView>();

        [SerializeField] private GalGraphAsset graphNode;
        

        public GalGraphAsset GraphNode{
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
            if (GUILayout.Button("创建")){
                
            }

            if (GUILayout.Button("打开")) 
                GalGraphWindow.Open(sceneGraph);
        }
    }
#endif
}