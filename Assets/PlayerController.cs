using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed;
	public float jumpStrength;
	public float airSpeed;
	public float mouseSens;
	public float throwStrength;
	public GameObject discharge;
	public bool grounded;
	InputAction move;
	InputAction jump;
	InputAction look;
	InputAction click;
	float camVertRot = 0f;
	Queue<GameObject> discharges;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
		look = InputSystem.actions.FindAction("Look");
		discharges = new Queue<GameObject>();
    }

    void Update()
    {
		Vector2 movement = move.ReadValue<Vector2>().normalized;
		Vector2 lookment = look.ReadValue<Vector2>();

		if (grounded) MoveGrounded(movement);
		else MoveAirborne(movement);

		MoveCamera(lookment);
		
		if (InputSystem.actions.FindAction("Attack").WasPressedThisFrame())
			Throw();

		if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
			discharges.Dequeue().GetComponent<Discharge>().Explode();
    }

	void MoveGrounded(Vector2 movement) {
		Rigidbody rb = GetComponent<Rigidbody>();

		if (jump.IsPressed()) {
			Vector3 moveVelocity = transform.TransformVector (
				movement.x * moveSpeed,
				jumpStrength,
				movement.y * moveSpeed
			);
			Vector3 currVelocity = new Vector3 (
				rb.linearVelocity.x,
				jumpStrength,
				rb.linearVelocity.z
			);
			if (movement.Equals(Vector2.zero)) {
				rb.linearVelocity = 
					transform.forward *
					currVelocity.magnitude;
			}
			rb.linearVelocity = 
				 moveVelocity.normalized * currVelocity.magnitude;
			return;
		}
		rb.linearVelocity = transform.TransformVector(
			movement.x * moveSpeed,
			0,
			movement.y * moveSpeed
		);
	}

	void MoveAirborne(Vector2 movement) {
		GetComponent<Rigidbody>().AddForce(transform.TransformVector(
			movement.x * airSpeed,
			0,
			movement.y * airSpeed
		));
	}

	void MoveCamera(Vector2 lookment) {
		gameObject.transform.Rotate(0, lookment.x * mouseSens, 0);
		camVertRot -= lookment.y * mouseSens;
		camVertRot = Mathf.Clamp(camVertRot, -89f, 89f);
		transform.GetChild(0).localEulerAngles =
			Vector3.right * camVertRot;
	}

	void Throw() {
			GameObject thrown = Instantiate(
				discharge,
				GetComponentInChildren<Transform>().position,
				Quaternion.identity
			);
			Physics.IgnoreCollision(
				GetComponent<Collider>(),
				thrown.GetComponent<Collider>()
			);
			thrown.GetComponent<Rigidbody>().linearVelocity =
				GetComponent<Rigidbody>().linearVelocity;
			thrown.GetComponent<Rigidbody>().AddForce(
				transform.GetChild(0).forward * throwStrength,
				ForceMode.VelocityChange
			);
			discharges.Enqueue(thrown);
	}

	private bool checkgrounded(Collision col) {
		List<ContactPoint> points = new List<ContactPoint>();
		col.GetContacts(points);
		foreach (ContactPoint contactPoint in points) {
			if (contactPoint.point.y <
				gameObject.transform.position.y - 0.95
			) return true;
		}
		return false;
	}

	private void OnCollisionStay(Collision col) {
		if (checkgrounded(col))
			grounded = true;
	}

	private void OnCollisionExit(Collision col) {
		if (!checkgrounded(col))
			grounded = false;
	}
}
