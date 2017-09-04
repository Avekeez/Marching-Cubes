using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExplore : MonoBehaviour {

	float rotationX = 0;
	float rotationY = 0;

	List<float> listRotationX = new List<float>();
	List<float> listRotationY = new List<float>();

	Quaternion quaternionX;
	Quaternion quaternionY;
	Quaternion original;

	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		original = transform.localRotation;
	}

	void LateUpdate () {
		transform.Translate(new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Up Down"),Input.GetAxis("Vertical")) * Time.deltaTime * 10);

		float averageRotationX = 0;
		rotationX += Input.GetAxis("Mouse X") * 2;
		listRotationX.Add(rotationX);
		if (listRotationX.Count >= 4) {
			listRotationX.RemoveAt(0);
		}

		for (int xi = 0; xi < listRotationX.Count; xi++) {
			averageRotationX += listRotationX[xi];
		}
		averageRotationX /= listRotationX.Count;

		float averageRotationY = 0;
		rotationY += Input.GetAxis("Mouse Y") * 2;
		listRotationY.Add(rotationY);
		if(listRotationY.Count >= 4) {
			listRotationY.RemoveAt(0);
		}

		for(int yi = 0; yi < listRotationY.Count; yi++) {
			averageRotationY += listRotationY[yi];
		}
		averageRotationY /= listRotationY.Count;

		quaternionX = Quaternion.AngleAxis(averageRotationX,Vector3.up);
		quaternionY = Quaternion.AngleAxis(averageRotationY,Vector3.left);
		transform.localRotation = original * quaternionX * quaternionY;
    }
}
