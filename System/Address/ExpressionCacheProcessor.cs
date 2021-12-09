//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExpressionCacheProcessor.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 12:44:35
//
//======================================================================

using System;
using System.Collections.Generic;

namespace GalForUnity.System.Address{
    public class ExpressionCacheProcessor:IDisposable{
        private static readonly Dictionary<string, object> ExpressionCache = new Dictionary<string, object>();
        public static bool cache = true;

        public object Cache(string expression, object value){
            if (cache) ExpressionCache[expression] = value;
            return value;
        }

        public bool HasCache(string expression,bool subExpression=false){
            if (subExpression){
                foreach (var keyValuePair in ExpressionCache){
                    if (keyValuePair.Key.Contains(expression)){
                        return true;
                    }
                }
            }
            return ExpressionCache.ContainsKey(expression);
        }

        public object GetCache(string expression,bool subExpression=false){
            if (!cache) return null;
            if (ExpressionCache.ContainsKey(expression)) return ExpressionCache[expression];
            if (subExpression){
                foreach (var keyValuePair in ExpressionCache){
                    if (keyValuePair.Key.Contains(expression)){
                        return keyValuePair.Value;
                    }
                }
            }
            return null;
        }


        public void Dispose(){
            ExpressionCache.Clear();
        }
    }
}