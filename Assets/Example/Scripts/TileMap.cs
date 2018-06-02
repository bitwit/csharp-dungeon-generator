using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x = 100;
	public int size_z = 100;
	public float tileSize = 1.0f;
	
	// Use this for initialization
	void Start () {
		BuildMesh();
	}
	
	public void BuildMesh() {

		var transform = GetComponent<Transform>();
		
		// var combine = new List<CombineInstance>();
		// var combineInstance = new CombineInstance();
		// combineInstance.mesh = buildPlaneMesh(new Vector3(tileSize, 0, tileSize));
		// combineInstance.transform = transform.worldToLocalMatrix;
		// combine.Add(combineInstance);

		// // var combineInstance2 = new CombineInstance();
		// // combineInstance2.mesh = buildPlaneMesh(new Vector3(tileSize, tileSize, 0));
		// // combineInstance2.transform = transform.worldToLocalMatrix;
		// // combine.Add(combineInstance2);

        // var mesh = new Mesh();
		// mesh.name = "KyleMesh";
		// mesh.Clear();
        // mesh.CombineMeshes(combine.ToArray(), true);

		var mesh = buildPlaneMesh(new Vector3(tileSize, 0, tileSize));
		
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
	}

	Mesh buildPlaneMesh(Vector3 planeSize) {

		int numTiles = size_x * size_z;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_z = size_z + 1;
		int numVerts = vsize_x * vsize_z;
		
		// Generate the mesh data
		Vector3[,] vertices = new Vector3[vsize_z, vsize_x];
		Vector3[,] normals = new Vector3[vsize_z, vsize_x];
		Vector2[,] uv = new Vector2[vsize_z, vsize_x];
		
		int[] triangles = new int[numTris * 3];

		int colCount, rowCount;
		for(rowCount=0; rowCount < vsize_z; rowCount++) {
			for(colCount=0; colCount < vsize_x; colCount++) {
				vertices[rowCount, colCount] = new Vector3(colCount * planeSize.x, 0, rowCount * planeSize.z);
				normals[rowCount, colCount] = Vector3.up;
				uv[rowCount, colCount] = new Vector2( (float)colCount / planeSize.x, (float)rowCount / (planeSize.y + planeSize.z) );
			}
		}
		
		for(rowCount=0; rowCount < size_z; rowCount++) {
			for(colCount=0; colCount < size_x; colCount++) {
				int squareIndex = rowCount * size_x + colCount;
				int triOffset = squareIndex * 6;

				int topLeft = rowCount * vsize_x + colCount + 0;  
				int topRight = rowCount * vsize_x + colCount + 1;  
				int bottomLeft = rowCount * vsize_x + colCount + vsize_x + 0;
				int bottomRight = rowCount * vsize_x + colCount + vsize_x + 1;

				triangles[triOffset + 0] = topLeft;
				triangles[triOffset + 1] = bottomLeft;
				triangles[triOffset + 2] = bottomRight;
				
				triangles[triOffset + 3] = topLeft;
				triangles[triOffset + 4] = bottomRight;
				triangles[triOffset + 5] = topRight;
			}
		}
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = flatten(vertices);
		mesh.triangles = triangles;
		mesh.normals = flatten(normals);
		mesh.uv = flatten(uv);

		return mesh;
	}

	public T[] flatten<T>(T[,] twoDArray) {
		var rowCount = twoDArray.GetLength(0);
		var colCount = twoDArray.GetLength(1);
		T[] newArray = new T[rowCount * colCount]; 

		for(int row = 0; row < twoDArray.GetLength(0); row++) {
			for(int col = 0; col < twoDArray.GetLength(1); col++) {
				var index = (row * colCount) + col;
				newArray[index] = twoDArray[row, col];
			}
		}
		return newArray;
	} 
	
}
