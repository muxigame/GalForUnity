//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SavableSpriteRender.cs
//
//        Created by 半世癫(Roc) at 2021-12-12 19:32:43
//
//======================================================================


using System;
using GalForUnity.System.Address.Addresser;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.System.Archive.Data.Savables{
    [Serializable]
    public class SavableSpriteRender:Savable{
        public SavableSpriteRender(SpriteRenderer spriteRenderer){
            _spriteRenderer = spriteRenderer;
        }
        [NonSerialized]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private Color color;
        [SerializeField]
        private SpriteDrawMode drawMode;
        [SerializeField]
        private Vector2 size;
        [SerializeField]
        private int sortingOrder;
        [SerializeField]
        private bool flipX;
        [SerializeField]
        private bool flipY;
        // [SerializeField]
        // private Material material;

        public override void Save(){
            address = InstanceIDAddresser.GetInstance().Parse(_spriteRenderer);
            sprite = _spriteRenderer.sprite;
            color = _spriteRenderer.color;
            drawMode = _spriteRenderer.drawMode;
            size = _spriteRenderer.size;
            sortingOrder = _spriteRenderer.sortingOrder;
            // material = _spriteRenderer.material;WCTMD 序列化Unity内置的材质Unity会崩溃的
            flipX = _spriteRenderer.flipX;
            flipY = _spriteRenderer.flipY;
        }

        public override void Recover(){
            base.Recover();
            if(InstanceIDAddresser.GetInstance().Get(address,out object obj)){
                _spriteRenderer=(SpriteRenderer)obj;
                _spriteRenderer.sprite = sprite;
                _spriteRenderer.color = color;
                _spriteRenderer.size = size;
                _spriteRenderer.drawMode = drawMode;
                _spriteRenderer.sortingOrder = sortingOrder;
                // _spriteRenderer.material = material;
                _spriteRenderer.flipX = flipX;
                _spriteRenderer.flipY = flipY;
            }
        }
    }
}
