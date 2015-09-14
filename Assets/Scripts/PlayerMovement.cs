using UnityEngine;
using System.Collections;
using System.Xml.Schema;

public class PlayerMovement : MonoBehaviour 
{
	public float acceleration;
	public float jumpStrength;
	public float jumpSpeed;
	public float glideSpeed;
	public float glideGravityMultiplier;
	public float turnSmoothing;

	private Rigidbody rig;
	private bool grounded = false;
	private bool gliding = false;

	void Awake()
	{
		rig = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		grounded = groundCollision();

		moveCharacter (horizontal, vertical);
		if (Input.GetButtonDown ("Jump")) 
		{
			jump();
		}
		glide ();
	}

	/*
	 * Bewegt den Charakter abhängig vom Bewegungszustand
	 */
	private void moveCharacter(float horizontal, float vertical)
	{
		Vector3 velocityAxis = new Vector3 (horizontal, 0, vertical);

		velocityAxis = Quaternion.AngleAxis (Camera.main.transform.eulerAngles.y, Vector3.up) * velocityAxis;

		rotateCharacter (velocityAxis);

		if (grounded) 
		{
			velocityAxis = velocityAxis.normalized * acceleration;
		} 
		else 
		{
			velocityAxis = velocityAxis.normalized * jumpSpeed;
			velocityAxis = new Vector3(velocityAxis.x, rig.velocity.y, velocityAxis.z);
		}
		rig.velocity = velocityAxis;
	}

	/*
	 * Prüft, ob der Spieler an seiner Unterseite den Boden berührt.
	 */
	private bool groundCollision()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Collision");
		Bounds bounds = GetComponent<MeshFilter> ().mesh.bounds;
		return Physics.Raycast(transform.position+bounds.center, Vector3.down, bounds.extents.y + 0.1f, layerMask);
	}

	/*
	 * Führt die dem Bewegungszustand entsprechenden Sprungoperationen aus.
	 */
	private void jump()
	{
		if (grounded)
			rig.velocity = new Vector3(rig.velocity.x, jumpStrength, rig.velocity.z);
		else
			gliding = !gliding;
	}

	/*
	 * Manipuliert die Bewegung des Spielers sofern er gleitet.
	 */
	private void glide()
	{
		if (gliding) {
			float ySpeed = rig.velocity.y;
			rig.velocity = transform.forward * glideSpeed;
			rig.velocity = new Vector3(rig.velocity.x, ySpeed * glideGravityMultiplier, rig.velocity.z);
		}
		if (grounded) {
			gliding = false;
		}
	}

	/*
	 * Rotiert den Spieler in angegebene Richtung
	 */
	private void rotateCharacter(Vector3 direction)
	{
		if (direction.magnitude > 0) {
			Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
			Quaternion newRotation = Quaternion.Lerp(rig.rotation, targetRotation, turnSmoothing * Time.deltaTime);
			rig.MoveRotation(newRotation);
		}
	}
}
