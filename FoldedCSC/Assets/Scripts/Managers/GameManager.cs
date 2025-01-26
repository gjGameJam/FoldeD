using System.Collections.Generic;
using UnityEngine;
using static SemiCircleFoldingAlgorithm;

/**
 * Game manager handles logic regarding camera/ui updating, paper folding, gene splicing, (all delegated to their own managers) and glider spawning (handled by glider(s))
 * 
 * @author Grant Benson
 **/
public class GameManager : MonoBehaviour
{

    [SerializeField] private Vector3 startingPos; //the starting position for all paper plane spawns
    private List<GameObject> activeGliders = new List<GameObject>(); // keep track all spawned paper airplanes
    [SerializeField] private GameObject gliderModel; //glider prefab to spawn (consider strengthening connection)
    [SerializeField] private LeaderboardController leaderBoardScript; //controls leaderboard
    [SerializeField] private DistanceGraphing distanceGraphScript; //controls leaderboard
    [SerializeField] private GeneManger geneManager; //controls genetics
    [SerializeField] private int numGlidersPerRound = 7; //how many gliders should each round have?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //creates test glider and adds it to active gliders
        activeGliders.Add(Instantiate(gliderModel, startingPos, Quaternion.identity));
        //Debug.Log("spawnedPlane");
    }

    //function to spawn set of gliders upon the start of each round
    private void SpawnGlidersForRound()
    {
        //uses gene manager to get array of mutated gene sequences
        GeneSequence[] genes = geneManager.GenerateOffspring(numGlidersPerRound);
        //loop through genes, creating new glider and setting its gene sequence
        for (int i = 0; i < genes.Length; i++)
        {

            //create glider object at starting point
            GameObject newGlider = Instantiate(gliderModel, startingPos, Quaternion.identity);

            //perform folding calculations from gene sequence to get correct physics and mesh generation numbers
            PaperAirplanePhysicsAttributes foldResults = new PaperAirplanePhysicsAttributes(genes[i].GetNumberOfFolds(), genes[i].GetPaperHeight(), genes[i].GetPaperWidth(), genes[i].GetPaperDensity(), false); //(int numFolds, float radius, float thickness, float density, bool canFly

            //use the triangle mesh generation script and input dimensions from folding algorithm to create triangle mesh model of paper
            if (genes[i].GetNumberOfFolds() == 0)
            {
                newGlider.GetComponent<TriangleGenerator>().CreateDartPlaneModel(foldResults.calculateWingspan(), foldResults.getRadius(), foldResults.getThicknessFolded(), true); //create unfolded model
            }
            else
            {
                newGlider.GetComponent<TriangleGenerator>().CreateDartPlaneModel(foldResults.calculateWingspan(), foldResults.getRadius(), foldResults.getThicknessFolded(), false); //create folded model
            }

            //need to assign gene sequence gene[i] and foldResults to each glider
            GliderFlight flightCalcs = newGlider.GetComponent<GliderFlight>();
            flightCalcs.SetPhysicsResults(foldResults);
            flightCalcs.SetGeneSequence(genes[i]);

            //finally add glider as an active glider now that physics, genes, and rendering are handled
            activeGliders.Add(newGlider);

            //enable movement in glider (disabled by default) now that everything is set and glider is active
            foldResults.AllowFlight(); //now allow the glider to fly once folding calculations and mesh generation are complete

        }
        //end of loop spawning all gliders
    }



    //TESTING SECTION to make sure dynamic adjustment of UI
    private float timer = 0f;  // Timer to track time passed
    private float interval = 1.5f;  // Time interval in seconds
    //private float testDist = 5;
    int testNum = 0;
    // Update is called once per frame
    void Update()
    {
        // Increment the timer by the time passed since the last frame
        timer += Time.deltaTime;

        // Check if interval amount of time has passed
        if (timer >= interval)
        {
            AddResultsToUI($"timothy{testNum++}", 0, Random.Range(1.0f, 100.0f));

            // Reset the timer to reuse
            timer = 0f;
        }

    }//END OF TESTING SECTION to make sure dynamic adjustment of UI works

    //updates leaderboard controller and distance graphing each time data is available
    void AddResultsToUI(string gliderName, int generationNum, float distanceTravelled)
    {
        //update leaderboard with text
        leaderBoardScript.AddToLeaderboard(gliderName, generationNum, distanceTravelled);
        //update distance graph with new distance
        distanceGraphScript.AddToGraph(distanceTravelled);
        //look to prevent coupling with camera manager (maybe update from here when leader dies?)

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
