using System;
using GalForUnity.Graph.SceneGraph;

namespace GalForUnity.Graph.Nodes.Runtime{
    public struct PortDefinition{
        public Capacity Capacity;
        public Orientation Orientation;
        public string PortName;
        public Type PortType;

        public static PortDefinition Create(Capacity capacity, Type portType, string portName=null, Orientation orientation=Orientation.Horizontal){
            return new PortDefinition(){
                Capacity = capacity, PortName = string.IsNullOrEmpty(portName)?portType.Name:portName, PortType = portType,Orientation = orientation
            };
        }
    }
}