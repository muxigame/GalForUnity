using UnityEngine;

namespace MUX.Transform {
	public class Rotate : MonoBehaviour {
		public enum ForWard{
			x,
			y,
			z
		}
		public float speed = 3f;
		public ForWard forWard;
		void FixedUpdate() {
			if (forWard == ForWard.x) {
				transform.Rotate(Vector3.left, speed*Time.deltaTime);
			} else if (forWard == ForWard.z) {
				transform.Rotate(Vector3.forward, speed*Time.deltaTime);
			} else {
				transform.Rotate(Vector3.up, speed*Time.deltaTime);
			}
		}
	}

}
