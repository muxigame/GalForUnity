//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TimeCheckNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-09 23:26:44
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Model;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.GFUNode.Logic{
    [NodeRename("Logic/"+nameof(TimeCheckNode))]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph|NodeAttributeTargets.ItemGraph)]
    [Serializable]
    public sealed class TimeCheckNode : EnterNode{
        

        [NodeRename("Satisfy"+nameof(Exit),typeof(RoleData),NodeDirection.Output,NodeCapacity.Single)]
        public GfuPort Exit;
        [NodeRename(nameof(DissatisfyExit),typeof(RoleData),NodeDirection.Output,NodeCapacity.Single)]
        public GfuPort DissatisfyExit;

        
        public GameTime GameTime = new GameTime(2021,1,1);
        public GameTime GameTime2 = new GameTime(2021,1,1);
        
        public enum TimeCheckType{
            [Rename("之前")]
            Before,
            [Rename("之后")]
            After,
            [Rename("之间")]
            Between
        }

        public TimeCheckType timeCheckType;
        public override RoleData Execute(RoleData roleData){
            switch (timeCheckType){
                case TimeCheckType.Before:
                    if (GameTime >= GameSystem.Data.CurrentTime)
                        Executed();
                    else
                        Executed(1);
                    break;
                case TimeCheckType.After:
                    if (GameTime <= GameSystem.Data.CurrentTime)
                        Executed();
                    else
                        Executed(1);
                    break;
                case TimeCheckType.Between:
                    if (GameTime < GameTime2){
                        if (GameTime >= GameSystem.Data.CurrentTime && GameSystem.Data.CurrentTime <= GameTime2)
                            Executed();
                        else
                            Executed(1);
                        break;
                    } else if(GameTime > GameTime2){
                        if (GameTime2 >= GameSystem.Data.CurrentTime && GameSystem.Data.CurrentTime <= GameTime)
                            Executed();
                        else
                            Executed(1);
                        break;
                    } else{
                        if (GameTime == GameSystem.Data.CurrentTime && GameSystem.Data.CurrentTime == GameTime2)
                            Executed();
                        else
                            Executed(1);
                        break;
                    }
            }
            return roleData;
        }
        
#if UNITY_EDITOR
        private EnumField condition;
        private Vector3IntField _vector3IntField;
        private Vector3IntField _vector3IntField2;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            _vector3IntField = new Vector3IntField() {
                label = GfuLanguage.GfuLanguageInstance.TIME.Value,
                value = GameTime,
                style = {
                    minWidth = 0,
                },
                labelElement = {
                    style= {
                        minWidth = 0,
                        // fontSize = 12,
                        unityTextAlign = TextAnchor.MiddleLeft
                    },
                },
            };
            _vector3IntField2 = new Vector3IntField() {
                label = GfuLanguage.GfuLanguageInstance.TIME.Value, value = GameTime2,
                style = {
                    minWidth = 0,
                },
                labelElement = {
                    style= {
                        minWidth = 0,
                        // fontSize = 12,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                },
            };
            _vector3IntField.RegisterValueChangedCallback((x) => {
                var vector3Int = TimeCheck(x);
                if (vector3Int != x.newValue){
                    _vector3IntField.value = vector3Int;
                }
                GameTime = vector3Int;
            });
            _vector3IntField2.RegisterValueChangedCallback((x) => {
                var vector3Int = TimeCheck(x);
                if (vector3Int != x.newValue){
                    _vector3IntField2.value = vector3Int;
                }
                GameTime2 = vector3Int;
            });
            condition = new EnumField() {
                label = GfuLanguage.Parse(nameof(condition)),
                style = { marginTop = 5},
                labelElement = {
                    style = {
                        minWidth = 0,
                        // fontSize = 12,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            condition.Init(timeCheckType);
            condition.RegisterValueChangedCallback((x) => {
                timeCheckType = (TimeCheckType) x.newValue;
                EnumCheck();
            });
            mainContainer.Add(condition);
            mainContainer.Add(_vector3IntField);
            foreach (var visualElement in _vector3IntField.ElementAt(1).Children()){
                ((IntegerField) visualElement).label=(visualElement as IntegerField)?.label == "X"?GfuLanguage.GfuLanguageInstance.YEAR.Value:(visualElement as IntegerField)?.label =="Y"?GfuLanguage.GfuLanguageInstance.MONTH.Value:GfuLanguage.GfuLanguageInstance.DAY.Value;
                ((IntegerField) visualElement).labelElement.style.minWidth = 0;
            }
            foreach (var visualElement in _vector3IntField2.ElementAt(1).Children()){
                ((IntegerField) visualElement).label=(visualElement as IntegerField)?.label == "X"?GfuLanguage.GfuLanguageInstance.YEAR.Value:(visualElement as IntegerField)?.label =="Y"?GfuLanguage.GfuLanguageInstance.MONTH.Value:GfuLanguage.GfuLanguageInstance.DAY.Value;
                ((IntegerField) visualElement).labelElement.style.minWidth = 0;
            }
            // ObjectField.ElementAt(1).ElementAt(2).style.marginRight = 38;
            EnumCheck();
        }

        public Vector3Int TimeCheck(ChangeEvent<Vector3Int> x){
            Vector3Int vector3Int = x.newValue;
            if (vector3Int.y > 12){
                vector3Int.y = 12;
            }

            if (vector3Int.z > 31){
                vector3Int.z = 31;
            }

            vector3Int.x = vector3Int.x < 0 ? 0 : vector3Int.x;
            vector3Int.y = vector3Int.y < 0 ? 0 : vector3Int.y;
            vector3Int.z = vector3Int.z < 0 ? 0 : vector3Int.z;
            // x.previousValue.Set(vector3Int.x,vector3Int.y,vector3Int.z);
            return vector3Int;
        }

        public void EnumCheck(){
            if (timeCheckType == TimeCheckType.Between){
                if (!mainContainer.Contains(_vector3IntField2)){
                    mainContainer.Add(_vector3IntField2);
                }
            } else{
                if (mainContainer.Contains(_vector3IntField2)){
                    mainContainer.Remove(_vector3IntField2);
                }
            }
        }
#endif
        
    }
    
}
