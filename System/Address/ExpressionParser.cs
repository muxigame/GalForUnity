//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExpressionParser.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 23:29:06
//
//======================================================================

using System;
using UnityEngine;

namespace GalForUnity.System.Address{
    /// <summary>
    /// 解析表达式的解析器。
    /// </summary>
    public class ExpressionParser{
        
        public ExpressionParser(string expression){
            if(!AddressableExpression.IsAddress.IsMatch(expression)) throw new ArgumentException("这不是一个地址表达式");
            _expression = AddressableExpression.getExpression.Match(expression).Value;
            var strings = _expression.Split(':');
            _protocol = (ExpressionProtocol) Enum.Parse(typeof(ExpressionProtocol), strings[0]);
            _address = strings[1];
            _assemblyName = CancelBrackets(AddressableExpression.AssemblyName.Match(_address).Value);
            if (_protocol == ExpressionProtocol.Memory){
                var ObjectName = AddressableExpression.ObjectName.Match(_address).Value;
                _objectName = CancelSquareBrackets(ObjectName);
                _className = AddressableExpression.ClassName.Match(_address.Substring(_address.IndexOf("]", StringComparison.Ordinal))).Value;
                if (strings.Length == 3){
                    _address = _address +":" +strings[2];
                    _fieldName = strings[2];
                }
            }
            if (_protocol == ExpressionProtocol.InstanceID){
                var InstanceID = AddressableExpression.InstanceID.Match(_address).Value;
                _instanceID = long.Parse(CancelSquareBrackets(InstanceID));
                _className = AddressableExpression.ClassName.Match(_address.Substring(_address.IndexOf("]", StringComparison.Ordinal))).Value;
                if (strings.Length == 3){
                    _address = _address +":" +strings[2];
                    _fieldName = strings[2];
                }
            }
        }
        
        private readonly string _expression;
        private readonly ExpressionProtocol _protocol;
        private readonly string _address;
        private readonly string _assemblyName;
        private readonly string _objectName;
        private readonly string _className;
        private readonly string _fieldName;
        private readonly long _instanceID;

        public string GetExpression(){
            return _expression;
        }
        public string GetAddress(){
            return _address;
        }
        public ExpressionProtocol GetProtocol(){
            return _protocol;
        }
        public string GetAssemblyName(){
            return _assemblyName;
        }
        public string GetObjectName(){
            return _objectName;
        }
        public string GetClassName(){
            return _className;
        }
        public string GetFieldName(){
            return _fieldName;
        }

        public long GetInstanceID(){
            return _instanceID;
        }
        
        private string CancelSquareBrackets(string express){
            return express.Replace("[","").Replace("]","");
        }

        private string CancelBrackets(string express){
            return express.Replace("(","").Replace(")","");
        }
    }
}
