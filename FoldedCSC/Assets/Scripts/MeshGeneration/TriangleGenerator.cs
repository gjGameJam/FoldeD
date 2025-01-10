using NUnit.Framework.Interfaces;
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
    MeshRenderer meshRenderer; //mesh renderer to render material
    MeshFilter meshFilter; //mesh filter to create mesh
    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        //gets mesh renderer and filter
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        //test triangle prism
        //Create3DTriangularPrism(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), .3f);

        //creates dart model with wingspan of 1 length of 1 and paper thicknes of .25
        CreateDartPlaneModel(1, 1, .25f);
    }

    void CreateDartPlaneModel(float wingspan, float length, float paperThickness)
    {
        Vector3 nose = new Vector3(length, 0, 0);
        Vector3 wingAdditionalLength = new Vector3(length / 11, 0, 0); //a little bit of length to cover up corners of rudder peeking through
        Vector3 back = new Vector3(0, 0, 0);
        Vector3 rightWingTip = new Vector3(0, 0, wingspan);
        Vector3 leftWingTip = new Vector3(0, 0, -wingspan);
        Vector3 bottomRudderTip = new Vector3(0, -wingspan, 0);

        // Offset for prism height (y is up so half of that for both sides in each dirction creates space)
        Vector3 prismVerticalOffset = new Vector3(0, paperThickness / 2, 0);
        Vector3 prismHorizontalOffset = new Vector3(0, 0, paperThickness / 3); //there's only one rudder but we still want it to be thin so it doesn't stick out

        //add right wing
        Create3DTriangularPrism(back, rightWingTip, nose + wingAdditionalLength, prismVerticalOffset);
        //add bottom wing
        Create3DTriangularPrism(back, bottomRudderTip, nose, prismHorizontalOffset);
        //add left wing
        Create3DTriangularPrism(nose + wingAdditionalLength, leftWingTip, back, prismVerticalOffset);
    }

    //creates two dimensional triangle mesh between provided points
    void Create3DTriangularPrism(Vector3 Point1, Vector3 Point2, Vector3 Point3, Vector3 prismOffset)
    {

        // calculate points of trianglular prism
        Vector3 Point1Top = Point1 + prismOffset;
        Vector3 Point1Bottom = Point1 - prismOffset;
        Vector3 Point2Top = Point2 + prismOffset;
        Vector3 Point2Bottom = Point2 - prismOffset;
        Vector3 Point3Top = Point3 + prismOffset;
        Vector3 Point3Bottom = Point3 - prismOffset;

        // add top and bottom faces
        AddTriangle(Point1Top, Point2Top, Point3Top); // Top face
        AddTriangle(Point1Bottom, Point3Bottom, Point2Bottom); // Bottom face (flipped order for correct normal)

        // add side faces
        AddRectangle(Point1Top, Point2Top, Point2Bottom, Point1Bottom); // Side 1
        AddRectangle(Point2Top, Point3Top, Point3Bottom, Point2Bottom); // Side 2
        AddRectangle(Point3Top, Point1Top, Point1Bottom, Point3Bottom); // Side 3

        // create mesh
        Mesh prismMesh = new Mesh();
        prismMesh.SetVertices(vertices);
        prismMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        prismMesh.RecalculateNormals(); //ensure lighting is correct
        prismMesh.RecalculateBounds(); // ensure the bounding box is correct

        // assigns new prism to mesh filter's mesh
        meshFilter.mesh = prismMesh;

        // assigns material to renderer
        if (meshRenderer != null && meshRenderer.material == null)
        {
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.SetFloat("_CullMode", (float)UnityEngine.Rendering.CullMode.Off); // Optional: Disable backface culling to render both sides
        }
    }

    // Helper function to add a triangle
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int baseIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        indices.Add(baseIndex);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
    }

    // helper function to add a rectangle (two triangles)
    void AddRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        // First triangle
        AddTriangle(v2, v1, v3);

        // Second triangle
        AddTriangle(v1, v4, v3);
    }





}
