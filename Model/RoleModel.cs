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
using GalForUnity.Graph;
using GalForUnity.InstanceID;
using GalForUnity.System;
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
	public class RoleModel : MonoBehaviour {
		//姓名 
		[Rename(nameof(name))]
		[SerializeField]
		private new string name = "";

		public static readonly Color HighLightColor = UnityEngine.Color.white;
		public static readonly Color UnHighLightColor = new Color(0.35f, 0.35f, 0.35f,1f) ;
		public string Name {
			set => name = value;
			get => name;
		}

		private void OnEnable(){
			roleData = GetComponent<RoleData>();
#if LIVE2D
			if(!gameObject.GetComponent<CubismMotionController>()
#if UNITY_EDITOR
			&&
			!EditorApplication.isPlaying
#endif
			&&gameObject.GetComponent<CubismModel>()
			) gameObject.AddComponent<CubismMotionController>();
#endif
		}

		/// <summary>
		///角色的数值
		/// </summary>
		[Rename(nameof(roleData))]
		public RoleData roleData;

		
		public RoleData Parse(List<RoleData.RoleDataItem> roleDataItems){
			return roleData.Parse(roleDataItems);
		}

		public RoleModel Add(List<RoleData.RoleDataItem> roleDataItems){
			roleData=Parse(roleDataItems);
			return this;
		}
		public RoleModel Add(RoleData roleDataItems){
			roleData=Parse(roleData+roleDataItems);
			return this;
		}
		
		public static List<RoleData.RoleDataItem> operator +(RoleModel roleModel1, RoleModel roleModel){
			// var roleModel1RoleData = roleModel.roleData + roleModel1.roleData;
			return roleModel.roleData + roleModel1.roleData;
		}

		public void HighLight(bool isHighLight){
			if(gameObject.activeSelf)
				Color(isHighLight?HighLightColor:UnHighLightColor);
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
		
		public void Color(Color otherColor){
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
			if (gameObject.TryGetComponent(out MeshRenderer meshRenderer)) material = meshRenderer.material;
			if(!material&&gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) material = spriteRenderer.material;
			if (!(material is null)){
				material.color = otherColor;
			}
		}
	}
}
