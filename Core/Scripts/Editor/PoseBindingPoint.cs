using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor{
    public class PoseBindingPoint : Foldout
    {
        public class PoseBindingPointUxmlFactory : UxmlFactory<PoseBindingPoint, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){
                var visualElement = base.Create(bag, cc);
                visualElement.Q<Toggle>().Add(new Vector2Field(){
                    label = "锚点",
                    labelElement = {
                        style = { minWidth = 0,marginLeft = 0}
                    },
                    style = { flexGrow = 2}
                });
                visualElement.contentContainer.style.alignItems = Align.Center;
                visualElement.contentContainer.Add(new DragObjectField());
                visualElement.contentContainer.Add(new ListView());
                return visualElement;
            }
        }
    }
}
