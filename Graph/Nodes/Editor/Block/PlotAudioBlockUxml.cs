//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotAudioBlock.cs Created at 2022-09-28 00:31:49
//
//======================================================================

using System.Reflection;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor.Block;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class PlotAudioBlockUxml : DraggableBlock{
        private readonly BlockContentUxml _blockContentUxml;
        private readonly BlockPortUxml _blockPortUxml;

        public PlotAudioBlockUxml(){
            GalAudioConfig galAudioConfig=new GalAudioConfig();
            galConfig = galAudioConfig;
            styleSheets.Add(UxmlHandler.instance.gfuTogglePortUss);
            _blockContentUxml = new BlockContentUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + _blockContentUxml.LocalToWorld(transform.position));
                var searchTypeProvider = ConfigSearchTypeProvider.Create<GalAudioConfig>(x => 
                    !x.FieldType.IsSubclassOf(typeof(Object))
                    &&_blockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    &&_blockPortUxml.Content.Q<Port>(x.Name) == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    _blockContentUxml.Content.Add(new GfuConfigFieldUXml(xUserData,galAudioConfig));
                    xUserData.SetValue(galAudioConfig,default);
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });

            _blockPortUxml = new BlockPortUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + _blockPortUxml.LocalToWorld(transform.position));
                var searchTypeProvider = ConfigSearchTypeProvider.Create<GalAudioConfig>(x => 
                    (x.FieldType.IsSubclassOf(typeof(Object)) || x.FieldType.IsNullablePrimitive())
                    &&_blockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    &&_blockPortUxml.Content.Q<Port>(x.Name) == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    var gfuTogglePort = new GfuTogglePort(xUserData, galAudioConfig);
                    _blockPortUxml.Content.Add(gfuTogglePort);
                    xUserData.SetValue(galAudioConfig,default);
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });

            content.Add(_blockPortUxml);
            content.Add(_blockContentUxml);
        }

        public class PlotAudioBlockUxmlFactory : UxmlFactory<PlotAudioBlockUxml, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){ return new PlotAudioBlockUxml(); }
        }


    }
}