using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    private Vector3 startingPos; //the starting position for all paper plane spawns
    private List<GameObject> activeGliders = new List<GameObject>(); // keep track all spawned paper airplanes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //getter for the gliders still flying
    List<GameObject> getActiveGliders()
    {
        return activeGliders;
    }

    //helper function to get the furthest glider; returns distance thrown from starting point (x dist travelled)
    float getDistFromStart(Vector3 pos)
    {
        return pos.x - startingPos.x;
    }

    // Function to get the glider farthest from the starting position
    public GameObject GetFarthestGlider()
    {
        //start with no glider and low furthest distance
        GameObject farthestGlider = null;
        float maxDistance = float.MinValue;

        List<GameObject> glidersCopy = new List<GameObject>(activeGliders); //shallow copy of gliders because some might get removed
        if (glidersCopy.Count == 0)
        {
            return null; //if there are no gliders return null early
        }
        foreach (var glider in glidersCopy)
        {
            if (glider != null) // Ensure the glider is valid
            {
                //get distance from start and update farthest glider and distance if distance is further than current max distance
                float distance = getDistFromStart(glider.transform.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestGlider = glider;
                }
            }
        }

        return farthestGlider;
    }


}
