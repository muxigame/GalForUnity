using MUX.Support;
using UnityEngine;

namespace MUX.Transform {
    public class Translate : MonoBehaviour {
		[Header("功能")]
		[TextArea(3, 10)]
		public string function = "将此类挂在在GameObject上，自身会朝指定轴移动";

		[Header("参数")]
		[FieldLabel("速度")]
		[Tooltip("缩放的速度")]
		public float speed = 1f;
		[FieldLabel("方向x")]
		[Tooltip("移动轴向,会向勾选的轴向移动")]
		public bool x = true;
		[FieldLabel("方向y")]
		[Tooltip("移动轴向,会向勾选的轴向移动")]
		public bool y = false;
		[FieldLabel("方向z")]
		[Tooltip("移动轴向,会向勾选的轴向移动")]
		public bool z = false;

        void Update() {
			transform.Translate(new Vector3(x ? speed : 0, y ? speed : 0, z ? speed : 0));
        }
    }

}

