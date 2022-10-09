// using MUX.Support;
// using UnityEngine;
// using EventType = _Project.Script.EventType;
//
// namespace MUX {
//     /// <summary>
//     /// CopyRight © MUXI Studio 
//     /// Author Roc
//     /// 适用于太阳的脚本，提供太阳的旋转和太阳的旋转周期，即一天的周期，周期使用DailyCycle枚举，信息保存在DailyCycleInfo类中，请用EventCenter.GetInstance().AddEventListeningWithParam("DailyCycleChange",(x)=>{})来监听事件
//     /// 但是这脚本不仅可以给太阳用，事实上，所有对象都可以挂载它
//     /// 经过性能测试，此脚本会造成性能峰值，特别是在昼夜交替的时间段，过快的旋转速度会导致性能峰值，如非必要，请保持旋转速度在0.2f以下
//     /// </summary>
//     public class Sun : MonoBehaviour {
//         /// <summary>
//         /// 是否旋转
//         /// </summary>
//         public bool rotate = true;
//         /// <summary>
//         /// 是否发送事件
//         /// </summary>
//         public bool sendEvent = true;
//         /// <summary>
//         /// 太阳的旋转速度
//         /// </summary>
//         [Range(0, 5)]
//         public float speed = 0.5f;
//         /// <summary>
//         /// 太阳的起始旋转角度
//         /// </summary>
//         public Vector3 startAngle = new Vector3(0, 0, 0);
//         /// <summary>
//         /// 一个正确的旋转符合区间(-180,180)
//         /// </summary>
//         private float X {
//             get {
//                 if (transform.rotation.eulerAngles.z > 179) {
//                     return 180 - transform.rotation.eulerAngles.x;
//                 } else if (transform.rotation.eulerAngles.x > 269) {
//                     return -(360 - transform.rotation.eulerAngles.x);
//                 }
//                 return transform.rotation.eulerAngles.x;
//
//             }
//         }
//         
//
//         private DailyCycleInfo _dailyCycleInfo;
//
//
//         private void Start() {
//             _dailyCycleInfo = new DailyCycleInfo();
//             transform.localEulerAngles = startAngle;
//         }
//         
//         private void Update() {
//             if(rotate) transform.Rotate(Vector3.left * -speed, Space.Self);
//         }
//         private void LateUpdate() {
//             if (sendEvent){
//                 _dailyCycleInfo.normalAngle = X;
//                 _dailyCycleInfo.eulerAngles = transform.rotation.eulerAngles;
//                 DailyCycle();
//             }
//         }
//
//         private void DailyCycle() {
//             lock (this) {
//                 if (_dailyCycleInfo.normalAngle >= -10 && _dailyCycleInfo.normalAngle < 10) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Dawn) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Dawn;
//
//                 } else if (_dailyCycleInfo.normalAngle >= 10 && _dailyCycleInfo.normalAngle < 30) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Morning) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Morning;
//
//                 } else if (_dailyCycleInfo.normalAngle >= 30 && _dailyCycleInfo.normalAngle < 70) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Forenoon) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Forenoon;
//
//                 } else if (_dailyCycleInfo.normalAngle >= 70 && _dailyCycleInfo.normalAngle < 110) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Noon) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Noon;
//
//                 } else if (_dailyCycleInfo.normalAngle >= 110 && _dailyCycleInfo.normalAngle < 150) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Afternoon) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Afternoon;
//
//                 } else if (_dailyCycleInfo.normalAngle >= 150 && _dailyCycleInfo.normalAngle < 170) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Dusk) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Dusk;
//
//                 } else if ((_dailyCycleInfo.normalAngle >= 170 && _dailyCycleInfo.normalAngle <= 182) || (_dailyCycleInfo.normalAngle >= -182 && _dailyCycleInfo.normalAngle < -120)) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Night) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Night;
//
//                 } else if (_dailyCycleInfo.normalAngle >= -120 && _dailyCycleInfo.normalAngle < -60) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Midnight) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Midnight;
//
//                 } else if (_dailyCycleInfo.normalAngle >= -60 && _dailyCycleInfo.normalAngle < -10) {
//                     if (_dailyCycleInfo.dailyCycle == MUX.DailyCycle.Latenight) return;
//                     _dailyCycleInfo.dailyCycle = MUX.DailyCycle.Latenight;
//
//                 }
//                 EventCenter.GetInstance().SendEventWithParam(EventType.DailyCycleChange, _dailyCycleInfo);
//             }
//         }
//
//     }
//     public enum DailyCycle {
//         /// <summary>
//         /// 黎明
//         /// </summary>
//         Dawn,
//         /// <summary>
//         /// 清晨
//         /// </summary>
//         Morning,
//         /// <summary>
//         /// 上午
//         /// </summary>
//         Forenoon,
//         /// <summary>
//         /// 中午
//         /// </summary>
//         Noon,
//         /// <summary>
//         /// 下午
//         /// </summary>
//         Afternoon,
//         /// <summary>
//         /// 傍晚
//         /// </summary>
//         Dusk,//
//         /// <summary>
//         /// 晚上
//         /// </summary>
//         Night,//
//         /// <summary>
//         /// 半夜
//         /// </summary>
//         Midnight,
//         /// <summary>
//         ///深夜
//         /// </summary>
//         Latenight
//     }
//     public class DailyCycleInfo {
//         /// <summary>
//         /// 当前时间的描述
//         /// </summary>
//         public DailyCycle dailyCycle;
//         /// <summary>
//         /// 太阳的旋转角在区间内(-180,180)
//         /// </summary>
//         public float normalAngle;
//         /// <summary>
//         /// 太阳的欧拉角在区间内(0,360)
//         /// </summary>
//         public Vector3 eulerAngles;
//     }
// }
