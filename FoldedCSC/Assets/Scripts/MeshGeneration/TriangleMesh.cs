using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//avoiding using monobehavior to optimize performance
public class TriangleMesh
{
    //vertices for corners of triangle
    public List<Vector3> vertices = new List<Vector3>();
    //indices for corners
    public List<int> indices = new List<int>();
}
