using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//creates mesh if needed
[RequireComponent(typeof(MeshFilter))]
//and mesh renderer
[RequireComponent(typeof(MeshRenderer))]

public class TriangleGenerator : MonoBehaviour
{
    //referenced brackeys vid<3: https://www.youtube.com/watch?v=eJEpeUH1EMg

    public Material mat;


    // Start is called before the first frame update
    void Start()
    {


        var verticesToSet = new List<Vector3>
        {
            //create triangle
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1)
        };

        var indicesToBeSet = new List<int>
        {
            //set indices of triangles
            0, 1, 2, //first triangle
            1, 3, 2  //second triangle shares first and third indice with first triangle with second forming quad
        };


        TriangleMesh triTest = new TriangleMesh();
        //add all vertices
        triTest.vertices = verticesToSet;
        //set indices to use vertices
        triTest.indices = indicesToBeSet;

        //gets mesh renderer and filter
        var meshRenderer = GetComponent<MeshRenderer>();
        var meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh.SetVertices(triTest.vertices);
        meshFilter.mesh.SetIndices(triTest.indices, MeshTopology.Triangles, 0); //last param is submesh
        //recalculate normals to make lighting not weird
        meshFilter.mesh.RecalculateNormals();

        meshRenderer.material = this.mat;
    }

}
