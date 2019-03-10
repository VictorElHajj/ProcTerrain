using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEditor;

public class TerrainDrawer : MonoBehaviour
{
    public Terrain terrain;
    Vector3[] vectors;
    void Start() //Change to start again
    {
        terrain = new Terrain(255, 255, 1338);
        PointMap Map = terrain.Map;
        int width  = Map.Width;
        int height = Map.Height;

        vectors = mapToVector3(Map);

        int[] triangles = createTriangles(vectors, width, height);

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vectors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }
    //DEBUG INFO
    /*private void OnDrawGizmos () {
		Gizmos.color = Color.black;
		for (int i = 0; i < vectors.Length; i++) {
			Gizmos.DrawSphere(vectors[i], 0.03f);
            Handles.color = Color.red;
            Handles.Label(vectors[i], i.ToString());
		}
    }*/

    //Flattens Pointmap into a 1D array of Vectors
    Vector3[] mapToVector3 (PointMap Map) {
        Vector3[] v = new Vector3[Map.Width*Map.Height];
    
        //Foreach is just simpler than two for loops, less off by one errors..
        int i = 0;
        foreach (Point point in Map.Points) { 
            (int x, double y, int z) = point.Pos;
            v[i] = new Vector3 ((float)x, (float)y, (float)z);
            i++;
        }
        /* Ordered like this in a 10,4 size map
        0  1  2  3  4  5  6  7  8  9
        10 11 12 13 14 15 16 17 18 19
        20 21 22 23 24 25 26 27 28 29
        30 31 32 33 34 35 36 37 38 39
         */
        return v;
    }

    //Creates triangles using the vectors as vertices, assumes a grid shape like in the function above.
    int[] createTriangles (Vector3[] vectors, int width, int heigth) {
        List<int> triangles = new List<int>();

        for (int i = 0; i < vectors.GetLength(0)-width; i+=width) {
            for (int j = 0; j < width-1; j++) {
                triangles.Add(i+j);
                triangles.Add(i+j+width+1);
                triangles.Add(i+j+width);

                triangles.Add(i+j+width+1);        
                triangles.Add(i+j);
                triangles.Add(i+j+1);
            }
        }
        return triangles.ToArray();
    }
}
