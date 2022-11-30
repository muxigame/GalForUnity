// //======================================================================
// //
// //       CopyRight 2019-2020 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  RoleController.cs
// //
// //        Created by 半世癫(Roc) at 2021-01-02 21:55:36
// //
// //======================================================================
//
// using System;
// using System.Collections.Generic;
// using GalForUnity.Attributes;
// using GalForUnity.InstanceID;
// using GalForUnity.Model;
// using GalForUnity.System;
// using UnityEngine;
//
// namespace GalForUnity.Controller{
//     /// <summary>
//     /// 角色控制器，管理着当前场景中所有的角色，可通过索引访问每一个角色
//     /// </summary>
//     public class RoleController : MonoBehaviour
//     {
//         // ReSharper disable all MemberCanBePrivate.Global
//         [SerializeField]
//         [Title("RoleSet")]
//         [Rename("RoleIndex")]
//         [Tooltip("角色控制器会自动寻找场景内的所有角色并附加，您也可以手动附加，来获得可控的索引，然后通过角色控制器的索引来访问场景对象")]
//         private List<RoleModel> roleModels=new List<RoleModel>();
//
//         // public List<RoleModel> RoleModels => roleModels;
//
//         void Start(){
//             InitialRoleController();
//             foreach (var roleModel in roleModels){
//                 roleModel.gameObject.SetActive(false);
//             }
//         }
//
//         public void InitialRoleController(){
//             for (int i = roleModels.Count - 1; i >= 0; i--){
//                 roleModels.RemoveAt(i);
//             }
//             var allRoleModel = FindObjectsOfType(typeof(RoleModel));
//             foreach (var obj in allRoleModel){
//                 if (!(obj is RoleModel roleModel)) continue;
//                 if(!roleModels.Contains(roleModel)) roleModels.Add(roleModel);
//                 if(!roleModel.transform.IsChildOf(transform)) roleModel.transform.parent = transform;
//             }
//             if (GameSystem.Data.CurrentRoleModel == null && roleModels != null && roleModels.Count > 0) GameSystem.Data.CurrentRoleModel = roleModels[0];
//         }
//
//         /// <summary>
//         /// 通过GfuInstanceID查找角色数据对象,该查找不通过GfuinstanceID系统，只查找RoleMode列表
//         /// </summary>
//         /// <param name="instanceID"></param>
//         /// <returns></returns>
//         public RoleModel FindRoleModelWithInstanceID(long instanceID){
//             foreach (var roleModel in roleModels){
//                 // if(roleModel.GetComponent<GfuInstance>().instanceID==instanceID) return roleModel;
//             }
//             return null;
//         }
//         
//         /// <summary>
//         /// 向角色控制器添加角色
//         /// </summary>
//         /// <param name="roleModel"></param>
//         public void Add(RoleModel roleModel){
//             for (int i = roleModels.Count - 1; i >= 0; i--){
//                 if (roleModels[0] == null){
//                     roleModels.RemoveAt(i);
//                 }
//             }
//             roleModels.Add(roleModel);
//         }
//         /// <summary>
//         /// 向角色控制器移除角色
//         /// </summary>
//         /// <param name="roleModel"></param>
//         public void Remove(RoleModel roleModel){
//             for (int i = roleModels.Count - 1; i >= 0; i--){
//                 if (roleModels[0] == null){
//                     roleModels.RemoveAt(i);
//                 }
//             }
//             roleModels.Remove(roleModel);
//         }
//         /// <summary>
//         /// 通过所以访问被角色控制器管理的角色
//         /// </summary>
//         /// <param name="index">索引</param>
//         /// <exception cref="ArgumentOutOfRangeException">当索引超出边界时触发</exception>
//         public RoleModel this[int index]{
//             get{
//                 if (index >= 0 && index < roleModels.Count){
//                     return roleModels[index];
//                 }
//                 throw new ArgumentOutOfRangeException();
//             }
//             set{
//                 if (index >= 0 && index < roleModels.Count){
//                     roleModels[index] = value;
//                 }
//                 throw new ArgumentOutOfRangeException();
//             }
//         }
//         /// <summary>
//         /// 通过名称访问被角色控制器管理的角色
//         /// </summary>
//         /// <param name="gameObjectName">传入要访问的角色名称</param>
//         public RoleModel[] this[string gameObjectName]{
//             get{
//                 List<RoleModel> list=new List<RoleModel>();
//                 foreach (var roleModel in roleModels){
//                     if (roleModel.gameObject.name == gameObjectName){
//                         list.Add(roleModel);
//                     }
//                 }
//                 return list.ToArray();
//             }
//         }
//
//         public void AutoHighLight(RoleModel roleModel){
//             foreach (var model in roleModels){
//                 if (roleModel == model){
//                     model.HighLight(true);
//                 }else{
//                     model.HighLight(false);
//                 }
//             }
//         }
//         public void AutoHighLight(string roleName){
//             foreach (var model in roleModels){
//                 if (roleName == model.Name){
//                     model.HighLight(true);
//                 }else{
//                     model.HighLight(false);
//                 }
//             }
//         }
//         
//         public static void AutoHighLight_S(RoleModel roleModel){
//             GameSystem.Data.RoleController.AutoHighLight(roleModel);
//         }
//         public static void AutoHighLight_S(string roleName){
//             GameSystem.Data.RoleController.AutoHighLight(roleName);
//         }
//     }
// }
