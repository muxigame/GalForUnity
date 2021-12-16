//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  RoleData.cs
//
//        Created by 半世癫(Roc)
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalForUnity.InstanceID;
// #if UNITY_EDITOR
// using System.Reflection;
// using System.Reflection.Emit;
// #endif
using GalForUnity.System;
using GalForUnity.System.Archive.Attributes;
using GalForUnity.System.Archive.Behavior;
using GalForUnity.System.Event;
using UnityEngine;

namespace GalForUnity.Model{
    /// <summary>
    /// 角色数据，角色数据存放的类，保存着玩家定义的角色数据
    /// </summary>
    [RequireComponent(typeof(GfuInstance))]
    [Serializable]
    public class RoleData : SavableBehaviour, IEnumerable<RoleDataItem>{

        [SerializeField] 
        [SaveFlag]
        public List<RoleDataItem> dataArray;
        [NonSerialized]
        private readonly Dictionary<string,RoleDataItem> _dataMap = new Dictionary<string, RoleDataItem>();

        public RoleData Data{
            get{
                if (_dataMap.Count != dataArray.Count){
                    _dataMap.Clear();
                    for (var i = 0; i < dataArray.Count; i++){
                        dataArray[i].index = i;
                        _dataMap.Add(dataArray[i].name,dataArray[i]);
                    }
                }
                return this;
            }
        }

        // ReSharper disable all MemberCanBePrivate.Global
        public RoleDataItem GetItem(string itemName){
            return Data._dataMap[itemName];
        }

        public RoleDataItem GetItem(int itemIndex){
            return Data.dataArray[itemIndex];
        }
        /// <summary>
        /// 获得所有的属性名
        /// </summary>
        public string[] Names{
            get{
                return Data._dataMap.Keys.ToArray();
            }
        }
        

        [NonSerialized] public RoleModel RoleModel = null;

        // private void Start() {
        //     //_state = new int[10] { intelligence, strength, inspiration, rhythm, exercise, love, charm, mood, potency, 0 };
        // }

        public int Size(){
            return dataArray.Count;
        }

        public List<RoleDataItem> GetData(){
            return dataArray;
        }

        public RoleData SetData(string itemName, int value){
            this[itemName] = value;
            EventCenter.GetInstance().RoleChangeEvent.Invoke(_dataMap[itemName].index, itemName, value);
            EventCenter.GetInstance().RoleStateChangeEvent.Invoke(RoleModel);
            return this;
        }

        public RoleData SetData(int index, int value){
            this[index] = value;
            EventCenter.GetInstance().RoleChangeEvent.Invoke(index, Names[index], value);
            EventCenter.GetInstance().RoleStateChangeEvent.Invoke(RoleModel);
            return this;
        }
        

        /// <summary>
        /// 比较和某个角色的属性是否相等
        /// </summary>
        /// <param name="x">要进行比较的其他角色</param>
        /// <returns>返回和某个角色的属性是否相等</returns>
        public override bool Equals(object x){
            switch (x){
                case null:
                    return ReferenceEquals(this, null);
                case RoleData otherRoleData:
                    return Array.Equals(otherRoleData, dataArray.ToArray());
                default:
                    return false;
            }
        }

        public override int GetHashCode(){
            return base.GetHashCode();
        }
        /// <summary>
        /// 获得迭代器的方法
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<RoleDataItem> GetEnumerator(){
            
            return new RoleDateEnumerator(dataArray);
        }
        /// <summary>
        /// 获得迭代器的方法
        /// </summary>
        /// <returns>迭代器</returns>
        IEnumerator IEnumerable.GetEnumerator(){
            return new RoleDateEnumerator(dataArray);
        }
        
        public static bool operator ==(RoleData roleModel, RoleData roleModel1){
            return RoleData.Equals(roleModel, roleModel1);
        }
        
