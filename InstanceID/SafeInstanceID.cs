// //======================================================================
// //
// //       CopyRight 2019-2021 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  SafeInstanceID.cs
// //
// //        Created by 半世癫(Roc) at 2021-11-21 10:37:45
// //
// //======================================================================
//
// using GalForUnity.System;
// using UnityEngine;
//
// namespace GalForUnity.InstanceID{
//     [ExecuteAlways]
//     public class SafeInstanceID : MonoBehaviour{
//         private GfuInstance _gfuInstance;
//         private void Update(){
//             if (!transform.parent) return;
//             if (!_gfuInstance&&transform.parent.TryGetComponent(out GfuInstance gfuInstance)) _gfuInstance = gfuInstance;
//             if (_gfuInstance){
//                 // _gfuInstance.instanceID = long.Parse(name);
//             }
//         }
//     }
// }
