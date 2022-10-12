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
using GalForUnity.Graph.Block;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.AssetGraph.GFUNode{
    public class GfuTogglePort : VisualElement{
        public GfuTogglePort() : this(null){ }

        public GfuTogglePort(FieldInfo fieldInfo){
            Port port;
            if (Nullable.GetUnderlyingType(fieldInfo.FieldType) != null){
                port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, Nullable.GetUnderlyingType(fieldInfo.FieldType));
            } else{
                port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, fieldInfo.FieldType);
            }
            port.portName = fieldInfo.Name;
            port.name = fieldInfo.Name;
            var toggle = new Toggle {
                value = true
            };
            toggle.RegisterValueChangedCallback(x => { port.SetEnabled(x.newValue); });
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