        public RoleData Parse(List<RoleDataItem> roleDataItems){
            this.dataArray = roleDataItems;
            foreach (var roleDataItem in roleDataItems){
                this[roleDataItem.index] = roleDataItem.value;
            }
            return this;
        }
        public static List<RoleDataItem> operator +(RoleData roleModel, RoleData roleModel1){
            var min = roleModel.Size() < roleModel1.Size() ? roleModel.Data.dataArray : roleModel1.Data.dataArray;
            var max= roleModel.Size() > roleModel1.Size() ? roleModel.Data._dataMap : roleModel1.Data._dataMap;
            Dictionary<string, RoleDataItem> newData = max.Clone();
            foreach (var roleItem in min){
                if(!newData.ContainsKey(roleItem.name)) throw new ArgumentException();
                newData[roleItem.name].value += roleItem.value;
            }
            return newData.Values.ToList();
        } 
        public static List<RoleDataItem> operator -(RoleData roleModel, RoleData roleModel1){
            var min = roleModel.Size() < roleModel1.Size() ? roleModel.Data.dataArray : roleModel1.Data.dataArray;
            var max= roleModel.Size() > roleModel1.Size() ? roleModel.Data._dataMap : roleModel1.Data._dataMap;
            Dictionary<string, RoleDataItem> newData = max.Clone();
            foreach (var roleItem in min){
                if(!newData.ContainsKey(roleItem.name)) throw new ArgumentException();
                newData[roleItem.name].value -= roleItem.value;
            }
            return newData.Values.ToList();
        }

        public static bool operator !=(RoleData roleModel, RoleData roleModel1){
            return !(roleModel == roleModel1);
        }

        public static bool operator >=(RoleData maxRoleModel, RoleData minRoleModel1){
            if (maxRoleModel.Size() < minRoleModel1.Size()) return false;
            for (int i = 0, length = minRoleModel1.dataArray.Count; i < length; i++){
                if (maxRoleModel[minRoleModel1.Names[i]] < minRoleModel1[i]) return false;
            }

            return true;
        }

        public static bool operator >=(RoleData maxRoleModel, List<RoleDataItem> minRoleModel1){
            if (maxRoleModel.Size() < minRoleModel1.Count) return false;
            for (int i = 0, length = minRoleModel1.Count; i < length; i++){
                if (maxRoleModel[minRoleModel1[i].name] < minRoleModel1[i].value) return false;
            }
            return true;
        }

        public static bool operator <=(RoleData minRoleModel, List<RoleDataItem> maxRoleModel1){
            if (minRoleModel.Size() > maxRoleModel1.Count) return false;
            for (int i = 0, length = minRoleModel.dataArray.Count; i < length; i++){
                RoleDataItem[] value = maxRoleModel1.Where(x => x.name == minRoleModel.Names[i]).ToArray();
                if (value == null||value.Length==0) return false;
                if (value[0].value < minRoleModel[i]) return false;
            }
            return true;
        }

