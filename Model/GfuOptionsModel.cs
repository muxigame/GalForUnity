//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuOptionsModel.cs
//
//        Created by 半世癫(Roc) at 2021-11-17 18:47:37
//
//======================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Model{
    public class GfuOptionsModel : MonoBehaviour{
        [SerializeField]
        public GfuOptionData GfuOptionData;

        private void OnEnable(){
            
        }
    }
    [Serializable]
    public class GfuOptionData{
        public int index;
        public string optionContent;
        public GameObject optionContainer;
        public Text text;
    }
    public class GfuOptions{
        public List<GfuOptionData> options;
        public Action<int> OnSelect;
        
        public void Parse(List<string> optionsName){
            options=new List<GfuOptionData>();
            for (var i = 0; i < optionsName.Count; i++){
                options.Add(new GfuOptionData() {
                    index = i,
                    optionContent = optionsName[i]
                });
            }
        }
    }
}
