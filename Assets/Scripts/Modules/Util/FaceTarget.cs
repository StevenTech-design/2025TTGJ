using UnityEngine;

namespace TTGJ.Util
{
	public class FaceTarget : MonoBehaviour
	{
		[SerializeField] private Transform target;
		[SerializeField] private bool onlyY = true;
		[SerializeField] private float turnSpeed = 360f; // 度/秒

		void Update()
		{
			if (!target) return;
			Vector3 dir = target.position - transform.position;
			if (onlyY) dir.y = 0f;
			if (dir.sqrMagnitude < 1e-6f) return;

			Quaternion to = Quaternion.LookRotation(dir.normalized, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, to, turnSpeed * Time.deltaTime);
		}

		public void SetTarget(Transform t) { target = t; }
	}
}