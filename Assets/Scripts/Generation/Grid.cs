using System.Collections;
using System;
using System.Linq;
using System.Threading;
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

	public Transform Observer;

	private Noise noise;

	private List<MeshRequest> requests = new List<MeshRequest>();

	void Awake () {
		instance = this;
		chunks = new Dictionary<Vector3,Chunk>();
		noise = new Noise(DateTime.Now.Ticks.ToString().GetHashCode());

		if(Observer == null)
			Observer = transform;
	}

	public void RequestMeshData(Chunk chunk, Action<MeshData> callback) {
		requests.Add(new MeshRequest(chunk, callback));
	}

	private void Update() {
		if (requests.Count > 0) {
			MeshRequest request = requests.OrderBy(x => Vector3.SqrMagnitude(x.chunk.centerPos - Observer.transform.position)).First();
			requests.Remove(request);

			lock (requests) {
				ThreadStart thread = delegate {
					request.chunk.March(request.callback);
				};
				thread.Invoke();
			}
		}
	}

	public Vector3 worldToChunkCoordinates (Vector3 worldPos) {
		Vector3 pos = worldPos / ChunkSize;
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
			RequestMeshData(c,c.Apply);
			chunks.Add(new Vector3(x,y,z),c);
			return c;
		}
		return null;
	}
}

public struct MeshRequest {
	public Chunk chunk;
	public Action<MeshData> callback;

	public MeshRequest(Chunk chunk, Action<MeshData> callback) {
		this.chunk = chunk;
		this.callback = callback;
	}
}

public struct MeshData {
	public Vector3[] vertices;
	public int[] triangles;

	public MeshData(List<Vector3> v,List<int> t) {
		vertices = v.ToArray();
		triangles = t.ToArray();
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		return mesh;
	}
}
