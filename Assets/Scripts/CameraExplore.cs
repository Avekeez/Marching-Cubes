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

	private int viewDistance = 3;

	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		original = transform.localRotation;
	}

	void LateUpdate () {
		Vector3 chunkPos = Grid.instance.worldToChunkCoordinates(transform.position);

		foreach(KeyValuePair<Vector3,Chunk> k in Grid.instance.chunks) {
			Grid.instance.UnloadChunk(k.Key);
		}
		for(int x = -viewDistance; x <= viewDistance; x++) {
			for(int y = -viewDistance; y <= viewDistance; y++) {
				for(int z = -viewDistance; z <= viewDistance; z++) {
					Grid.instance.LoadChunk(chunkPos + new Vector3(x,y,z));
				}
			}
		}

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

	public float ClampAngle(float angle, float min, float max) {
		angle = angle % 360;
		if (angle >= -360 && angle <= 360) {
			if(angle < -360)
				angle += 360;
			if(angle > 360)
				angle -= 360;
		}
		return Mathf.Clamp(angle,min,max);
	}
}
