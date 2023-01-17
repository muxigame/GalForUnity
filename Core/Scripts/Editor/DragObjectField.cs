using GalForUnity.Graph.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor{
    public sealed class DragObjectField:VisualElement{
        public DragObjectField(){
            styleSheets.Add(UxmlHandler.instance.dragObjectFieldUss);
            contentContainer.Add(new Label("拖拽精灵图片至此"){
                style = {
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            });
        }
        public class DragObjectFieldUxmlFactory : UxmlFactory<DragObjectField, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){
                return base.Create(bag, cc);
            }
        }
    }
}