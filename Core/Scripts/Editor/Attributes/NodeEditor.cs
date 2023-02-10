using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GalForUnity.Core.Editor.Attributes{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeEditor:Attribute{
        public static Dictionary<Type,Type> Types=new Dictionary<Type, Type>();
        // [UnityEditor.Callbacks.DidReloadScripts]
        static NodeEditor(){
            Types.Clear();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                          .Where(x=>!x.IsDynamic)
                                          // .Where(x=>IsDefined(x,typeof(NodeEditor)))
                                          .SelectMany(x=>x.GetExportedTypes())
                                          .Where(x=>IsDefined(x,typeof(NodeEditor)))){
                Types.Add(type.GetCustomAttribute<NodeEditor>().Type,type);
            }
        }
        public Type Type;

        public static Type GetEditor(Type type){
            if(Types.ContainsKey(type))
                return Types[type];
            return null;
        }
        public NodeEditor(Type type){
            this.Type = type;
        }
    }
}