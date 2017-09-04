using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	public Chunk ChunkPrefab;
	private Chunk[,,] chunks;

	public int Width = 4;
	public int Height = 4;

	public float ChunkSize = 16;
	public int ChunkResolution = 16;

	void Awake () {
		chunks = new Chunk[Width, Height, Width];
		StartCoroutine(Generate());
	}

	IEnumerator Generate() {
		Noise noise = new Noise(System.DateTime.Now.Ticks.ToString().GetHashCode());
		for(int y = 0; y < Height; y++) { 
			for(int z = 0; z < Width; z++) {
				for(int x = 0; x < Width; x++) {
					Chunk c = Instantiate(ChunkPrefab,transform,false);
					c.transform.localPosition = new Vector3(x,y,z) * ChunkSize - new Vector3(ChunkSize * Width,ChunkSize * Height,ChunkSize * Width) * 0.5f;
					bool[] close = new bool[6];

					if(x == 0)
						close[2] = true;
					if(y == 0)
						close[0] = true;
					if(z == 0)
						close[4] = true;
					if(x == Width - 1)
						close[3] = true;
					if(y == Height - 1)
						close[1] = true;
					if(z == Width - 1)
						close[5] = true;

					c.closeSides = close;

					c.Initialize(x,y,z,ChunkResolution,ChunkSize,noise.Evaluate);
					chunks[x,y,z] = c;
				}
				yield return null;
			}
		}
	}
}
