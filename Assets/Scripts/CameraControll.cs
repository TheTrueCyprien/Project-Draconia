using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {

	public GameObject target;
	public Vector3 offset = new Vector3(0,10,-20);
	public float speed = 10f;

	private Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(camera != null && target != null)
		{
			Vector3 targetPos = target.transform.position;
			Vector3 localOffset = offset;

			float cameraAngle = camera.transform.eulerAngles.y;
			float targetAngle = target.transform.eulerAngles.y;

			if(Input.GetAxisRaw("Vertical") < 0.2f)
			{
				targetAngle = cameraAngle;
			}

			targetAngle = Mathf.LerpAngle(cameraAngle, targetAngle, speed * Time.deltaTime);
			localOffset = Quaternion.Euler(0,targetAngle,0) * offset;

			camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos + localOffset, speed * Time.deltaTime);
			camera.transform.LookAt(targetPos);
		}
	}
}
