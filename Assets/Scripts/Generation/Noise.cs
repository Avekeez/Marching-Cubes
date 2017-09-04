using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {
	FastNoise noise;

	public Noise (int seed) {
		noise = new FastNoise(seed);
	}

	public float Sample (Vector3 point) {
		float result = 0;
		float normalizeFactor = 0;
		float frequency = 5;
		float amplitude = 0.1f;

		for (int i = 0; i < 5; i ++) {
			normalizeFactor += amplitude;
			result += amplitude * noise.GetSimplex(frequency * point.x,frequency * point.y,frequency * point.z);
			frequency *= 2;
			amplitude *= 0.5f;
		}
		return result / normalizeFactor;
	}

	public bool Evaluate (Vector3 point) {
		float distance = Vector3.Distance(point,Vector3.zero);

		return (Sample(point)+1f)/2f < 0.5f;
	}
}
