using GalForUnity.Graph.Nodes.Editor.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor
{
    public sealed class PoseView:VisualElement
    {
        private static readonly float ParentBorderOffset = 2f;
        private float OffsetedWidth=>parent.worldBound.width-ParentBorderOffset;
        private float OffsetedHeight=>parent.worldBound.height-ParentBorderOffset;
        private readonly Label _label;
        public float ScaleRadio;

        public PoseView()
        {
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.defaultPose);
            contentContainer.Add(_label = new Label("点击更换姿势")
            {
                style =
                {
                    fontSize = 24,
                    color = new StyleColor(new Color(0.45f, 0.45f, 0.45f, 1))
                }
            });
        }

        public void ShowPose(Sprite sprite)
        {
            _label.visible = false;
            style.backgroundImage = new StyleBackground(sprite);
            ScaleRadio = sprite.texture.width / 300f;
            style.height = sprite.texture.height / ScaleRadio;
        }
        public void RemovePose()
        {
            _label.visible = true;
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.defaultPose);
            ScaleRadio = ResourceHandler.instance.defaultPose.texture.width / 300f;
            style.height = ResourceHandler.instance.defaultPose.texture.height / ScaleRadio;
        }
        public void ShowAnchor(Sprite sprite,PoseBindingAnchor poseBindingAnchor)
        {
            poseBindingAnchor.ShowPreview(sprite,new Vector2(sprite.texture.width / ScaleRadio,sprite.texture.height / ScaleRadio));
        }
        public void HideAnchor(PoseBindingAnchor poseBindingAnchor)
        {
            poseBindingAnchor.HidePreview();
        }
        public class PoseViewUxmlFactory : UxmlFactory<PoseView, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}