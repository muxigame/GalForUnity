﻿using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Core.Block;
using GalForUnity.Core.External;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public class PortableBlockEditor<T> : DraggableBlockEditor where T : IGalConfig{
        protected readonly BlockContentUxml BlockContentUxml;
        protected readonly BlockPortUxml BlockPortUxml;

        public PortableBlockEditor(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){
            GalBlock = (T) galBlock;
            styleSheets.Add(UxmlHandler.instance.gfuTogglePortUss);
            BlockContentUxml = new BlockContentUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + BlockContentUxml.LocalToWorld(transform.position));
                var searchTypeProvider = BlockFieldSearchProvider.Create<T>(x =>
                    x.FieldType.IsSubclassOf(typeof(object))
                    && BlockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    && BlockPortUxml.Content.Q<GalPort>(x.Name)               == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    BlockContentUxml.Content.Add(new GfuConfigFieldUXml(xUserData, GalBlock));
                    xUserData.SetValue(GalBlock, default);
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });
            BlockPortUxml = new BlockPortUxml(() => {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + BlockPortUxml.LocalToWorld(transform.position));
                var searchTypeProvider = BlockFieldSearchProvider.Create<T>(x =>
                    (x.FieldType.IsSubclassOf(typeof(object)) || x.FieldType.IsNullablePrimitive())
                    && BlockContentUxml.Content.Q<GfuConfigFieldUXml>(x.Name) == null
                    && BlockPortUxml.Content.Q<GalPort>(x.Name)               == null);
                searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                    var xUserData = (FieldInfo) x.userData;
                    var gfuTogglePort = new GfuTogglePort(xUserData, GalBlock);
                    BlockPortUxml.Content.Add(gfuTogglePort);
                    xUserData.SetValue(GalBlock, default);
                    return true;
                };
                SearchWindow.Open(searchWindowContext, searchTypeProvider);
            });
            content.Add(BlockPortUxml);
            content.Add(BlockContentUxml);
        }

        public override IEnumerable<(GalPort, GalPortAsset)> OnSavePort(GalNodeAsset galNodeAsset){
            var gfuConfig = (T) GalBlock;
            gfuConfig.Clear();
            BlockContentUxml.Query<GfuConfigFieldUXml>().ForEach(x => {
                gfuConfig.AddField(x.name);
            });
            foreach (var port in BlockPortUxml.Query<GalPort>().ToList()){
                var gfuPortAsset = new GalPortAsset();
                gfuConfig.AddPort(gfuPortAsset);
                gfuPortAsset.Save(port, galNodeAsset);
                yield return (port, gfuPortAsset);
            }
        }


        public override IEnumerable<(GalPortAsset, GalPort)> OnLoadPort(GalNodeAsset galNodeAsset){
            var gfuConfig = (T) GalBlock;
            var type = gfuConfig.GetType();
            gfuConfig.GetField().ForEach(x => {
                var fieldInfo = type.GetField(x);
                BlockContentUxml.Content.Add(new GfuConfigFieldUXml(fieldInfo, gfuConfig));
            });
            foreach (var gfuPortAsset in gfuConfig.GetPort()){
                var fieldInfo = type.GetField(gfuPortAsset.portName);
                var gfuTogglePort = new GfuTogglePort(fieldInfo, gfuConfig);
                BlockPortUxml.Content.Add(gfuTogglePort);
                yield return (gfuPortAsset, gfuTogglePort.port);
            }
        }
    }
}