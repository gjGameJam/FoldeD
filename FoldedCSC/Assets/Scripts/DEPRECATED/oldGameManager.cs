using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GeneticSplicer;

public class oldGameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject planePrefab;

    //[SerializeField]
    private GeneticSplicer geneFunctions;

    //where the paper airplanes should all spawn
    private GameObject startPoint;

    private int simulation_round_number = 0;

    private bool sim_is_running;

    private float simulation_time_multiplier;

    private List<GameObject> activePlanes = new List<GameObject>(); // Track all spawned planes
    private int planesPerRound = 5;  // Limit number of planes per round

    private float firstPlaceCompetitorDistance;
    PaperAirplaneGeneSequence firstPlaceGenes;
    private float secondPlaceCompetitorDistance;
    PaperAirplaneGeneSequence secondPlaceGenes;


    // Start is called before the first frame update
    void Start()
    {
        simulation_round_number = 0;
        simulation_time_multiplier = 1;
        Debug.Log("initialize sim");
        sim_is_running = false;
        geneFunctions = new GeneticSplicer();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(activePlanes.Count);
        if (sim_is_running && activePlanes.Count == 0)
        {
            //spawn new set of planes when sim is running and all old compeititors lost
            //Debug.Log("spawn planes!!!");
            spawnPlanes();



        }
    }


    public void spawnPlanes()
    {
        if (startPoint == null)
        {
            //the startpoint game object 
            startPoint = GameObject.FindWithTag("StartPoint");
            Debug.Log(startPoint.transform.position);
        }



        //if first round then create first generation
        if (simulation_round_number == 0)
        {
            for (int i = 0; i < planesPerRound; i++)
            {

                // Instantiate the paper airplane at the start point
                GameObject newPlane = Instantiate(planePrefab, startPoint.transform.position, Quaternion.identity);
                activePlanes.Add(newPlane);

                // Get the paperAirplaneFlight component from the instantiated plane
                paperAirplaneFlight planeScript = newPlane.GetComponent<paperAirplaneFlight>();
                planeScript.OnPlaneDestroyed += HandlePlaneDestroyed;  // Subscribe to the destroy event
                // Call the default constructor from GeneticSplicer to create a new PaperAirplaneGeneSequence
                GeneticSplicer.PaperAirplaneGeneSequence newGeneSequence = new GeneticSplicer.PaperAirplaneGeneSequence(GeneticSplicer.PaperType.Letter);
                //set gene sequence to be used for fold calculations
                if (planeScript != null)
                {
                    //planeScript.setGeneSequence(newGeneSequence);
                }
            }


        }
        else //if not first round then new geneneration is offspring of top performers + top performer
        {
            Debug.Log("spawn planes based on winners of last round");
            PaperAirplaneGeneSequence[] childGeneArray = geneFunctions.GeneratePaperAirplaneOffspring(firstPlaceGenes.geneSequence, secondPlaceGenes.geneSequence, 6);
            for (int i = 0; i < childGeneArray.Length; i++)
            {
                // Instantiate the paper airplane at the start point
                GameObject newPlane = Instantiate(planePrefab, startPoint.transform.position, Quaternion.identity);
                activePlanes.Add(newPlane);

                // Get the paperAirplaneFlight component from the instantiated plane
                paperAirplaneFlight planeScript = newPlane.GetComponent<paperAirplaneFlight>();
                planeScript.OnPlaneDestroyed += HandlePlaneDestroyed;  // Subscribe to the destroy event
                //set gene sequence to be used for fold calculations
                if (planeScript != null)
                {
                   // planeScript.setGeneSequence(childGeneArray[i]);
                }
            }
        }

        simulation_round_number++;

    }


    // Callback function when a plane is destroyed
    private void HandlePlaneDestroyed(GameObject plane)
    {

        //gets the distance travelled
        paperAirplaneFlight flightScript = plane.GetComponent<paperAirplaneFlight>();
        float distTravelled = flightScript.distanceTravelled;
        //check if dist travelled was first or second greatest
        if (distTravelled > firstPlaceCompetitorDistance)
        {
            //set genes to new flight gene pool
            firstPlaceCompetitorDistance = distTravelled; //update the highest score for generation
            firstPlaceGenes = flightScript.geneSequence;
            Debug.Log("Plane destroyed for first place: " + flightScript.getGeneSequence());
        }
        else if (distTravelled > secondPlaceCompetitorDistance)
        {
            secondPlaceCompetitorDistance = distTravelled; //update the second highest score for generation
            secondPlaceGenes = flightScript.geneSequence;
            Debug.Log("Plane destroyed for second place: " + flightScript.getGeneSequence());
        }


        activePlanes.Remove(plane);
    }

/*    public void updateSimulationTimeDelta()
    {
        //updates the time scale when the slider is moved
        simulation_time_multiplier = timeScaleSlider.value;
        //need to multiply all paper airplane flight's delta time by this number (starts at 1)
        Debug.Log("new time scale: " + simulation_time_multiplier);
    }*/
}
