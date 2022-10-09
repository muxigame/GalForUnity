// //======================================================================
// //
// //       CopyRight 2019-2021 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  RandomIntensity.cs
// //
// //        Created by 半世癫(Roc) at 2021-04-12 20:47:11
// //
// //======================================================================
//
// using _Project.Script.System;
// using _Project.Script.System.Base;
// using MUX.Support;
//
//
// namespace MUX.LightSupport{
//     public class RandomIntensity : MonoTimeBehavior{
//         private UnityEngine.Rendering.Universal.Light2D _light2d;
//         private UnityEngine.Light _light;
//         public float max = 0.6f;
//         public float min = 1.4f;
//         public float speed = 0.5f;
//         public float intensityMultiplying = 1f;
//
//         public override void NormalStart(){
//             _light2d = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
//             _light = GetComponent<UnityEngine.Light>();
//             //Random(max -UnityEngine.Random.Range(0,max -min));
//         }
//
//         public override void OnDayTime(){
//             _light2d.intensity = 0;
//             SmoothTransition.GetInstance().StopAll();
//         }
//
//         public override void OnNight(){ Random(max - UnityEngine.Random.Range(0, max - min)); }
//
//         public void Random(float source){
//             SmoothTransition.GetInstance().Linear((f => {
//                 if (_light) _light.intensity = f     * intensityMultiplying;
//                 if (_light2d) _light2d.intensity = f * intensityMultiplying;
//             }), (f => {
//                 float _ = max;
//                 max = min;
//                 min = _;
//                 Random(max);
//             }), source, min, speed);
//         }
//     }
// }
