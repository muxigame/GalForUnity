//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotVideoBlockUxml.cs 2022-10-11 21:43:32
//
//======================================================================

using System.Reflection;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode;
using GalForUnity.Graph.Block.Config;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class PlotVideoBlockUxml : DraggableBlock{
        private readonly BlockContentUxml _blockContentUxml;
        private readonly BlockPortUxml _blockPortUxml;

        public PlotVideoBlockUxml(){
            styleSheets.Add(UxmlHandler.instance.gfuTogglePortUss);
            _blockContentUxml = new BlockContentUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + _blockContentUxml.LocalToWorld(transform.position));
                var searchTypeProvider = ConfigSearchTypeProvider.Create<GalVideoConfig>(x => 
                    !x.FieldType.IsSubclassOf(typeof(Object))
                    &&_blockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    &&_blockPortUxml.Content.Q<Port>(x.Name)    == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    if (_blockContentUxml.Content.Q<GfuConfigFieldUXml>(xUserData.Name) != null) return true;
                    _blockContentUxml.Content.Add(new GfuConfigFieldUXml(x => { }, xUserData));
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });

            _blockPortUxml = new BlockPortUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + _blockPortUxml.LocalToWorld(transform.position));
                var searchTypeProvider = ConfigSearchTypeProvider.Create<GalVideoConfig>(x =>  
                    (x.FieldType.IsSubclassOf(typeof(Object)) || x.FieldType.IsNullablePrimitive())
                    &&_blockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    &&_blockPortUxml.Content.Q<Port>(x.Name) == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    var gfuTogglePort = new GfuTogglePort(xUserData);
                    _blockPortUxml.Content.Add(gfuTogglePort);
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });

            content.Add(_blockPortUxml);
            content.Add(_blockContentUxml);
        }

        public class PlotVideoBlockUxmlFactory : UxmlFactory<PlotVideoBlockUxml, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){ return new PlotVideoBlockUxml(); }
        }
    }
}