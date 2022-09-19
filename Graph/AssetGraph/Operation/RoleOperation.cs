//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  RoleOperation.cs
//
//        Created by 半世癫(Roc) at 2021-01-28 17:33:27
//
//======================================================================

using GalForUnity.Graph.AssetGraph.GFUNode.Operation;
using GalForUnity.Model;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class RoleOperation : GfuOperation
    {
        /// <summary>
        /// 截断参数传递，角色操作直接通过构造方法传入要求修改的角色，因为此节点不传递参数给下一个节点
        /// </summary>
        /// <param name="roleModel">角色模型</param>
        public RoleOperation(RoleModel roleModel){
            Container = new GfuOperationData(
                new Data(typeof(RoleModel),roleModel),
                new Data(typeof(RoleNode.RoleOperationType),null)
                );
            Input = new GfuOperationData(
                new Data(typeof(Transform),roleModel != null ? roleModel.transform : null),
                new Data(typeof(AnimationClip),null),
                new Data(typeof(float),0),
                new Data(typeof(Color),0)
            );
        }
        /// <summary>
        /// 截断参数传递，角色操作直接通过构造方法传入要求修改的角色，因为此节点不传递参数给下一个节点
        /// </summary>
        public RoleOperation(GfuOperationNode gfuNode){
            Input = new GfuOperationData(gfuNode.GetDefaultValue());
        }
        public RoleOperation(GfuOperationData input){
            Input = input;
        }

        public override void Start(GfuOperationData gfuOperationData){
            var roleModel=(RoleModel) ContainerData[0];
            if (roleModel){
                float opacity = (float) InputData[2].value;
                Color color =  InputData[3].value as Color? ?? (Vector4)InputData[3].value;
                color.a = opacity;
                roleModel.Color=color;
                var gameObject = roleModel.gameObject;
                if (gameObject.activeSelf == false){
                    if ((RoleNode.RoleOperationType)ContainerData[1].value == RoleNode.RoleOperationType.ToStage){
                        gameObject.SetActive(true);
                    }
                }
                if (InputData[1].value is AnimationClip animationClip){
                    roleModel.PlayAnimationClip(animationClip);
                }else if (InputData[1].value is Animation animation){
#if LIVE2D
                    roleModel.PlayAnimationClip(animation.clip);
#else
                    animation.Play();
#endif
                }
            } else{
                Debug.Log(GfuLanguage.ParseLog("The role node has no corresponding operation role"));
            }
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            
            if (ContainerData != null && ContainerData.Count >= 2){
                var roleModel=(RoleModel) ContainerData[0];
                // Debug.LogError(roleModel.GetComponent<SpriteRenderer>());
                // Debug.LogError(roleModel.GetComponent<SpriteRenderer>().sprite);
                // Debug.LogError(roleModel.GetComponent<SpriteRenderer>().size);
                if (roleModel){
                    if (InputData[0] != null && ContainerData?[0] != null){
                        var transform = roleModel.transform;
                        var inputTransform = (Transform) InputData[0].value;
                        transform.position = inputTransform.position;
                        transform.rotation = inputTransform.rotation;
                        transform.localScale = inputTransform.localScale;
                    }
                    float opacity = (float) InputData[2].value;
                    Color color =  InputData[3].value as Color? ?? (Vector4)InputData[3].value;
                    // Debug.LogError(color);
                    // Debug.LogError(InputData[3].value);
                    // Debug.LogError(opacity);
                    color.a = opacity;
                    roleModel.Color=color;
                }
            }else{
                Debug.LogError("a unknown paramException,maybe \"ContainerData = null\" or \"ContainerData.Count < 2\"");
            }
            base.Update(gfuOperationData);
        }

        public override void OperationOver(){
            if (ContainerData != null && ContainerData.Count >= 2){
                var roleModel=(RoleModel) ContainerData[0];
                if (roleModel){
                    var gameObject = roleModel.gameObject;
                    if ((RoleNode.RoleOperationType)ContainerData[1].value == RoleNode.RoleOperationType.StepDown){
                        gameObject.SetActive(false);
                    }
                
                    if (InputData[0] != null && ContainerData?[0] != null){
                        var transform = roleModel.transform;
                        var inputTransform = (Transform) InputData[0].value;
                        transform.position = inputTransform.position;
                        transform.rotation = inputTransform.rotation;
                        transform.localScale = inputTransform.localScale;
                    }
                    float opacity = (float) InputData[2].value;
                    Color color =  InputData[3].value as Color? ?? (Vector4)InputData[3].value;
                    color.a = opacity;
                    roleModel.Color=color;
                }
            }else{
                Debug.LogError("a unknown paramException,maybe \"ContainerData = null\" or \"ContainerData.Count < 2\"");
            }
            base.OperationOver();
        }
    }
}
