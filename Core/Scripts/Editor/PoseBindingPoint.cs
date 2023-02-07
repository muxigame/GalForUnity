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
        public List<SpritePoseItem> PoseSpriteItems;
        public PoseBindingList PoseBindingList;
        public Vector2Field AnchorVector2Field;
        public TextField NameTextField;
        public PoseBindingAnchor PoseBindingAnchor;
        public PoseView PoseView;
        public BindingPoint BindingPoint;
        public PoseBindingPoint():this(null,null,null)
        {
            
        }

        public PoseBindingPoint(PoseBindingAnchor poseBindingAnchor, PoseView poseView, BindingPoint bindingPoint)
        {
            BindingPoint = bindingPoint;
            PoseView = poseView;
            PoseBindingAnchor = poseBindingAnchor;
            var type = typeof(BindingPoint);
            var toggle = this.Q<Toggle>();
            toggle.contentContainer[0].style.maxWidth = 20;
            toggle.contentContainer[0].style.alignItems = Align.Center;
            toggle.contentContainer[0][0].style.marginTop = 4;
            AnchorVector2Field = new Vector2Field()
            {
                label = "锚点",
                labelElement =
                {
                    style = { minWidth = 0, marginLeft = 0 }
                },
                style = { flexGrow = 2 }
            };
            NameTextField = new TextField()
            {
                label = "名称",
                labelElement =
                {
                    style = { minWidth = 0, marginLeft = 0 }
                },
                style = { flexGrow = 2 }
            };
            toggle.Add(NameTextField);
            toggle.Add(AnchorVector2Field);
            contentContainer.style.alignItems = Align.Center;
            contentContainer.Add(DragObjectField = new DragObjectField(typeof(Sprite)));
            contentContainer.Add(PoseBindingList =
                new PoseBindingList(PoseSpriteItems = BindingPoint?.spritePoseItems ?? new List<SpritePoseItem>(),
                    this));
            DragObjectField.OnAdded += (unityObjects) =>
            {
                foreach (var unityObject in unityObjects)
                {
                    if (PoseSpriteItems.Count == 0 || PoseSpriteItems.All(item => item.sprite != unityObject))
                        PoseSpriteItems.Add(new SpritePoseItem() { sprite = (Sprite)unityObject });
                }

                PoseBindingList.RefreshItems();
            };
            NameTextField.CreateBinder(type.GetField("name"), BindingPoint);
            if (poseBindingAnchor != null)
            {
                
                AnchorVector2Field.CreateBinder(type.GetField("point"), BindingPoint, filter: vector2 =>
                {
                    vector2.x = Mathf.Clamp01(vector2.x);
                    vector2.y = Mathf.Clamp01(vector2.y);
                    return vector2;
                }, () =>
                {
                    AnchorVector2Field.value = poseBindingAnchor.value;
                }, (x) =>
                {
                    poseBindingAnchor.SetValueWithoutNotify(bindingPoint.point);
                });
                
            }
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
        {
            var doMeasure = base.DoMeasure(desiredWidth, widthMode, desiredHeight, heightMode);
            AnchorVector2Field.value = BindingPoint.point;
            return doMeasure;
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