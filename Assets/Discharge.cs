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
			hit.attachedRigidbody.linearVelocity = Vector3.zero;
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

	void OnCollisionEnter(Collision col) {
		foreach (Collider hit in Physics.OverlapSphere(
			transform.position,
			1F
		)) {
			if (hit.tag == "Player")
				GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
		}
	}
}
