using System;
using GalForUnity.System.Address;
using GalForUnity.System.Address.Addresser;
using UnityEngine;

namespace GalForUnity.System.Archive.Data{
    [Serializable]
    public class ComponentValue:KeyValue{
        public ComponentValue(string name,Component component,object value):base(name,value){
            addressExpression=InstanceIDAddresser.GetInstance().Parse(component);
        }
        [SerializeField]
        public string addressExpression;

        public Component Value(){
            object obj;
            InstanceIDAddresser.GetInstance().Get(addressExpression,out obj);
            return (Component) obj;
        }
        
        public void Set(object wantSetValue){
            InstanceIDAddresser.GetInstance().Set(addressExpression,wantSetValue);
        }
    }
}