using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Nodes;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor
{
    public sealed class PoseBindingPoint : Foldout
    {
        public DragObjectField DragObjectField;
        public List<PoseSpriteItem> PoseSpriteItems;
        public PoseBindingList PoseBindingList;
        public Vector2Field Vector2Field;
        public PoseBindingAnchor PoseBindingAnchor;
        public PoseView PoseView;
        public PoseBindingPoint():this(null,null)
        {
            
        }
        public PoseBindingPoint(PoseBindingAnchor poseBindingAnchor,PoseView poseView)
        {
            PoseView = poseView;
            PoseBindingAnchor = poseBindingAnchor;
            var toggle = this.Q<Toggle>();
            toggle.contentContainer[0].style.alignItems = Align.Center;
            toggle.contentContainer[0][0].style.marginTop = 4;
            Vector2Field = new Vector2Field()
            {
                label = "锚点",
                labelElement =
                {
                    style = { minWidth = 0, marginLeft = 0 }
                },
                style = { flexGrow = 2 }
            };
            toggle.Add(Vector2Field);
            contentContainer.style.alignItems = Align.Center;
            contentContainer.Add(DragObjectField = new DragObjectField(typeof(Sprite)));
            contentContainer.Add(PoseBindingList=new PoseBindingList(PoseSpriteItems = new List<PoseSpriteItem>(),this));
            DragObjectField.OnAdded += (unityObjects) =>
            {
                foreach (var unityObject in unityObjects)
                {
                    if (PoseSpriteItems.Count==0||PoseSpriteItems.All(item => item.Sprite != unityObject))
                        PoseSpriteItems.Add(new PoseSpriteItem() { Sprite = (Sprite)unityObject });
                }
                PoseBindingList.RefreshItems();
            };
            if (poseBindingAnchor != null)
            {
                Vector2Field.CreateBinder(poseBindingAnchor.ValueFieldInfo, poseBindingAnchor,filter: vector2 =>
                {
                    vector2.x = Mathf.Clamp01(vector2.x);
                    vector2.y = Mathf.Clamp01(vector2.y);
                    return vector2;
                });
                poseBindingAnchor.Value= Vector2Field.value = new Vector2(0.5f, 0.5f);
            }
        }

        public class PoseBindingPointUxmlFactory : UxmlFactory<PoseBindingPoint, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}