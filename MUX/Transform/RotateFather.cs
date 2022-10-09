using MUX.Support;
using UnityEngine;

namespace MUX.Transform{
	public class RotateFather : MonoBehaviour{

		[Header("功能")] [TextArea(3, 10)] public string function = "将此类挂在在GameObject上，物体会自动绕父对象旋转";

		[Header("参数")] [FieldLabel("速度")] [Tooltip("绕父类旋转的速度")]
		public float speed = 3f;

		[FieldLabel("方向")] [Tooltip("绕父类旋转的方向")]
		public ForWard forWard;

		public enum ForWard{
			侧,
			上,
			前
		}

		// Update is called once per frame
		void Update(){
			if (forWard == ForWard.侧){
				transform.RotateAround(transform.parent.position, Vector3.left, speed * Time.deltaTime);
			}
			else if (forWard == ForWard.前){
				transform.RotateAround(transform.parent.position, Vector3.forward, speed * Time.deltaTime);
			}
			else{
				transform.RotateAround(transform.parent.position, Vector3.up, speed * Time.deltaTime);
			}

		}
	}

}
