//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  InitNodeTool.cs
//
//        Created by 半世癫(Roc) at 2021-01-15 19:07:15
//
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.InstanceID;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Graph.Tool{
    public class InitNodeTool{
        /// <summary>
        /// 为节点容器赋值
        /// </summary>
        /// <param name="nodeData">节点数值</param>
        /// <param name="gfuNode">节点</param>
        public static void SetContainerValue(DataInfo nodeData, GfuNode gfuNode){ SetValue(nodeData, gfuNode); }

        /// <summary>
        /// 为节点默认值端口赋值
        /// </summary>
        /// <param name="nodeData">节点数据</param>
        /// <param name="gfuInputView">节点默认值端口</param>
        public static void SetDefaultPortValue(DataInfo nodeData, GfuInputView gfuInputView){
            // SetValue(nodeData, gfuInputView.fieldContainer[0]);
            //#if UNITY_EDITOR
            if (nodeData != null){
                // Debug.LogError(nodeData.idField);
                if (nodeData.idField != null){
                    foreach (var keyValuePair in nodeData.idField){
                        PropertyInfo propertyInfo = gfuInputView.GetType().GetProperty("Value");
                        if (keyValuePair.scriptableObject){
                            propertyInfo.SetValue(gfuInputView, keyValuePair.scriptableObject);
                            return;
                        }

                        Type fieldType = Type.GetType(keyValuePair.type);
                        if (string.IsNullOrEmpty(keyValuePair.assembly) || fieldType == null){
                            if (Assembly.GetExecutingAssembly().FullName != keyValuePair.assembly) fieldType = Assembly.Load(keyValuePair.assembly).GetType(keyValuePair.type);
                        }

                        // Debug.Log(keyValuePair.type);
                        var gfuInstance = GfuInstance.FindAllWithGfuInstanceID(keyValuePair.instanceID, fieldType);
                        if (gfuInstance == null){
                            Debug.LogError(GfuLanguage.ParseLog("Do not save, or because the scene switch caused the object lost, cannot get the value of the object from the ID, you can try to open the original scene and try again, do not save!"));
                        }

                        propertyInfo.SetValue(gfuInputView, gfuInstance);
                        return;
                    }
                }

                //Debug.LogError(nodeData.jsonField);
                if (nodeData.jsonField != null){
                    foreach (var keyValuePair in nodeData.jsonField){
                        PropertyInfo propertyInfo = gfuInputView.GetType().GetProperty("Value");
                        if (keyValuePair.scriptableObject){
                            propertyInfo.SetValue(gfuInputView, keyValuePair.scriptableObject);
                            return;
                        }

                        Type fieldType = null;
                        if (!string.IsNullOrEmpty(keyValuePair.type)){
                            fieldType = Type.GetType(keyValuePair.type);
                            if (!string.IsNullOrEmpty(keyValuePair.assembly) || fieldType == null){
                                if (Assembly.GetExecutingAssembly().FullName != keyValuePair.assembly) fieldType = Assembly.Load(keyValuePair.assembly).GetType(keyValuePair.type);
                            }
                        }

                        //Debug.LogError(fieldType);
                        if (fieldType == null) continue;
                        var fromJson = !fieldType.IsPrimitive && fieldType != typeof(string) ? JsonUtility.FromJson(keyValuePair.data, fieldType) : Convert.ChangeType(keyValuePair.data, fieldType);
                        try{
                            propertyInfo.SetValue(gfuInputView, fromJson);
                        } catch (Exception e){
                            Debug.LogError("Default Value Set Field" + e);
                        }

                        return;
                    }
                }
            }

            //#endif
        }

        /// <summary>
        /// 通过反射来为对象赋值，
        /// 当对象为预制体：
        /// 编辑器中：遍历Assets文件夹，并根据InstanceID给字段赋值
        /// 游戏中：遍历Resources文件夹，并根据InstanceID给字段赋值
        /// 当对象为层级面板的对象：
        /// 编辑器中：通过保存一个名为Instance的对象于目标对象内来进行对象查找，并将找到的对象赋值给字段
        /// 游戏中：通过保存一个名为Instance的对象于目标对象内来进行对象查找，并将找到的对象赋值给字段
        /// 当对象为可序列化的非Mono对象：
        /// 编辑器中：通过保存对象的序列化Json来反序列化赋值给字段
        /// 游戏中：通过保存对象的序列化Json来反序列化赋值给字段
        /// </summary>
        /// <param name="nodeData">节点数据</param>
        /// <param name="obj">进行赋值的对象</param>
        private static void SetValue(DataInfo nodeData, object obj){
            if (nodeData != null){
                // 尝试对集合进行初始化，该API未经过测试，失败率较大，目前支持的条件是
                // 继承自IEnumerable类
                // 拥有Add 获得 EnQueue方法
                // 仅限一维
                if (nodeData.listField != null && nodeData.listField.Count > 0){
                    foreach (var listData in nodeData.listField){
                        if (listData.jsonField != null && listData.jsonField.Count > 0){
                            SetListValue(listData, obj, listData.jsonField);
                        }

                        if (listData.idField != null && listData.idField.Count > 0){
                            SetListValue(listData, obj, listData.idField);
                        }
                    }
                }

                // 尝试为含有UnityID的字段赋值
                if (nodeData.idField != null){
                    foreach (var keyValuePair in nodeData.idField){
                        SetIDValue(keyValuePair, obj);
                    }
                }

                //尝试为可序列化字段赋值
                if (nodeData.jsonField != null){
                    foreach (var keyValuePair in nodeData.jsonField){
                        object field = obj.GetType().GetField(keyValuePair.name) ?? (object) obj.GetType().GetProperty(keyValuePair.name);
                        if (keyValuePair.scriptableObject){
                            if (field is FieldInfo fieldInfo){
                                fieldInfo.SetValue(obj, keyValuePair.scriptableObject);
                            } else if (field is PropertyInfo propertyInfo) propertyInfo.SetValue(obj, keyValuePair.scriptableObject);

                            continue;
                        }

                        Type fieldType = null;
                        if (!string.IsNullOrEmpty(keyValuePair.type)){
                            fieldType = Type.GetType(keyValuePair.type);
                            if (!string.IsNullOrEmpty(keyValuePair.assembly) || fieldType == null){
                                if (Assembly.GetExecutingAssembly().FullName != keyValuePair.assembly) fieldType = Assembly.Load(keyValuePair.assembly).GetType(keyValuePair.type);
                            }
                        }

                        if (fieldType == null) continue;
                        if (field is PropertyInfo propertyInfo2){
                            var fromJson = !propertyInfo2.PropertyType.IsPrimitive && propertyInfo2.PropertyType != typeof(string) ? JsonUtility.FromJson(keyValuePair.data, fieldType) : Convert.ChangeType(keyValuePair.data, fieldType);
                            propertyInfo2.SetValue(obj, fromJson);
                        } else if (field is FieldInfo fieldInfo){
                            var fromJson = !fieldInfo.FieldType.IsPrimitive && fieldInfo.FieldType != typeof(string) ? JsonUtility.FromJson(keyValuePair.data, fieldType) : Convert.ChangeType(keyValuePair.data, fieldType);
                            fieldInfo.SetValue(obj, fromJson);
                        }
                    }
                }
            }
        }

        private static object GetValue(NodeData.NodeFieldInfo nodeFieldInfo){
            if (nodeFieldInfo.scriptableObject) return nodeFieldInfo.scriptableObject;
            Type fieldType = Type.GetType(nodeFieldInfo.type);
            if (!string.IsNullOrEmpty(nodeFieldInfo.data)){
                // object field = obj.GetType().GetField(nodeFieldInfo.name) ?? (object) obj.GetType().GetProperty(nodeFieldInfo.name);
                if (fieldType == null) return null;
                return !fieldType.IsPrimitive && fieldType != typeof(string) ? JsonUtility.FromJson(nodeFieldInfo.data, fieldType) : Convert.ChangeType(nodeFieldInfo.data, fieldType);
            }

            if (!string.IsNullOrEmpty(nodeFieldInfo.assembly) || fieldType == null){
                if (Assembly.GetExecutingAssembly().FullName != nodeFieldInfo.assembly) fieldType = Assembly.Load(nodeFieldInfo.assembly).GetType(nodeFieldInfo.type);
            }

            var gfuInstance = GfuInstance.FindAllWithGfuInstanceID(nodeFieldInfo.instanceID, fieldType);
            if (gfuInstance == null){
                Debug.LogError(GfuLanguage.ParseLog("Do not save, or because the scene switch caused the object lost, cannot get the value of the object from the ID, you can try to open the original scene and try again, do not save!"));
            }

            return gfuInstance;
        }

        private static void SetIDValue(NodeData.NodeFieldInfo keyValuePair, object obj){
            object field = obj.GetType().GetField(keyValuePair.name) ?? (object) obj.GetType().GetProperty(keyValuePair.name);
            if (keyValuePair.scriptableObject){
                if (field is FieldInfo fieldInfo){
                    fieldInfo.SetValue(obj, keyValuePair.scriptableObject);
                } else if (field is PropertyInfo propertyInfo) propertyInfo.SetValue(obj, keyValuePair.scriptableObject);

                return;
            }

            Type fieldType = Type.GetType(keyValuePair.type);
            if (string.IsNullOrEmpty(keyValuePair.assembly) || fieldType == null){
                if (Assembly.GetExecutingAssembly().FullName != keyValuePair.assembly) fieldType = Assembly.Load(keyValuePair.assembly).GetType(keyValuePair.type);
            }

            var gfuInstance = GfuInstance.FindAllWithGfuInstanceID(keyValuePair.instanceID, fieldType);
            if (gfuInstance == null){
                Debug.LogError(GfuLanguage.ParseLog("Do not save, or because the scene switch caused the object lost, cannot get the value of the object from the ID, you can try to open the original scene and try again, do not save!"));
            }

            if (field is FieldInfo fieldInfo2)
                fieldInfo2.SetValue(obj, gfuInstance);
            else if (field is PropertyInfo propertyInfo) propertyInfo.SetValue(obj, gfuInstance);
        }

        private static void SetListValue(NodeData.ListData listData, object obj, List<NodeData.NodeFieldInfo> nodeFieldInfos){
            var listObj = Activator.CreateInstance(Type.GetType(listData.type) ?? typeof(List<object>));
            var cacheList = new ArrayList();
            var listType = listObj.GetType();
            foreach (var nodeFieldInfo in nodeFieldInfos){
                var value = GetValue(nodeFieldInfo);
                if (value == null){
                    Debug.LogError(GfuLanguage.ParseLog(GfuLanguage.ParseLog("Failed to get a value") + nodeFieldInfo));
                    continue;
                }

                cacheList.Add(value);
                if (listType.GetMethod("Add") != null){
                    listType.GetMethod("Add")?.Invoke(listObj, new[] {
                        value
                    });
                } else if (listType.GetMethod("Enqueue") != null){
                    listType.GetMethod("Enqueue")?.Invoke(listObj, new[] {
                        value
                    });
                }
            }

            if (listObj is Array){
                listObj = cacheList.ToArray();
            }

            var field = obj.GetType().GetField(listData.name) ?? (object) obj.GetType().GetProperty(listData.name);
            if (field == null){
                Debug.LogError(GfuLanguage.ParseLog(GfuLanguage.ParseLog("Failed to get a value") + listData.name));
                return;
            }

            field.GetType().GetMethod("SetValue", new[] {
                typeof(object), typeof(object)
            })?.Invoke(field, new[] {
                obj, listObj
            });
        }
    }
}