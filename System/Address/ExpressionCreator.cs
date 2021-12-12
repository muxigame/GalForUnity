//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExpressionCreator.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 13:07:32
//
//======================================================================

using System;
using GalForUnity.InstanceID;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace GalForUnity.System.Address{
    public class ExpressionCreator{
        private ExpressionProtocol _protocol=ExpressionProtocol.None;
        private string _address="";
        private string _findFlag;
        private string _assemblyName;
        private string _className;
        private string _fieldName;
        public ExpressionCreator(){}
        public ExpressionCreator(ExpressionProtocol protocol,Component component){
            InitProtocol(protocol,component.gameObject);
            AddAssemblyName(Assembly.GetAssembly(component.GetType()).FullName);
            AddClassName(component.GetType().FullName);
        } 
        public ExpressionCreator(ExpressionProtocol protocol,Component component,string name){
            InitProtocol(protocol,component.gameObject);
            AddAssemblyName(Assembly.GetAssembly(component.GetType()).FullName);
            AddClassName(component.GetType().FullName);
            AddFieldName(name);
        }
        public ExpressionCreator(ExpressionProtocol protocol,Component component,object field){
            InitProtocol(protocol,component.gameObject);
            var type = component.GetType();
            AddAssemblyName(Assembly.GetAssembly(type).FullName);
            AddClassName(type.FullName);
            var fieldInfos = type.GetFields();
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.GetValue(component) == field){
                    AddFieldName(fieldInfo.Name);
                    break;
                }
            }
        }
        public ExpressionCreator(ExpressionProtocol protocol,GameObject gameObject,object field){
            InitProtocol(protocol, gameObject);
            var fieldInfos = gameObject.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.GetValue(gameObject) == field){
                    AddFieldName(fieldInfo.Name);
                    break;
                }
            }
        }
        public ExpressionCreator(ExpressionProtocol protocol,GameObject gameObject,string name){
            InitProtocol(protocol, gameObject);
            AddFieldName(name);
        } 
        public ExpressionCreator(ExpressionProtocol protocol,GameObject gameObject){
            InitProtocol(protocol, gameObject);
            AddFieldName(nameof(gameObject));
        }

        private void InitProtocol(ExpressionProtocol protocol,GameObject gameObject){
            AddProtocol(protocol);
            if (protocol is ExpressionProtocol.Memory){
                AddObjectName(GameObjectPath(gameObject));
            }else if (protocol is ExpressionProtocol.InstanceID){
                if(gameObject.TryGetComponent(out GfuInstance gfuInstance)) 
                    AddInstanceID(gfuInstance.instanceID);
                else throw new ArgumentException("这不是一个Gfu实例"+gameObject);
            }
        }
        
        private string GameObjectPath(GameObject gameObject){
            string path = "";
            var gameObjectTransform = gameObject.transform;
            while (gameObjectTransform.parent){
                var parent = gameObjectTransform.parent;
                path = parent.name + "/" + path;
                gameObjectTransform = parent;
            }
            return path +gameObject.name;
        }
        /// Memory 语法示例："[Memory:(AssemblyName)[GameObjectName]GalForUnity.System.GameSystem:Data.PlotFlowController]"
        /// InstanceID 语法示例："[InstanceID:15619684219856165878]"
        /// UnityInstanceID 语法示例："[UnityInstanceID:15619684219856165878]"(仅限UnityEditor)
        /// Resource 语法示例："[Resource:Dir.FileName]"
        /// Path 语法示例："[Path:Dir.FileName.suffix]"
        public ExpressionCreator AddProtocol(string protocol){
            _protocol =(ExpressionProtocol) Enum.Parse(typeof(ExpressionProtocol),protocol);
            return this;
        }
        public ExpressionCreator AddProtocol(ExpressionProtocol protocol){
            _protocol = protocol;
            return this;
        }
        public ExpressionCreator AddAssemblyName(string assemblyName){
            _assemblyName = Brackets(assemblyName);
            return this;
        }
         
        public ExpressionCreator AddClassName(string className){
            _className = className;
            return this;
        }
          
        public ExpressionCreator AddObjectName(string objectName){
            _findFlag = SquareBrackets(objectName);
            return this;
        }
        
        public ExpressionCreator AddInstanceID(string instanceID){
            _findFlag = SquareBrackets(instanceID);
            return this;
        }
        public ExpressionCreator AddInstanceID(long instanceID){
            _findFlag = SquareBrackets(instanceID+"");
            return this;
        } 
        public ExpressionCreator AddPath(string path){
            _findFlag = SquareBrackets(path);
            return this;
        } 
        public ExpressionCreator AddFieldName(string fieldName){
            _fieldName = fieldName;
            return this;
        }
        
        public ExpressionCreator AddExpression(string expression){
            _address = expression;
            return this;
        }
        
        private string SquareBrackets(string express){
            return "[" + express + "]";
        }

        private string Brackets(string express){
            return "(" + express + ")";
        }
        private string Scope(string scope){
            return "(" + scope + ")";
        }

        private string Combine(ExpressionProtocol expressionProtocol, string express){
            return expressionProtocol + ":" + express;
        }
        private string Combine(string objectAddress, string fieldAddress){
            return objectAddress + ":" + fieldAddress;
        }
        
        public string Create(){
            if (_protocol == ExpressionProtocol.None) throw new ArgumentException("未指定协议");
            if (!string.IsNullOrEmpty(_address)) return Combine(_protocol, _address);
            if (!string.IsNullOrEmpty(_assemblyName)) _address += _assemblyName;
            if(string.IsNullOrEmpty(_findFlag) &&string.IsNullOrEmpty(_className)) throw new ArgumentException("未指定对象");
            if (!string.IsNullOrEmpty(_findFlag)) _address += _findFlag;
            if (!string.IsNullOrEmpty(_className)) _address += _className;
            if (!string.IsNullOrEmpty(_fieldName)){
                _address =Combine(_address,_fieldName) ;
            }
            return SquareBrackets(Combine(_protocol,_address));
        }
    }

    public enum ExpressionProtocol{
        Memory,
        InstanceID,
        UnityInstanceID,
        Resource,
        Json,
        Path,
        None
    }
}
