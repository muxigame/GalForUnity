//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuPortAsset.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 17:07:38
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Build;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Graph.SceneGraph{
    [Serializable]
    public class GfuPortAsset{
        [SerializeField] public string portName;
        [HideInInspector] [SerializeReference] public GfuNodeAsset node;
        [SerializeReference] public List<GfuConnectionAsset> connections;
        [SerializeField] public byte[] portAttribute = new byte[3];
        public PortType portType;
        public Direction Direction{
            get => (Direction) portAttribute[0];
            set => portAttribute[0] = (byte) value;
        }

        public Orientation Orientation{
            get => (Orientation) portAttribute[1];
            set => portAttribute[1] = (byte) value;
        }

        public Capacity Capacity{
            get => (Capacity) portAttribute[2];
            set => portAttribute[2] = (byte) value;
        }

        [SerializeField] public PortValue value;

        public (T value, bool over) GetValueIfExist<T>(){
            if (Direction == Direction.Input){
                if (value.Value != null) return ((T) value.Value, true);
                return connections?.FirstOrDefault()?.output?.GetValueIfExist<T>() ?? default;
            }

            if (node.runtimeNode is OperationNode operationNode) return operationNode.GetValueFromOutput<T>(Index);
            return default;
        }

        public bool HasConnection => connections != null && connections.Count != 0;

        /// <summary>
        /// Get port index from ports of node
        /// Ports in the block always return -1
        /// </summary>
        public int Index => Direction == Direction.Input ? node.inputPort.IndexOf(this) : node.outputPort.IndexOf(this);

        public static implicit operator bool(GfuPortAsset gfuNode){
            if (gfuNode == null) return false;
            return true;
        }
    }

    public enum Direction:byte{
        Input = 0,
        Output = 1
    }

    public enum Orientation:byte{
        Horizontal = 0,
        Vertical = 1
    }

    public enum Capacity:byte{
        Single = 0,
        Multi = 1,
    }

    [Serializable]
    public class PortType
    {
        [SerializeField]
        private string stringType;

        public static implicit operator PortType(Type type)
        {
            return new PortType() { stringType = string.Concat(type.Namespace, ".", type.Name, ",", type.Assembly.GetName().Name) };
        }
        public static implicit operator Type(PortType type)
        {
            try
            {
                return Type.GetType(type.stringType);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return typeof(object);
        }
    }
}