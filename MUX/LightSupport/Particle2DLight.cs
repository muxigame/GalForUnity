// //======================================================================
// //
// //       CopyRight 2019-2021 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  Particle2DLight.cs
// //
// //        Created by 半世癫(Roc) at 2021-04-12 21:27:19
// //
// //======================================================================
//
// using System.Collections.Generic;
// using _Project.Script.System;
// using _Project.Script.System.Base;
// using UnityEngine;
//
//
//
// namespace MUX.LightSupport{
//     public class Particle2DLight : MonoBehaviour{
//         private readonly List<GameObject> _light2Ds = new List<GameObject>();
//         private ParticleSystem _particleSystem;
//         public float intensityMultiplying = 2f;
//
//         public override void NormalAwake(){ _particleSystem = GetComponent<ParticleSystem>(); }
//
//         public override void OnDayTime(){
//             if (!_particleSystem.isStopped) _particleSystem.Stop();
//         }
//
//         public override void OnNight(){
//             if (_particleSystem.isStopped || _particleSystem.isPaused) _particleSystem.Play();
//         }
//
//         public override void NightUpdate(){
//             ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_particleSystem.particleCount];
//             var particleCount = _particleSystem.GetParticles(particles);
//             for (var i = 0; i < particles.Length; i++){
//                 GameObject gObject;
//                 UnityEngine.Rendering.Universal.Light2D light2D1;
//                 if (_light2Ds.Count <= i){
//                     gObject = new GameObject();
//                     light2D1 = gObject.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
//                     light2D1.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
//                     _light2Ds.Add(gObject);
//                 } else{
//                     gObject = _light2Ds[i];
//                     light2D1 = gObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
//                 }
//
//                 gObject.transform.position = particles[i].position;
//                 light2D1.intensity = particles[i].GetCurrentSize(_particleSystem) * intensityMultiplying;
//             }
//         }
//     }
// }
