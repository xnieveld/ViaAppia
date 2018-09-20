using System.Collections;
using UnityEngine;

public class CameraController: MonoBehaviour {
	private Vector3 offset;

	[Header("Variables")]
	public Transform player;

	[Space]
	[Header("Position")]
	public float camPosX;
	public float camPosY;
	public float camPosZ;

	[Space]
	[Header("Rotation")]
	public float camRotationX;
	public float camRotationY;
	public float camRotationZ;

	[Space]
	[Range(0f, 10f)]
	public float turnSpeed;

	bool isRotating = false;

	private void Start() {
		offset = new Vector3(player.position.x + camPosX, player.position.y + camPosY, player.position.z + camPosZ);
		transform.rotation = Quaternion.Euler(camRotationX, camRotationY, camRotationZ);
	}


	private void LateUpdate() {

		if (isRotating) {
			offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offset;
			transform.position = player.position + offset;
			transform.LookAt(player.position);
		}


		if (Input.GetMouseButtonDown(0)) {
			isRotating = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			isRotating = false;
		}
	}
}
    //    void Update() {
    //        transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime);
    //    }
//}