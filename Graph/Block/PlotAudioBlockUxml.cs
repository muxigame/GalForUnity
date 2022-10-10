//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotAudioBlock.cs Created at 2022-09-28 00:31:49
//
//======================================================================

using System.Reflection;
using GalForUnity.Graph.AssetGraph.GFUNode;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class PlotAudioBlockUxml : DraggableBlock{

        public class PlotAudioBlockUxmlFactory : UxmlFactory<PlotAudioBlockUxml, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){
                return new PlotAudioBlockUxml();
            }
        }
        
        private readonly BlockContentUxml _blockContentUxml;

        public PlotAudioBlockUxml(){
            styleSheets.Add(UxmlHandler.instance.gfuTogglePortUss);
            var gfuTogglePort = new GfuTogglePort(typeof(AudioSource));
            _blockContentUxml = new BlockContentUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position+this.LocalToWorld(transform.position));
                var searchTypeProvider = ScriptableObject.CreateInstance<AudioConfigSearchTypeProvider>();
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    if (_blockContentUxml.Content.Q<GfuConfigFieldUXml>(xUserData.Name) != null) return true;
                    _blockContentUxml.Content.Add(new GfuConfigFieldUXml(x => { }, xUserData));
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });
            content.Add(gfuTogglePort);
            content.Add(_blockContentUxml);
        }
    }
}