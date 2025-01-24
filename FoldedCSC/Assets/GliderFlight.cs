using UnityEngine;
using static SemiCircleFoldingAlgorithm;

/**
 * script to be attatched to each glider to calculate aerodynamic pathing based on PaperAirplanePhysicsAttributes and current velocity/rotations
 * 
 * @author Grant Benson
 **/
public class GliderFlight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: use gene sequence.GetInitialVelocity() to create linear flight paths for testing
    }

    //sets the surface areas, center of mass, etc used in flight calculations
    void SetPhysicsResults(PaperAirplanePhysicsAttributes results)
    {

    }

    void SetGeneSequence(GeneSequence geneSequence)
    {
        //create copy of gene sequence and save for future use
    }

    //before destroy each glider needs to save distance and gene sequence in gene manager
    void OnDestroy()
    {
        //geneManager.updateBestCompetitor(GeneSequence geneRep, float distTravelled);
    }
}
