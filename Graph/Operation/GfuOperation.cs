//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuOperation.cs
//
//        Created by 半世癫(Roc) at 2021-01-27 18:07:16
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.GFUNode.Operation;
using GalForUnity.System;
using GalForUnity.System.Event;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph.Operation{
    /// <summary>
    /// GfuOperation是GalForUnity的节点操作的父类，提供同步或者异步的进行节点操作功能
    /// 越内层的操作节点执行的越优先，操作和节点自己相反，需要反向回调，而非正向执行
    /// </summary>
    public class GfuOperation{
        /// <summary>
        /// 该布尔值指示该操作为同步或者异步
        /// </summary>
        public bool IsSync = false;
        /// <summary>
        /// 该布尔值指示该操作是否已经结束
        /// </summary>
        public bool IsOver = false;
        /// <summary>
        /// 该布尔值指示该操作是否正在帧更新
        /// </summary>
        public bool IsExecute = false;
        /// <summary>
        /// 解决帧内执行的优先级,越高越优先执行
        /// </summary>
        public int Priority = -1;
        /// <summary>
        /// Input传递给所有的父节点，父节点依据情况更改值，所有父节点持有的都是同一个Input。当前节点则根据Input操作OutPut
        /// </summary>
        public GfuOperationData Input;
        
        public Action<GfuOperation> OnInit;
        public Action<GfuOperation> OnStart;
        public Action<GfuOperation> OnUpdate;
        public Action<GfuOperation> OnPostInput;
        
        protected GfuNode gfuNode;

        /// <summary>
        /// 对于没有Input的节点可以理解为改节点为最终根节点
        /// </summary>
        public virtual List<Data> InputData{
            get => Input?.Data;
            set{
                Input = Input??new GfuOperationData();
                Input.Data = value;
            }
        }
        public GfuOperationData OutPut;
        
        /// <summary>
        /// OutPut，实际上这个参数是从根节点反向传播的来的，每个节点都会将当前节点的Input传递给根节点的OutPut
        /// </summary>
        public virtual List<Data> OutPutData{
            get{
                OutPut = OutPut ?? new GfuOperationData();
                return OutPut?.Data;
            }
            set{
                OutPut = OutPut??new GfuOperationData();
                OutPut.Data = value;
            }
        }
        public GfuOperationData Container;
        
        /// <summary>
        /// OutPut，实际上这个参数是从根节点反向传播的来的，每个节点都会将当前节点的Input传递给根节点的OutPut
        /// </summary>
        public virtual List<Data> ContainerData{
            get{
                Container = Container ?? new GfuOperationData();
                return Container?.Data;
            }
            set{
                Container = Container??new GfuOperationData();
                Container.Data = value;
            }
        }

        public GfuOperation(){
            IsOver = false;
        }
        public GfuOperation(GfuOperationData gfuOperationData){
            IsOver = false;
        }
        /// <summary>
        /// 开始该操作，同时需要当前节点和节点操作数据,以及当前节点执行的优先级，该方法会初始化操作节点，并且将执行操作按
        /// 优先级加入下一个MonoUpdate中，
        /// </summary>
        /// <param name="priority">当前节点执行的优先级</param>
        /// <param name="data">操作数据</param>
        /// <param name="node">关联当前节点的其他节点</param>
        public void Start(int priority,Data data,GfuNode node){
            IsOver = false;
            OnInit?.Invoke(this);
            gfuNode = node;
            Priority = Mathf.Max(priority,Priority);
            EventCenter.GetInstance().OnNodeExecutedEvent+=RegisterExecuted;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += (x) => {
                if (x == PlayModeStateChange.ExitingPlayMode){
                    IsOver = true;
                    GfuRunOnMono.Clear(GfuMethodType.Update);
                }
            };
#endif
            if (data != null){
                //Start端口会因为多次链接而进入,每次传入都会为OutPutData增加数据原,节点因该为所有需要数据的节点赋值
                OutPutData.Add(data);
            }
            OnPostInput?.Invoke(this);
            //有默认值的端口，默认值会覆盖端口原先的值，
            //不存在默认值有两种情况，一种是Unity引用类型，即Object的继承类型，在获取默认值的时候为null，则会不赋值，保持原有的值
            //另一种是端口已经连接，则会使用已经连接的值类型或者是需要使用已经连接的值类型
            if (node is GfuOperationNode gfuOperationNode){
                for (var i = 0; i < node.InputPortCount; i++){
                    // Debug.Log(node+"==>"+this+Input.Data.Count);
                    var defaultValue = gfuOperationNode.GetDefaultValue(i);
                    if(gfuOperationNode.IsInputConnected(i)) continue;
                    Input.Data[i].IsOver = true;
                    if (defaultValue != null){
                        Input.Data[i].value = defaultValue;
                    }
                }
                
            }
            //遍历当前节点的所有父节点,并且依据优先级开启线程。数字越大越优先执行,会将所有数据传入到连入段的节点里去
            if (node.nodeData.InputPort != null && node.nodeData.InputPort.Count > 0){
                for (var i = 0; i < node.nodeData.InputPort.Count; i++){
                    var portData = node.nodeData.InputPort[i];
                    if(portData?.connections ==null ||portData.connections.Count <=0) continue;
                    foreach (var portDataConnection in portData.connections){
                        InputData[i].outportIndex=portDataConnection.outputIndex;
                        if(portDataConnection?.Output == null) continue;
                        if (node.GfuGraph.GetNode(portDataConnection.Output.instanceID) is GfuOperationNode parentGfuOperationNode){
                            // if (!parentGfuOperationNode.GfuOperation.IsOver) continue;
                            parentGfuOperationNode.GfuOperation.Start(Priority +1,Input.Data[i],parentGfuOperationNode);
                        }
                    }
                }
            }

            var execute = Execute(Input);//Input不一定存在，可能为一个默认值,默认值则来源上放的默认值算法
            if (IsSync){
                execute.Wait();//如果当前方法需要同步执行则阻塞当前线程
            }
        }

        
        /// <summary>
        /// 执行该节点的操作，操作会被异步执行，同时被加入Mono的生命周期中执行
        /// </summary>
        /// <param name="gfuOperationData"></param>
        /// <returns></returns>
        public virtual async Task Execute(GfuOperationData gfuOperationData){
            await Task.Run(delegate{
                bool isFirst = true;
                while (!IsOver){
                    if (GfuRunOnMono.IsLateUpdate&&!IsExecute){
                        IsExecute = true;
                        GfuRunOnMono.Update(Priority,delegate{
                            if(!IsOver&&!isFirst) Update(gfuOperationData);
                            if (!IsOver && isFirst){
                                isFirst = false;
                                OnStart?.Invoke(this);
                                Start(gfuOperationData);
                            }
                        });
                    }
                    Thread.Sleep(1);
                }
            });
        }

        /// <summary>
        /// GfuOperation的核心：首帧操作操作，需要在该方法内执行操作，且该方法是在Update中执行的,并且在执行完毕的时候调用Executed();,
        /// 否在会一直处于执行状态
        /// 如果第一帧就被停止了，那么至少会执行一次Update。
        /// 对于某些只执行一次或者初始化操作应放在Start中执行
        /// </summary>
        public virtual void Start(GfuOperationData gfuOperationData){
            if (gfuOperationData.IsOver){
                Update(gfuOperationData);
                //在以后的版本则不会执行了
            }else{
                IsExecute = false;
            }
            
        }
        /// <summary>
        /// GfuOperation的核心：操作的帧更新，需要在该方法内执行操作，且该方法是在Update中执行的,并且在执行完毕的时候调用Executed();,
        /// 否在会一直处于执行状态
        /// GfuOperation的方法是每次执行的时候判断父节点是否全部停止，如果父节点全部停止，则该节点停止，才默写线性过渡操作中，应该重写判定方法
        /// </summary>
        public virtual void Update(GfuOperationData gfuOperationData){
            OnUpdate?.Invoke(this);
            if(gfuOperationData.IsOver) Executed();
            IsExecute = false;
        }

        /// <summary>
        /// 需要在该方法中执行完结操作，如当前节点被快进而导致异步方法无法如期执行完成时，
        /// 执行此方法可以防止操作中断带来的后果，同时因保证该方法执行后和操作最后一次的执行的结果不能相差过大，
        /// 实际上他们应该为同一个操作
        /// </summary>
        public virtual void OperationOver(){
            // Debug.Log(Priority+":"+gfuNode);
            //所有父节点都已经停止工作，才会停止当前节点
            if (!IsOver){
                IsOver = true;
            }
            foreach (var data in InputData){
                data.IsOver = true;
            }
            foreach (var data in OutPutData){
                data.IsOver = true;
            }
            
        }

        public virtual void RegisterExecuted(GfuNode node){
            Executed();
        }

        /// <summary>
        /// 操作执行完毕时用户主动触发，会触发一个操作执行完成的事件
        /// </summary>
        public virtual void Executed(){
            // ReSharper disable once DelegateSubtraction
            EventCenter.GetInstance().OnNodeExecutedEvent-=RegisterExecuted;
            GfuRunOnMono.LateUpdate(Priority, delegate{
                if(!IsOver) OperationOver();
                // InputData.Clear();//这个行代码会导致节点执行完成之后InputData数据丢失，这会导致上层API选择性执行节点无法执行已经执行过的节点时，无法访问节点数据。
                // OutPutData.Clear();//这个行代码会导致节点执行完成之后InputData数据丢失，这会导致上层API选择性执行节点无法执行已经执行过的节点时，无法访问节点数据。
                //所以这里就清楚数据，图结束的时候再清楚
            });
            EventCenter.GetInstance().OnOperationExecutedEvent.Invoke(this);
        }
    }

    /// <summary>
    /// 在节点中流动的参数
    /// </summary>
    public class GfuOperationData{

        public bool AutoOver = false;
        /// <summary>
        /// 该布尔值指示该父节点的操作是否已经结束
        /// </summary>
        public bool IsOver => (Data.Count != 0 && Data.TrueForAll((x)=>x.IsOver))||AutoOver;
        
        /// <summary>
        /// 必须有一个无参构造方法该方法为节点操作指定默认值
        /// </summary>
        protected GfuOperationData(){
            Data=new List<Data>();
        }

        public GfuOperationData(List<Data> data){
            Data = data;
            foreach (var data1 in Data){
                data1.gfuOperationData = this;
            }
        }
        public GfuOperationData(params Data[] datas){
            Data = new List<Data>();
            foreach (var data in datas){
                data.gfuOperationData = this;
                Data.Add(data);
            }
        }
        public List<Data> Data;
        public void IndexOf(Data data) => Data.IndexOf(data);
        public static GfuOperationData CreateInstance(params Type[] types){
            Data[] datas = new Data[types.Length];
            for (var i = 0; i < types.Length; i++){
                datas[i]=new Data(types[i]);
            }
            GfuOperationData gfuOperationData = new GfuOperationData(datas);
            return gfuOperationData;
        }
        public static GfuOperationData CreateInstance(List<Type> types){
            Data[] datas = new Data[types.Count];
            for (var i = 0; i < types.Count; i++){
                datas[i]=new Data(types[i]);
            }
            GfuOperationData gfuOperationData = new GfuOperationData(datas);
            return gfuOperationData;
        }
        /// <summary>
        /// 创建一个Gfu操作数据的实例
        /// </summary>
        /// <param name="data">数据容器</param>
        /// <typeparam name="T">要创建的操作数据类型</typeparam>
        /// <returns>返回创建的实例</returns>
        /// <exception cref="NotSupportedException">传入的类型不是指定类型或者没有参数为0的默认构造时触发</exception>
        [Obsolete]
        public static T CreateInstance<T>(List<Data> data)where T:GfuOperationData{
            T gfuOperationData = default(T);
            Type type = typeof(T);
            ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance
                | BindingFlags.NonPublic
                | BindingFlags.Public);
            ConstructorInfo noParameterConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructorInfoArray)
            {
                ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                if (0 == parameterInfoArray.Length)
                {
                    noParameterConstructorInfo = constructorInfo;
                    break;
                }
            }
            if (null == noParameterConstructorInfo)
            {
                throw new NotSupportedException("No constructor without 0 parameter");
            }
            gfuOperationData = (T)noParameterConstructorInfo.Invoke(null);
            gfuOperationData.Data = data;
            return gfuOperationData;
        }
        [Obsolete]
        public static T CreateInstance<T>(params Data[] data)where T:GfuOperationData{
            T gfuOperationData = default(T);
            Type type = typeof(T);
            ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance
                                                                          | BindingFlags.NonPublic
                                                                          | BindingFlags.Public);
            ConstructorInfo noParameterConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructorInfoArray)
            {
                ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                if (0 == parameterInfoArray.Length)
                {
                    noParameterConstructorInfo = constructorInfo;
                    break;
                }
            }
            if (null == noParameterConstructorInfo)
            {
                throw new NotSupportedException("No constructor without 0 parameter");
            }
            gfuOperationData = (T)noParameterConstructorInfo.Invoke(null);
            gfuOperationData.Data = new List<Data>();
            foreach (var data1 in data){
                gfuOperationData.Data.Add(data1);
            }
            return gfuOperationData;
        }
        [Obsolete]
        public static T CreateInstance<T>(params object[] data)where T:GfuOperationData{
            T gfuOperationData = default(T);
            Type type = typeof(T);
            ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance
                                                                          | BindingFlags.NonPublic
                                                                          | BindingFlags.Public);
            ConstructorInfo noParameterConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructorInfoArray)
            {
                ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                if (0 == parameterInfoArray.Length)
                {
                    noParameterConstructorInfo = constructorInfo;
                    break;
                }
            }
            if (null == noParameterConstructorInfo)
            {
                throw new NotSupportedException("No constructor without 0 parameter");
            }
            gfuOperationData = (T)noParameterConstructorInfo.Invoke(null);
            gfuOperationData.Data = new List<Data>();
            foreach (var o in data){
                gfuOperationData.Data.Add(new Data(o.GetType(),o));
            }
            return gfuOperationData;
        }
    }

    /// <summary>
    /// 数据容器,亦或者称流动的数据本身
    /// </summary>
    public class Data{
        public Type Type{
            get{
                if (value != null) return value.GetType();
                return _type;
            }
            set => _type = value;
        }
        public Type _type;
        public object value;
        public GfuOperationData gfuOperationData;
        public int outportIndex=-1;
        public int Index => gfuOperationData.Data.IndexOf(this);
        public bool IsOver = false;
        public Data(Type type){
            this.Type = type;
            this.value = null;
        }
        public Data(object value){
            this.Type = value?.GetType();
            this.value = value;
        }
        // public Data(Data data){
        //     this.type = data.type;
        //     this.value = data.type;
        // }
        public Data(Type type, object value){
            this.Type = type;
            this.value = value;
        }
        
        public static implicit operator Data(Object obj){
            return new Data(obj);
        }
        public static implicit operator Object(Data obj){
            return (Object)obj.value;
        }
        public static object ToObject(Data obj){
            return obj.value;
        }
        public static Data FromObject(object obj){
            return new Data(obj);
        }
        
        /// TODO 将会废除下述字段，而采用新方案
        [Obsolete]
        public List<Type> Types = new List<Type>();
        [Obsolete]
        public static T CreateInstance<T>(params object[] param)where T:Data{
            T data = default(T);
            Type type = typeof(T);
            ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance
                                                                          | BindingFlags.NonPublic
                                                                          | BindingFlags.Public);
            ConstructorInfo ParameterConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructorInfoArray)
            {
                ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                if (parameterInfoArray.Length == param.Length)
                {
                    ParameterConstructorInfo = constructorInfo;
                    break;
                }
            }
            if (null == ParameterConstructorInfo)
            {
                throw new NotSupportedException($"No constructor without {param.Length} parameter");
            }
            data = (T)ParameterConstructorInfo.Invoke(param);
            //反射数据中存在的字段，并且记录到字段列表当中
            foreach (var fieldInfo in data.GetType().GetFields()){
                if (fieldInfo.FieldType != typeof(List<Type>)){
                    data.Types.Add(fieldInfo.FieldType);
                }
            }
            return data;
        }
    }
}