        public static bool operator <=(RoleData minRoleModel, RoleData maxRoleModel1){
            if (minRoleModel.Size() > maxRoleModel1.Size()) return false;
            for (int i = 0, length = minRoleModel.dataArray.Count; i < length; i++){
                //if (!minRoleModel.Names.Contains(maxRoleModel1.Names[i])) continue;
                // Debug.Log(maxRoleModel1[minRoleModel.Names[i]] < minRoleModel[i]);
                // Debug.Log(maxRoleModel1.RoleModel.Name+maxRoleModel1[minRoleModel.Names[i]]);
                // Debug.Log(minRoleModel.name+minRoleModel[i]);
                if (maxRoleModel1[minRoleModel.Names[i]] < minRoleModel[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 返回角色的数值，如果不存在，返回最小值;
        /// </summary>
        /// <param name="empty">角色的属性名</param>
        /// <exception cref="ArgumentException">当尝试设置的属性名为空时，触发异常</exception>
        public int this[string empty]{
            get{
                if (Data._dataMap.ContainsKey(empty)){
                    return _dataMap[empty].value;
                }
                // throw new ArgumentException();
                return Int32.MinValue; //如果不存在，返回最小值;
            }
            set{
                if (Data._dataMap.ContainsKey(empty)){
                    _dataMap[empty].value = value;
                    dataArray[_dataMap[empty].index].value = value;
                    return;
                }
                // throw new ArgumentException();
            }
        }
        /// <summary>
        /// 返回角色的数值，如果不存在，返回最小值;
        /// </summary>
        /// <param name="empty">角色的属性名</param>
        /// <exception cref="ArgumentOutOfRangeException">当尝试设置的属性名为空时，触发异常</exception>
        public int this[int empty]{
            get{
                if (Data.dataArray.Count>empty){
                    return dataArray[empty].value;
                }
                throw new ArgumentOutOfRangeException();
            }
            set{
                dataArray[empty].value = value;
                _dataMap[dataArray[empty].name].value = value;
            }
        }
        /// <summary>
        /// 角色数据的迭代器
        /// </summary>
        private sealed class RoleDateEnumerator : ICloneable, IEnumerator<RoleDataItem>{
            private List<RoleDataItem> _roleDataItems;
            private int _index = -1;
            private RoleDataItem _currentRoleDataItem;

            public RoleDateEnumerator(List<RoleDataItem> roleDataItems){
                this._roleDataItems = (List<RoleDataItem>) roleDataItems.Clone();
            }

            public bool MoveNext(){
                if (_index < _roleDataItems.Count-1&&_roleDataItems.Count>0){
                    // Debug.Log(_roleDataItems[++_index]);
                    _currentRoleDataItem = _roleDataItems[++_index];
                    return true;
                }
                
                _index = _roleDataItems.Count;
                return false;
            }

            public void Reset(){
                _index = -1;
                _currentRoleDataItem = null;
            }

            public RoleDataItem Current{
                get{
                    if (_index == -1){
                        throw new ArgumentOutOfRangeException();
                    }

                    if (_index >= _roleDataItems.Count){
                        throw new ArgumentOutOfRangeException();
                    }

                    return _currentRoleDataItem;
                }
            }

            object IEnumerator.Current => Current;

            public object Clone(){
                return this.MemberwiseClone();
            }

            public void Dispose(){
                if (_roleDataItems != null){
                    _roleDataItems.Clear();
                    _roleDataItems = null;
                }

                _currentRoleDataItem = null;
                _index = -1;

            }
        }

        public override void GetObjectData(){
            
        }

        public override void Recover(){
            
        }
// #if UNITY_EDITOR
//         static AssemblyBuilder myAssemblyBuilder;
//         static ModuleBuilder myModuleBuilder;
//         static EnumBuilder myEnumBuilder;
//         /// <summary>
//         /// EditorMethod 获得当前角色数据类型的枚举
//         /// </summary>
//         /// <returns></returns>
//         private Enum CreateEnum(){
//             if (dataArray == null || dataArray.Count < 1) return null;
//             AssemblyName myAssemblyName = Assembly.GetAssembly(typeof(RoleData)).GetName();
//             myAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Save);
//             myModuleBuilder = myAssemblyBuilder.DefineDynamicModule("EmittedModule", 
//                 "EmittedModule.mod");
//             myEnumBuilder = myModuleBuilder.DefineEnum(typeof(RoleData).Namespace+"."+RoleModel.Name+"Enum", 
//                 TypeAttributes.Public, typeof(Int32));
//             for (var i = 0; i < dataArray.Count; i++){
//                 myEnumBuilder.DefineLiteral(dataArray[i].name, i);
//             }
//             //FieldBuilder myFieldBuilder2 = myEnumBuilder.DefineLiteral("FieldTwo", 2);
//             var type = myEnumBuilder.CreateType();
//             // var values = Enum.GetValues(type);
//             Enum @enum = (Enum) Enum.Parse(type,dataArray[0].name);
//             return @enum;
//         }
// #endif
        
    }

}
