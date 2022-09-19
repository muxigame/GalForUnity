//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  RoleModel.cs
//
//        Created by 半世癫(Roc)
//
//======================================================================
using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.InstanceID;
using GalForUnity.System.Archive.Attributes;
using GalForUnity.System.Archive.Behavior;
using GalForUnity.System.Archive.Data;
using GalForUnity.System.Archive.Data.Savables;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if LIVE2D
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Rendering;
#endif
using UnityEngine;

namespace GalForUnity.Model {

	[ExecuteAlways]
	[RequireComponent(typeof(RoleData))]
	[RequireComponent(typeof(GfuInstance))]
	[Serializable]
	public class RoleModel : TransformSavableBehaviour {
		[SerializeField]
		[HideInInspector]
		public SavableSpriteRender SavableSpriteRender;
		
		//姓名 
		[Rename(nameof(name))]
		[SaveFlag]
		[SerializeField]
		private new string name = "";

		public static readonly Color HighLightColor = UnityEngine.Color.white;
		public static readonly Color UnHighLightColor = new Color(0.35f, 0.35f, 0.35f,1f) ;
		[Rename(nameof(roleSpriteMap))]
		[SerializeField]
		private Sprite roleSpriteMap;
		
		public Color Color{
			get => color;
			set => SetColor(color = _serializableColor = value);
		}
		
		[SerializeField]
		[Rename(nameof(color))]
		private Color color=new Color();
		
		[SaveFlag]
		private SerializableVector _serializableColor;

		public Sprite RoleSpriteMap{
			get => roleSpriteMap;
			set => SpriteRenderer.sprite = roleSpriteMap  = value;
		}

		private SpriteRenderer _spriteRenderer;

		public SpriteRenderer SpriteRenderer => _spriteRenderer ? _spriteRenderer : gameObject.AddComponent<SpriteRenderer>();

		public string Name {
			set => name = value;
			get => name;
		}

		private void OnEnable(){
			RoleData = GetComponent<RoleData>();
#if LIVE2D
			if(!gameObject.GetComponent<CubismMotionController>()
#if UNITY_EDITOR
			&&
			!EditorApplication.isPlaying
#endif
			&&gameObject.GetComponent<CubismModel>()
			) gameObject.AddComponent<CubismMotionController>();
#endif
			if (!gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) _spriteRenderer = spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = roleSpriteMap;
			spriteRenderer.color = color;
			_spriteRenderer = spriteRenderer;
		}
		
		/// <summary>
		///角色的数值
		/// </summary>
		[Rename(nameof(RoleData))]
		[NonSerialized]
		public RoleData roleData;

		public RoleData RoleData{
			get{
				if (!roleData) roleData = GetComponent<RoleData>();
				return roleData;
			}
			set => roleData = value;
		}

		
		public RoleData Parse(List<RoleDataItem> roleDataItems){
			return RoleData.Parse(roleDataItems);
		}

		public RoleModel Add(List<RoleDataItem> roleDataItems){
			RoleData=Parse(roleDataItems);
			return this;
		}
		public RoleModel Add(RoleData roleDataItems){
			RoleData=Parse(RoleData+roleDataItems);
			return this;
		}
		
		public static List<RoleDataItem> operator +(RoleModel roleModel1, RoleModel roleModel){
			// var roleModel1RoleData = roleModel.roleData + roleModel1.roleData;
			return roleModel.RoleData + roleModel1.RoleData;
		}

		public void HighLight(bool isHighLight){
			if(gameObject.activeSelf)
				SetColor(isHighLight?HighLightColor:UnHighLightColor);
		}
		
		public void Opacity(float opacity){
#if LIVE2D
			var renderController = gameObject.GetComponent<CubismRenderController>();
			if (renderController){
				renderController.Opacity = opacity > 1 ? 1 : opacity;
				return;
			}
#endif
			var localRenderer = gameObject.GetComponent<MeshRenderer>();
			if (localRenderer){
				var material = localRenderer.material;
				Color color = default;
				if (material) color = material.color;
				if (material&& color != default){
					material.color = new Color(color.r, color.g, color.b, opacity > 1 ? 1 : opacity);
				}
			}
		}
		public void Transform(Transform otherTransform){
			var transform1 = transform;
			transform1.position = otherTransform.position;
			transform1.rotation = otherTransform.rotation;
			transform1.localScale = otherTransform.localScale;

		}
		
		public void Transform(Vector3 position,Vector3 rotation,Vector3 scale){
			var transform1 = transform;
			transform1.position = position;
			transform1.eulerAngles = rotation;
			transform1.localPosition = scale;
		}

		public void PlayAnimationClip(AnimationClip animationClip){
			if(!animationClip) return;
#if LIVE2D
			var cubism = gameObject.GetComponent<CubismMotionController>();//找到动画的所属的模型控制器
			if (cubism){
				if (cubism.IsPlayingAnimation()){ //当播放新动画时，如果没有新动画，继续之前的动画停止之前在播放的动画
					cubism.StopAllAnimation();
				}
				cubism.PlayAnimation(animationClip, 0, 3, animationClip.isLooping); //并行播放所有动画
				return;
			}
#endif
			var localAnimation = GetComponent<Animation>();
			if (!localAnimation) localAnimation = gameObject.AddComponent<Animation>();
			localAnimation.clip = animationClip;
			localAnimation.Play();
		}
		
		private void SetColor(Color otherColor){
			// Debug.Log(color);
#if LIVE2D
			var renderController = gameObject.GetComponent<CubismRenderController>();
			if (renderController){
				foreach (var renderControllerRenderer in renderController.Renderers){
					renderControllerRenderer.Color = otherColor;
				}
				return;
			}
#endif
			Material material=null;
			if (gameObject.TryGetComponent(out MeshRenderer meshRenderer)){
				material = meshRenderer.material;
			}

			if (!material && gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)){
				material = spriteRenderer.material;
				spriteRenderer.color = otherColor;
			}
			if (!(material is null)){
				material.color = otherColor;
			}
		}

		public override void GetObjectData(ScriptData scriptData){
			SavableSpriteRender = new SavableSpriteRender(_spriteRenderer);
			base.GetObjectData(scriptData);
		}
		
		public override void GetObjectData(){
			base.GetObjectData();
			_serializableColor = color;
		}

		public override void Recover(){
			base.Recover();
			Color = _serializableColor;
		}
	}
}
