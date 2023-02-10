//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DefaultValueAttribute.cs
//
//        Created by 半世癫(Roc) at 2021-02-04 16:18:24
//
//======================================================================

using System;

namespace GalForUnity.Graph.Attributes{
    /// <summary>
    /// 指示一个默认值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DefaultValueAttribute:Attribute{
        public object[] value;
        public DefaultValueAttribute(params object[] value){
            this.value = value;
        }
    }
    
}