//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MUXInput.cs
//
//        Created by 半世癫(Roc) at 2021-04-14 20:31:24
//
//======================================================================

using System.Collections.Generic;
using MUX.Mono;
using UnityEngine;


namespace MUX{
    public class MuxInput : MonoBehaviour{
        private static Vector2 _axis;
        public static bool differentiateAxial = true;
        private static readonly List<KeyCode> HorizontalKeyCodes=new List<KeyCode>();
        private static readonly List<KeyCode> VerticalKeyCodes=new List<KeyCode>();

        static MuxInput(){
            RunOnMono.Update((() => {
                if (Input.GetKeyDown(KeyCode.A)){
                    HorizontalKeyCodes.Insert(0, KeyCode.A);
                }

                if (Input.GetKeyDown(KeyCode.D)){
                    HorizontalKeyCodes.Insert(0, KeyCode.D);
                }

                if (Input.GetKeyDown(KeyCode.W)){
                    if (differentiateAxial)
                        VerticalKeyCodes.Insert(0, KeyCode.W);
                    else
                        HorizontalKeyCodes.Insert(0, KeyCode.W);
                }

                if (Input.GetKeyDown(KeyCode.S)){
                    if (differentiateAxial)
                        VerticalKeyCodes.Insert(0, KeyCode.S);
                    else
                        HorizontalKeyCodes.Insert(0, KeyCode.S);
                }

                if (HorizontalKeyCodes.Count <= 0) return;

                if (Input.GetKey(HorizontalKeyCodes[0])){
                    if (HorizontalKeyCodes[0] == KeyCode.A) _axis.x = -1;
                    if (HorizontalKeyCodes[0] == KeyCode.D) _axis.x = 1;
                }

                if (differentiateAxial){
                    if (VerticalKeyCodes.Count > 0){
                        if (Input.GetKey(VerticalKeyCodes[0])){
                            if (VerticalKeyCodes[0] == KeyCode.S) _axis.y = -1;
                            if (VerticalKeyCodes[0] == KeyCode.W) _axis.y = 1;
                        }
                    }
                } else{
                    if (Input.GetKey(HorizontalKeyCodes[0])){
                        if (HorizontalKeyCodes[0] == KeyCode.S) _axis.y = -1;
                        if (HorizontalKeyCodes[0] == KeyCode.W) _axis.y = 1;
                    }
                }

                if (Input.GetKeyUp(KeyCode.A)){
                    if (HorizontalKeyCodes[0] == KeyCode.A) HorizontalKeyCodes.RemoveAt(0);
                }

                if (Input.GetKeyUp(KeyCode.D)){
                    if (HorizontalKeyCodes[0] == KeyCode.D) HorizontalKeyCodes.RemoveAt(0);
                }

                if (Input.GetKeyUp(KeyCode.W)){
                    if (differentiateAxial){
                        if (VerticalKeyCodes[0] == KeyCode.D) VerticalKeyCodes.RemoveAt(0);
                    } else{
                        if (HorizontalKeyCodes[0] == KeyCode.D) HorizontalKeyCodes.RemoveAt(0);
                    }
                }

                if (Input.GetKeyUp(KeyCode.S)){
                    if (differentiateAxial){
                        if (VerticalKeyCodes[0] == KeyCode.S) VerticalKeyCodes.RemoveAt(0);
                    } else{
                        if (HorizontalKeyCodes[0] == KeyCode.S) HorizontalKeyCodes.RemoveAt(0);
                    }
                }

                if (HorizontalKeyCodes.Count > 5){
                    HorizontalKeyCodes.RemoveAt(HorizontalKeyCodes.Count - 1);
                }

                if (VerticalKeyCodes.Count > 5){
                    VerticalKeyCodes.RemoveAt(VerticalKeyCodes.Count - 1);
                }

                if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)){
                    _axis.x = 0;
                }
                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)){
                    _axis.y = 0;
                }
            }), true);
        }

        public static float GetAxisRaw(AxisType axisType){
            if (axisType == AxisType.Horizontal) return _axis.x;
            if (axisType == AxisType.Vertical) return _axis.y;
            return 0;
        }
        public static void SetAxisRaw(AxisType axisType,float value){
            if (axisType == AxisType.Horizontal) _axis.x=value;
            if (axisType == AxisType.Vertical) _axis.y=value;
        }

        public static void ClearCache(){
            HorizontalKeyCodes.Clear();
            VerticalKeyCodes.Clear();
        }
    }

    public enum AxisType{
        Horizontal,
        Vertical
    }
}
