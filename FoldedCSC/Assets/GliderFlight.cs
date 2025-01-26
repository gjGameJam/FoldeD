using Unity.VisualScripting;
using UnityEngine;
using static SemiCircleFoldingAlgorithm;

/**
 * script to be attatched to each glider to calculate aerodynamic pathing based on PaperAirplanePhysicsAttributes and current velocity/rotations
 * 
 * @author Grant Benson
 **/
public class GliderFlight : MonoBehaviour
{
    GeneSequence geneSeq;
    PaperAirplanePhysicsAttributes physNums;
    Transform parentTransform; //glider object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the parent of the current GameObject
        parentTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: use gene sequence.GetInitialVelocity() to create linear flight paths for testing
        if (physNums.CanFly())
        {
            //add movement to parent position
            float xMovementAmount = geneSeq.GetInitialVelocity() * Time.deltaTime;
            float yMovementAmount = 2 * Time.deltaTime;
            parentTransform.position += new Vector3(xMovementAmount, -yMovementAmount, 0);
        }
    }

    //sets the surface areas, center of mass, etc used in flight calculations
    public void SetPhysicsResults(PaperAirplanePhysicsAttributes results)
    {
        physNums = results;
    }

    public void SetGeneSequence(GeneSequence geneSequence)
    {
        //create copy of gene sequence and save for future use
        geneSeq = geneSequence.Clone();
    }

    //before destroy each glider needs to save distance and gene sequence in gene manager
    void OnDestroy()
    {
        //geneManager.updateBestCompetitor(GeneSequence geneRep, float distTravelled);
    }
}
