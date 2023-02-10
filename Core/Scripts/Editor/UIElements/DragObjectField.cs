using System;
using System.Linq;
using GalForUnity.Graph.Editor.Block;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Core.Editor.UIElements
{
    public sealed class DragObjectField : VisualElement
    {
        private readonly Type _type;
        public Action<Object[]> OnAdded;

        public DragObjectField() : this(typeof(Object))
        {
        }

        public DragObjectField(Type type)
        {
            _type = type;   
            styleSheets.Add(UxmlHandler.instance.dragObjectFieldUss);
            contentContainer.Add(new Label("拖拽精灵图片至此")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            });
            RegisterCallback<DragUpdatedEvent>(_ =>
            {
                if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length != 0)
                    if (DragAndDrop.objectReferences.All(unityObject =>
                        {
                            if (!AssetDatabase.Contains(unityObject)) return false;
                            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(unityObject))
                                .Any(subAsset => _type.IsInstanceOfType(subAsset));
                        }))
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            });
            RegisterCallback<DragPerformEvent>(_ =>
            {
                if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length != 0)
                    if (DragAndDrop.objectReferences.All(unityObject =>
                        {
                            if (!AssetDatabase.Contains(unityObject)) return false;
                            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(unityObject))
                                .Any(subAsset => _type.IsInstanceOfType(subAsset));
                        }))
                    {
                        OnAdded?.Invoke(DragAndDrop.objectReferences.Select(unityObject =>
                        {
                            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(unityObject))
                                .FirstOrDefault(subAsset => _type.IsInstanceOfType(subAsset));
                        }).ToArray());
                    }

            });
        }

        public class DragObjectFieldUxmlFactory : UxmlFactory<DragObjectField, UxmlTraits>
        {

        }
    }
}