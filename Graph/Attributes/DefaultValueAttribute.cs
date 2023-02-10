

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