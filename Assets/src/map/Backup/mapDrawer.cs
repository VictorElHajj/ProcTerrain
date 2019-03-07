using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEditor;

public class mapDrawer : MonoBehaviour
{
    public terrainMap Map;
    Vector3[] vectors;
    void Start() //Change to start again
    {
        Map = new terrainMap(250, 250, 1338);
        double[,][] map = Map.map;
        int width  = map.GetLength(1);
        int heigth = map.GetLength(0);

        vectors = mapToVector3(map);

        int[] triangles = createTriangles(vectors, width, heigth);

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

    //Flattens 2D double array map into a 1D array of Vectors
    Vector3[] mapToVector3 (double[,][] map) {
        Vector3[] v = new Vector3[map.GetLength(0)*map.GetLength(1)];
    
        //Foreach is just simpler than two for loops, less off by one errors..
        int i = 0;
        foreach (double[] vec in map) { 
            v[i] = new Vector3 ((float)vec[0], (float)vec[1], (float)vec[2]);
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
