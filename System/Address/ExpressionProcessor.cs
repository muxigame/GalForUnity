//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExpressionProcessor.cs
//
//        Created by 半世癫(Roc) at 2021-12-08 15:30:28
//
//======================================================================

using System;
using UnityEngine;

namespace GalForUnity.System.Address{
    public class ExpressionProcessor {
        private ExpressionCreator ExpressionCreator;
        private ExpressionParser ExpressionParser;
        private object obj;

        public ExpressionProcessor(object obj){
            this.obj = obj;
        }
        public ExpressionProcessor(string expression){
            ExpressionParser = new ExpressionParser(expression);
            // ExpressionParser.
        }

        public string GetInstanceIDExpression(){
            if (obj is Component component){
                ExpressionCreator = new ExpressionCreator(ExpressionProtocol.InstanceID,component);
            } else if (obj is GameObject gameObject){
                ExpressionCreator = new ExpressionCreator(ExpressionProtocol.InstanceID,gameObject);
            } else{
                throw new NotImplementedException("暂不支持解析其他类型");
            }
            return ExpressionCreator.Create();
        }
        public string GetMemoryExpression(){
            if (obj is Component component){
                ExpressionCreator = new ExpressionCreator(ExpressionProtocol.Memory,component);
            }else if (obj is GameObject gameObject){
                ExpressionCreator = new ExpressionCreator(ExpressionProtocol.Memory,gameObject);
            } else{
                throw new NotImplementedException("暂不支持解析其他类型");
            }
            return ExpressionCreator.Create();
        }
        

    }
}
