using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class playerController : MonoBehaviour
{
	public float moveSpeed;
	public float jumpStrength;
	public float airSpeed;
	public float mouseSens;
	InputAction move;
	InputAction jump;
	InputAction look;
	float camVertRot = 0f;
	bool grounded;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
		look = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
		Vector2 movement = move.ReadValue<Vector2>().normalized;
		Vector2 lookment = look.ReadValue<Vector2>();
		if (grounded) {
			GetComponent<Rigidbody>().linearVelocity = transform.TransformVector(new Vector3 (
				movement.x * moveSpeed,
				(jump.IsPressed()) ? jumpStrength : 0,
				movement.y * moveSpeed
			));
		} else {
			GetComponent<Rigidbody>().AddForce(
				movement.x * airSpeed,
				0,
				movement.y * airSpeed
			);
		}
		gameObject.transform.Rotate(0, lookment.x * mouseSens, 0);
		camVertRot -= lookment.y * mouseSens;
		camVertRot = Mathf.Clamp(camVertRot, -90f, 90f);
		foreach (Transform child in transform) {
			child.localEulerAngles = Vector3.right * camVertRot;
		}
    }

	private bool checkgrounded(Collision col) {
		List<ContactPoint> points = new List<ContactPoint>();
		col.GetContacts(points);
		foreach (ContactPoint contactPoint in points) {
			if (contactPoint.point.y < gameObject.transform.position.y - 0.95) {
				return true;
			}
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
