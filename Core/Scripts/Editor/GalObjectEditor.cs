using UnityEditor;
using UnityEngine;

namespace GalForUnity.Core.Editor
{
    [CustomEditor(typeof(GalObject))]
    public class GalObjectEditor : UnityEditor.Editor
    {
        private GalObject m_GalObject;

        private void OnEnable()
        {
            m_GalObject = (GalObject)target;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (m_GalObject.pose == null || m_GalObject.pose.Count == 0) return;

            if (m_GalObject.pose[0] is SpritePose spritePose)
                GUI.DrawTexture(GetPreviewRenderRect(r, spritePose.sprite.texture), spritePose.sprite.texture);
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }


        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (m_GalObject.pose == null || m_GalObject.pose.Count == 0)
                return base.RenderStaticPreview(assetPath, subAssets, width, height);
        
            if (m_GalObject.pose[0] is SpritePose spritePose)
            {
                return spritePose.sprite.texture;
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        
        private Rect GetPreviewRenderRect(Rect rect, Texture2D texture2D)
        {
            if (!texture2D) return rect;
            var renderRect = new Rect(rect)
            {
                width = texture2D.width,
                height = texture2D.height
            };
            if (renderRect.width > rect.width)
            {
                var radio = rect.width / renderRect.width;
                renderRect.width *= radio;
                renderRect.height *= radio;
            }

            if (renderRect.height > rect.height)
            {
                var radio = rect.height / renderRect.height;
                renderRect.width *= radio;
                renderRect.height *= radio;
            }

            renderRect.center = (rect.min + rect.max) / 2f;
            return renderRect;
        }
    }
}