using GalForUnity.Core.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace GalForUnity.Framework.View{
    /// <summary>
    /// 自动调整背景的大小，使其自动等比拉伸填充屏幕
    /// </summary>
    public class BackgroundAutoSize : MonoBehaviour{
        
        private float _screenWidth;
        private float _screenHeight;
        private float _screenWidthWithHeight;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _screenWorldPosition;
        
        
        [Rename(nameof(renderingMode),"指定图片应当做何种处理，默认执行等比缩放")]
        [Tooltip("指定图片应当做何种处理，默认执行等比缩放")]
        public RenderingMode renderingMode = RenderingMode.GeometricScaling;

        // [Rename("自定义方法","请指定自定义方法")]
        [Tooltip("请指定自定义方法")]
        public UnityEvent<Sprite> customMethod;

        private void Start(){
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            _screenWidthWithHeight = _screenWidth / _screenHeight;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (Camera.main != null){
                _screenWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(_screenWidth, _screenHeight, 1));
            } else{
                _screenWorldPosition = Camera.current.ScreenToWorldPoint(new Vector3(_screenWidth, _screenHeight, 1));
            }
        }

        // Update is called once per frame
        private void Update(){
            if (_spriteRenderer.drawMode != SpriteDrawMode.Sliced&&renderingMode != RenderingMode.None){
                _spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            }

            switch (renderingMode){
                case RenderingMode.GeometricScaling:
                    GeometricScaling();
                    break;
                case RenderingMode.Tiled:
                    Tiled();
                    break;
                case RenderingMode.Custom:
                    customMethod?.Invoke(_spriteRenderer.sprite);
                    break;
                default:
                    None();
                    break;
            }
        }

        private void GeometricScaling(){
            var sprite = _spriteRenderer.sprite;
            if (sprite != null){
                float widthWithHeight = sprite.bounds.size.x / sprite.bounds.size.y;
                //如果图片的横纵比小于屏幕即，图片比屏幕高，则拉满宽度如1:8
                if (_screenWidthWithHeight > widthWithHeight){
                    _spriteRenderer.size=new Vector2(_screenWorldPosition.x * 2,  _screenWorldPosition.x * 2 / widthWithHeight);
                } else{ //如果图片的横纵比大于屏幕即，图片比屏幕长，则拉满高度如8:1
                    _spriteRenderer.size=new Vector2(_screenWorldPosition.y * 2 * widthWithHeight, _screenWorldPosition.y * 2);
                }
            }
            gameObject.transform.localScale=new Vector3(1,1,1);
        }

        private void Tiled(){
            _spriteRenderer.size=new Vector2(_screenWorldPosition.x * 2,  _screenWorldPosition.y * 2 );
        }

        private void None(){
            var bounds = _spriteRenderer.sprite.bounds;
            _spriteRenderer.size=new Vector2(bounds.size.x,bounds.size.y);
        }
    }
    public enum RenderingMode : byte{
        [Rename(nameof(Tiled))]
        [Tooltip("图像填充整个屏幕")]
        Tiled = 0,
        [Rename(nameof(GeometricScaling))]
        [Tooltip("按比例缩放图片直至最短边覆盖屏幕")]
        GeometricScaling = 1,
        [Rename(nameof(Custom))]
        [Tooltip("用自己的算法执行填充背景")]
        Custom =2,
        [Rename(nameof(None))]
        [Tooltip("保持原有图像")]
        None = 3
    }
}
