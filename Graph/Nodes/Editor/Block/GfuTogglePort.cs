//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuTogglePort.cs Created  at 2022-09-27 00:08:03
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Block;
using GalForUnity.Graph.SceneGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Direction = GalForUnity.Graph.SceneGraph.Direction;
using Orientation = GalForUnity.Graph.SceneGraph.Orientation;


namespace GalForUnity.Graph.AssetGraph.GFUNode{
    public sealed class GfuTogglePort : BindableElement{
        public GfuPort port;
        public GfuTogglePort() : this(null,null,null){ }
        
        public GfuTogglePort(FieldInfo fieldInfo,object instance,Action onUIPreUpdate=null,Action onValueChanged=null,Action onDelete=null){
            if (Nullable.GetUnderlyingType(fieldInfo.FieldType) != null){
                port = GfuPort.Create<Edge>(Orientation.Horizontal, Direction.Input, Capacity.Single, Nullable.GetUnderlyingType(fieldInfo.FieldType));
            } else{
                port = GfuPort.Create<Edge>(Orientation.Horizontal, Direction.Input, Capacity.Single, fieldInfo.FieldType);
            }
            port.portName = fieldInfo.Name;
            port.name = fieldInfo.Name;
            var toggle = new Toggle {
                value = true
            };
            // toggle.CreateBinder(fieldInfo, instance,onUIPreUpdate,onValueChanged);
            contentContainer.Add(port);
            contentContainer.Add(toggle);
            styleSheets.Add(UxmlHandler.instance.gfuTogglePortUss);
        }

        public class GfuTogglePortUxmlFactory : UxmlFactory<GfuTogglePort, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}