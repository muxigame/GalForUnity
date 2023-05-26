using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor
{
    public sealed class AnchorFoldout : Foldout
    {
        public DragObjectField DragObjectField;
        public List<AnchorSprite> PoseSpriteItems;
        public AnchorListView AnchorListView;
        public Vector2Field AnchorVector2Field;
        public TextField NameTextField;
        public AnchorElement AnchorElement;
        public PoseView PoseView;
        public Anchor Anchor;
        public AnchorFoldout():this(null,null,null)
        {
            
        }

        public AnchorFoldout(AnchorElement anchorElement, PoseView poseView, Anchor anchor)
        {
            Anchor = anchor;
            PoseView = poseView;
            AnchorElement = anchorElement;
            var type = typeof(Anchor);
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
            contentContainer.Add(AnchorListView =
                new AnchorListView(PoseSpriteItems = Anchor?.sprites ?? new List<AnchorSprite>(),
                    this));
            DragObjectField.OnAdded += (unityObjects) =>
            {
                foreach (var unityObject in unityObjects)
                {
                    if (PoseSpriteItems.Count == 0 || PoseSpriteItems.All(item => item.sprite != unityObject))
                        PoseSpriteItems.Add(new AnchorSprite() { sprite = (Sprite)unityObject });
                }

                AnchorListView.RefreshItems();
            };
            NameTextField.CreateBinder(type.GetField(nameof(Anchor.name)), Anchor);
            if (anchorElement != null)
            {
                AnchorVector2Field.CreateBinder(type.GetField(nameof(anchor.pivot)), Anchor, filter: vector2 =>
                {
                    vector2.x = Mathf.Clamp01(vector2.x);
                    vector2.y = Mathf.Clamp01(vector2.y);
                    return vector2;
                }, () =>
                {
                    AnchorVector2Field.value = anchorElement.value;
                }, (x) =>
                {
                    anchorElement.SetValueWithoutNotify(anchor.pivot);
                });
                
            }
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
        {
            var doMeasure = base.DoMeasure(desiredWidth, widthMode, desiredHeight, heightMode);
            AnchorVector2Field.value = Anchor.pivot;
            return doMeasure;
        }

        public class PoseBindingPointUxmlFactory : UxmlFactory<AnchorFoldout, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}