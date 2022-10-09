using MUX.Support;
using UnityEngine;

namespace MUX.Transform {
	public class Scale : MonoBehaviour {
		[Header("功能")]
		[TextArea(3, 10)]
		public string function = "将此类挂在在GameObject上，自身会缩放";

		[Header("参数")]
		[FieldLabel("速度")]
		[Tooltip("缩放的速度")]
		public float speed = 1f;
		[FieldLabel("方向x")]
		[Tooltip("旋转轴向,勾选的轴向会被缩放")]
		public bool x = true;
		[FieldLabel("方向y")]
		[Tooltip("旋转轴向,勾选的轴向会被缩放")]
		public bool y = true;
		[FieldLabel("方向z")]
		[Tooltip("旋转轴向,勾选的轴向会被缩放")]
		public bool z = true;
		[FieldLabel("循环")]
		[Tooltip("旋转轴向,勾选的则无限循环不会停止")]
		public bool isloop = true;
		[FieldLabel("播放时长")]
		[Tooltip("如果没有勾选循环播放时长生效")]
		public float oldtime;
		[FieldLabel("缩小下限")]
		[Tooltip("最小允许缩小多小")]
		[Range(0, 1)]
		public float min;
		[Range(0, 5)]
		public float max;

		private float overtime;

		Vector3 old;
		private void Start() {
			old = transform.localScale;
			overtime = oldtime;
		}
		void Update() {
			float time = Time.fixedTime * speed;
			if (isloop) {
				overtime = time + oldtime;
				var f = max - min;
				var localScale = transform.localScale;
				localScale = new Vector3(
					x ? Mathf.Sin(time * old.x) / 1  * f+ min:localScale.x,
					y ? Mathf.Sin(time * old.y) / 1 * f + min:localScale.y,
					z ? Mathf.Sin(time * old.z) / 1 * f + min:localScale.z);
				transform.localScale = localScale;
			} else {
				if (overtime > time) {
					transform.localScale = new Vector3(
						x ? Mathf.Sin(time * old.x) * 0.5f + 0.5f < min ? min : Mathf.Sin(time * old.x) * 0.5f + 0.5f : 1,
						y ? Mathf.Sin(time * old.y) * 0.5f + 0.5f < min ? min : Mathf.Sin(time * old.x) * 0.5f + 0.5f : 1,
						z ? Mathf.Sin(time * old.z) * 0.5f + 0.5f < min ? min : Mathf.Sin(time * old.x) * 0.5f + 0.5f : 1);
				}
			}

		}
	}
}

