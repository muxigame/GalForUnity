using System;
using UnityEngine;

namespace MUX.Support{
    public class JQMono : MonoBehaviour {
        // Start is called before the first frame update
        public GameObject Q(string str){
            if (str.Contains("#")) {
                return  GameObject.Find(str.Substring(1));
            }else {
                throw new Exception();
            }

        }
    }
}
