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
	public float jumpSmoothing;
	public float glideSmoothing;

	private Rigidbody rig;
	private bool grounded = false;
	private bool gliding = false;
	private bool bonk = false;

	void Awake()
	{
		rig = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		bool jumpPressed = Input.GetButtonDown ("Jump");
		grounded = groundCollision();
		Debug.Log(grounded);

		Vector3 velocityAxis = buildAxis (horizontal, vertical);
		rotateCharacter (velocityAxis);
		bonk = wallCollision(velocityAxis);
		moveCharacter (velocityAxis);
		if (jumpPressed)jumpButtonHandler();

		updateGliding ();
	}

	/**
	 * Konstruiert Beschleunigungsvektor aus Input
	 */
	private Vector3 buildAxis(float horizontal, float vertical)
	{
		Vector3 velocityAxis = new Vector3 (horizontal, 0, vertical);

		velocityAxis = Quaternion.AngleAxis (Camera.main.transform.eulerAngles.y, Vector3.up) * velocityAxis;

		return velocityAxis;
	}

	/*
	 * Bewegt den Charakter entsprechend des aktuellen Bewegungszustandes
	 */
	private void moveCharacter(Vector3 velocityAxis)
	{
		if (grounded) {
			velocityAxis = velocityAxis.normalized * acceleration;
		} 
		else if (gliding) 
		{
			velocityAxis = transform.forward * glideSpeed;
			velocityAxis = new Vector3(velocityAxis.x, rig.velocity.y * glideGravityMultiplier, velocityAxis.z);
		}
		else
		{
			velocityAxis = velocityAxis.normalized * jumpSpeed;
			velocityAxis = new Vector3(velocityAxis.x, rig.velocity.y, velocityAxis.z);
		}
		if(!wallCollision(velocityAxis))rig.velocity = velocityAxis;
	}

	/*
	 * Führt die dem Bewegungszustand entsprechenden Sprungoperationen aus.
	 */
	private void jumpButtonHandler()
	{
		if (grounded)
			rig.velocity = new Vector3(rig.velocity.x, jumpStrength, rig.velocity.z);
		else
			gliding = !gliding;
	}

	/*
	 * Beendet Gleitflug, wenn Abbruchbedingungen erfüllt.
	 */
	private void updateGliding()
	{
		if (grounded||bonk) {
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
			Quaternion newRotation = Quaternion.Lerp(rig.rotation, targetRotation, (grounded? turnSmoothing : (gliding? glideSmoothing : jumpSmoothing))* Time.deltaTime);
			rig.MoveRotation(newRotation);
		}
	}

	/*
	 * Prüft, ob der Spieler an seiner Unterseite den Boden berührt.
	 */
	private bool groundCollision()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Collision");
		Bounds bounds = GetComponent<MeshFilter> ().mesh.bounds;
		Ray groundRay = new Ray (transform.position, Vector3.down);
		Debug.DrawRay (groundRay.origin, groundRay.direction, Color.blue);
		return Physics.Raycast(groundRay, bounds.extents.y + 0.1f, layerMask);
	}
	
	/*
	 * Prüft, ob der Spieler in angegebener Richtung mit der Wand kollidiert.
	 */
	private bool wallCollision(Vector3 direction)
	{
		direction = new Vector3 (direction.x, 0f, direction.z);
		int layerMask = 1 << LayerMask.NameToLayer("Collision");
		Bounds bounds = GetComponent<MeshFilter> ().mesh.bounds;
		Vector3 origin = transform.position;
		origin = new Vector3 (origin.x, origin.y+bounds.min.y+0.1f, origin.z);
		Debug.DrawRay(origin, direction, Color.red);
		return Physics.Raycast(transform.position+bounds.min, direction, bounds.extents.z+1f, layerMask);
	}

}
