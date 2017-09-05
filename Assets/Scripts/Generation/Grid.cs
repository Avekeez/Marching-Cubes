using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	public static Grid instance;

	public Chunk ChunkPrefab;
	public Dictionary<Vector3,Chunk> chunks;

	public int Width = 4;
	public int Height = 4;

	public float ChunkSize = 16;
	public int ChunkResolution = 16;

	public bool Infinite = false;
	public int ViewDistance = 3;

	private Noise noise;

	void Awake () {
		instance = this;
		chunks = new Dictionary<Vector3,Chunk>();
		//chunks = new Chunk[Width, Height, Width];
		noise = new Noise(System.DateTime.Now.Ticks.ToString().GetHashCode());
		//StartCoroutine(Generate());
	}

	IEnumerator Generate() {
		for(int y = 0; y < Height; y++) { 
			for(int z = 0; z < Width; z++) {
				for(int x = 0; x < Width; x++) {
					generateChunk(x,y,z);
				}
			}
		}
		yield return null;
	}

	public Vector3 worldToChunkCoordinates (Vector3 worldPos) {
		Vector3 pos = worldPos / ChunkSize;
		//print(ChunkSize);
		return new Vector3(
			Mathf.FloorToInt(pos.x),
			Mathf.FloorToInt(pos.y),
			Mathf.FloorToInt(pos.z));
	}

	public Chunk LoadChunk (Vector3 chunkPos) {
		return LoadChunk(
			Mathf.FloorToInt(chunkPos.x),
			Mathf.FloorToInt(chunkPos.y),
			Mathf.FloorToInt(chunkPos.z));
	}

	public Chunk LoadChunk (int x, int y, int z) {
		Vector3 key = new Vector3(x,y,z);
		Chunk c;
        if (chunks.ContainsKey(key)) {
			c = chunks[key];
			//print(c.transform.position);
		} else {
			c = generateChunk(x,y,z);
		}
		c.gameObject.SetActive(true);
		return c;
	}

	public Chunk UnloadChunk(Vector3 chunkPos) {
		return UnloadChunk(
			Mathf.FloorToInt(chunkPos.x),
			Mathf.FloorToInt(chunkPos.y),
			Mathf.FloorToInt(chunkPos.z));
	}

	public Chunk UnloadChunk (int x, int y, int z) {
		Vector3 key = new Vector3(x,y,z);
		Chunk c = null;
		if (chunks.ContainsKey(key)) {
			c = chunks[key];
			c.gameObject.SetActive(false);
		}
		return c;
	}

	private Chunk generateChunk (int x, int y, int z) {
		if (!chunks.ContainsKey(new Vector3 (x,y,z))) {
			Chunk c = Instantiate(ChunkPrefab,transform,false);
			c.transform.localPosition = (new Vector3(x,y,z) - Vector3.one * 0.5f) * ChunkSize;
			bool[] close = new bool[6];
			if (!Infinite) {
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
			}

			c.closeSides = close;

			c.Initialize(x,y,z,ChunkResolution,ChunkSize,noise.Evaluate);
			chunks.Add(new Vector3(x,y,z),c);
			return c;
		}
		return null;
	}
}
