using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Graph.Data{
    public interface DataInfo{
        List<NodeData.NodeFieldInfo> jsonField{ get; set; }
        List<NodeData.NodeFieldInfo> idField{ get; set; }
        
        List<NodeData.ListData> listField{ get; set; }
    }
    
}