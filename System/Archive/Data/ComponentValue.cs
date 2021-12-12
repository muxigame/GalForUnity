using System;
using GalForUnity.System.Address.Addresser;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.System.Archive.Data{
    [Serializable]
    public class ComponentValue:KeyValue{
        public ComponentValue(string name,Component component,object value){
            addressExpression=InstanceIDAddresser.GetInstance().Parse(component);
            if (value is Object o){
                this.value = o;
            }

            this.name = name;
        }
        [SerializeField]
        public string addressExpression;
        [SerializeField]
        public new Object value;

        public Component Value(){
            object obj;
            InstanceIDAddresser.GetInstance().Get(addressExpression,out obj);
            return (Component) obj;
        }
        
        public void Set(object wantSetValue){
            InstanceIDAddresser.GetInstance().Set(addressExpression,wantSetValue);
        }

        public override string ToString(){
            return  name + ":" + addressExpression + ":" + value+"-----";
        }
    }
}