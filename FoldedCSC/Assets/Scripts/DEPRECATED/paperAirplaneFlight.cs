using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneticSplicer;
using static SemiCircleFoldingAlgorithm;

public class paperAirplaneFlight : MonoBehaviour
{
    [Header("Constants")]
    private const float ACCELERATION_OF_GRAVITY = 9.81f; //  m/s^2
    private const float DENSITY_OF_AIR = 1.225f; //  kg/m^3 at sea level 15 degrees C
    private const float DYNAMIC_VISCOSITY_OF_AIR = 181000; //pascals at sea level 15 degrees C
    private const float COEFFICIENT_OF_LIFT = .6f; //~.6 approximation for this simulation, would need experimentation

    [Header("Variables")]
    [SerializeField] private float HeightToGround; //m


    [Header("FoldResults")]
    [SerializeField] private float wingspan;
    [SerializeField] private float Mass; //kg
    //surface area vals needed for drag and lift
    [SerializeField] private float surfaceAreaOfWing; //m^2
    [SerializeField] private float surfaceAreaOfFront; //m^2
    [SerializeField] private float surfaceAreaOfSide; //m^2
    [SerializeField] private float surfaceAreaTotal; //m^2

    [Header("Positioning")]
    //velocity and initial val
    Vector3 VelocityVector = new Vector3(0, 0, 0);
    [SerializeField] private float InitialLaunchVelocity;
    //start values used to determine distance travelled
    private float startZval;
    private float startYval;
    private float endZval;
    public float distanceTravelled;


    public event Action<GameObject> OnPlaneDestroyed;

    [Header("Genes")]
    public PaperAirplaneGeneSequence geneSequence; //geneSequence[0] = numberOfFolds;, geneSequence[1] = initialVelocity; geneSequence[2] = paperLength;, geneSequence[3] = paperWidth;. geneSequence[4] = paperHeight; geneSequence[5] = (float) paperType;  // Convert enum to float


    //sets the gene sequence and updates variables required for physics calculations
/*    public void setGeneSequence(PaperAirplaneGeneSequence newGeneSequence)
    {
        geneSequence.geneSequence = newGeneSequence.geneSequence;

        FoldingPizzaAlgorithm foldingAlgothm = GetComponent<FoldingPizzaAlgorithm>();

        PaperAirplanePhysicsAttributes results = foldingAlgothm.getPhysicsNumbers(newGeneSequence);

        //TODO: debug below section
        //Some math in here is fucky wuckying up (below)
        this.wingspan = results.getWingspan();
        this.Mass = results.getMass();
        this.surfaceAreaOfWing = results.getWingAreaTop();
        this.surfaceAreaOfFront = results.getFrontalArea();
        this.surfaceAreaOfSide = results.getSideArea();
        //Some math in here is fucky wuckying up (above)
        //TODO: need to figure out exactly how to get total surface area and if both sides of wings count for what
        Debug.Log("<color=purple>(m) set gene sequence </color>");
    }*/

    public float[] getGeneSequence()
    {
        return geneSequence.geneSequence;
    }

    // Start is called before the first frame update
    void Start()
    {
        //add initial launch velocity to current velocity
        VelocityVector.z = InitialLaunchVelocity;
        startZval = transform.position.z;
        startYval = transform.position.y;


    }

    /*    // Update is called once per frame
        void Update()
        {

        }*/

    /*
     * function for getting distance travelled (success metric)
     */
    private float getZScore()
    {
        return Mathf.Abs(startZval - transform.position.z);
    }


    /*
     * gets the newton force on the plane from force body diagram
     */
    private Vector3 getForceOnPlane(Vector3 velocity)
    {

        float liftForce = ForceOfLift(velocity.magnitude);
        //no X change for no roll
        //Y change due to gravity - lift
        //Z change due to drag (no thrust provided for gliders)
        return new Vector3(0, liftForce - ForceOfGravity(), ForceOfDrag(velocity.magnitude, liftForce));
    }


    /*
     * function for getting force of drag
     */
    private float ForceOfDrag(float velocity, float liftForce)
    {

        //apparently the velocity in this is the magnitude of the vector3
        float aspectRatio = getAspectRatio(); // AR
        //Finduced = Flift / ( p * V^2/2 * pi * AR * e)   // e is oswald efficiencyFactor
        float inducedDrag = Mathf.Pow(liftForce, 2) / (Mathf.Pow(velocity, 2) / 2.0f * DENSITY_OF_AIR * Mathf.PI * aspectRatio * getOswaldNumber(aspectRatio));
        //nose cutting through air
        float pressureDrag = Mathf.Pow(velocity, 2) / 2.0f * DENSITY_OF_AIR * surfaceAreaOfFront * getCoefficientOfDrag(velocity);
        //air rubbing on wings as flying happens
        float skinDrag = Mathf.Pow(velocity, 2) / 2.0f * DENSITY_OF_AIR * surfaceAreaTotal * getCoefficientOfDrag(velocity);

        float totalDrag = inducedDrag + pressureDrag + skinDrag;

        // Check if totalDrag is a valid float number
        if (float.IsNaN(totalDrag) || float.IsInfinity(totalDrag))
        {
            return 0f;
        }

        return totalDrag;


    }

    private float getCoefficientOfDrag(float velocityMag)
    {
        return 1.328f / Mathf.Sqrt(getReynoldsNumber(velocityMag));
    }



    private float getReynoldsNumber(float velocityMag)
    {
        //p * V * L / mu
        return velocityMag * DENSITY_OF_AIR * wingspan / DYNAMIC_VISCOSITY_OF_AIR;

    }

    private float getOswaldNumber(float aspectRatio)
    {
        return 1.78f * (1.0f - .045f * Mathf.Pow(aspectRatio, .68f)) - .64f;
    }

    private float getAspectRatio()
    {
        //multiplied by two for each wing
        return wingspan / (surfaceAreaOfWing * 2);
    }


    /*
     * function for getting force of lift
     */
    private float ForceOfLift(float velocity)
    {

        //apparently the velocity in this is the magnitude of the vector3
        //Cl * p * V^2/2 * S
        return COEFFICIENT_OF_LIFT * Mathf.Pow(velocity, 2) / 2 * DENSITY_OF_AIR * surfaceAreaOfWing;


    }

    private float ForceOfGravity()
    {
        return (float)(Mass * ACCELERATION_OF_GRAVITY);
    }

    private void FixedUpdate()
    {
        checkForEndOfFlight();
        //need to multiply all force calculations by delta time
        // Calculate acceleration
        Vector3 acceleration = getForceOnPlane(VelocityVector) / Mass;

        // Update velocity
        VelocityVector += acceleration * Time.deltaTime;

        // Update position by multiplying velocity 
        transform.position += VelocityVector * Time.deltaTime;
    }

    private void checkForEndOfFlight()
    {
        float currentFall = getHeightFallen();
        if (currentFall >= HeightToGround)
        {
            distanceTravelled = getZScore();
            //if plane goes down to ground level then delete it
            Debug.Log("<color=red>Plane died with distance of: </color>" + distanceTravelled + "<color=red>(m) after falling meters: </color>" + currentFall);
            Destroy(gameObject);
        }
    }

    private float getHeightFallen()
    {
        return startYval - transform.position.y;
    }


    void OnDestroy()
    {
        if (OnPlaneDestroyed != null)
        {
            OnPlaneDestroyed(gameObject);
        }
    }
}
