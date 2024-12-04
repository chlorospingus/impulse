using UnityEngine;
using System.Collections.Generic;

public class Discharge : MonoBehaviour
{
	public float boomRadius;
	public float boomForce;
	public float fakeUp;

	public void Explode() {
		foreach (Collider hit in Physics.OverlapSphere(
			transform.position,
			boomRadius
		)) {
			if (hit.attachedRigidbody == null) continue;
			hit.attachedRigidbody.AddExplosionForce(
				boomForce,
				transform.position,
				boomRadius,
				fakeUp,
				ForceMode.Impulse
			);
		}
		Destroy(gameObject);
	}
}
