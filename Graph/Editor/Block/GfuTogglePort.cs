

using System;
using System.Reflection;
using GalForUnity.Graph.Editor.Builder;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Direction = GalForUnity.Graph.Direction;
using Orientation = GalForUnity.Graph.Orientation;


namespace GalForUnity.Graph.Editor.Block{
    public sealed class GfuTogglePort : BindableElement{
        public GalPort port;
        public GfuTogglePort() : this(null,null,null){ }
        
        public GfuTogglePort(FieldInfo fieldInfo,object instance,Action onUIPreUpdate=null,Action onValueChanged=null,Action onDelete=null){
            if (Nullable.GetUnderlyingType(fieldInfo.FieldType) != null){
                port = GalPort.Create<Edge>(Orientation.Horizontal, Direction.Input, Capacity.Single, Nullable.GetUnderlyingType(fieldInfo.FieldType));
            } else{
                port = GalPort.Create<Edge>(Orientation.Horizontal, Direction.Input, Capacity.Single, fieldInfo.FieldType);
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