using UnityEngine;


/*
 * Algorithm to get surface areas of paper airplane such that the piece of paper is a half circle and each wing is a symmetrical quarter circle
 * given a radius and number of folds, provide surface area of each face making contact with air.
 * 
 * 
 * Author: Grant Benson
 */
public class SemiCircleFoldingAlgorithm : MonoBehaviour
{

    //testing purposes
    void Awake()
    {
        //Debug.Log("Start1");
        PaperAirplanePhysicsAttributes airplaneAttributes = new PaperAirplanePhysicsAttributes(
            numFolds: 1,
            radius: 7.0f,
            thickness: 0.2f,
            mass: 0.2f
        );
        Debug.Log(airplaneAttributes.ToString());
    }


    public struct PaperAirplanePhysicsAttributes
    {
        private const float THICKNESS_MULTIPLIER_PER_FOLD = 1.7f;
        private const float NOSE_STARTING_ANGLE = 90.0f;


        private float radius;
        private float thickness;
        private int numberOfFoldds;
        //sent variables
        private float middleArea; //the middle part of the paper airplane that you hold
        private float frontArea; //the part that goes into the wind/air
        private float topArea;  //the part that is visible from the top down view
        private float mass;  //weight of paper airplane (Kg)

        //physical attributes of a wing+middle part (half of a plane)
        public PaperAirplanePhysicsAttributes(int numFolds, float radius, float thickness, float mass)
        {
            //initialize number of folds, radius, thickness, and calculate mass via volume and density from paper type
            this.numberOfFoldds = numFolds;
            this.radius = radius;
            this.thickness = thickness;
            this.mass = mass;
            this.middleArea = -1;
            this.frontArea = -1;
            this.topArea = -1;
            middleArea = getMiddleArea();
            topArea = getTopArea();
            frontArea = getFrontArea();
        }

        public override string ToString()
        {
            return $"Paper Airplane Physics Attributes:\n" +
                   $"- Number of Folds: {numberOfFoldds}, Radius: {radius}, Thickness: {thickness}, Mass: {mass}, Middle Area: {middleArea}, Front Area: {frontArea}, Top Area: {topArea}";
        }



        //gets the middle area of one side of plane (there will be two)
        private float getMiddleArea()
        {
            float quarterCircleArea = getSAofCircleOfRadius(radius) / 4; //(pi * r^2 / 4) is quarter circle
            float fractionOfAreaDueToFolds = Mathf.Pow((.5f), numberOfFoldds); //(1 / 2) ^ num folds becuase the area will get halved every time with the pizza model
            return quarterCircleArea * fractionOfAreaDueToFolds; //quarter circle * fractionOfAreaDueToFolds will be the circles area given number of folds
        }

        //gets the wing area of one side of plane (there will be two)
        private float getTopArea()
        {
            //wings do not exist if there have been no folds yet
            switch (numberOfFoldds)
            {
                case 0:
                    return thickness * radius; //if there are no folds the wing area will be the side of the paper (thickness * length of paper which will be radius here) pointing up
                default:
                    //top of wing and bottom of wing (the body) will always be symmetrical (assuming at least one fold has been made)
                    return getMiddleArea();
            }

        }

        private float getSAofCircleOfRadius(float radius)
        {
            //PI * R^2 = circle surface area
            return Mathf.PI * Mathf.Pow(radius, 2);
        }

        // AAS Formula: Given a nose angle and radius, calculate the opposite side length.
        public float GetOppositeSideLength(float noseAngleRadians, float radius)
        {
            // Use the sine of the nose angle to calculate the opposite side.
            float oppositeSide = radius * Mathf.Sin(noseAngleRadians);

            // Return the calculated opposite side length.
            return oppositeSide;
        }

        //gets the wingspan of one wing given the number of folds and radius
        private float calculateWingspan()
        {

            //if no folds have occured the wing span will be the thickness of the paper
            switch (numberOfFoldds)
            {
                case 0:
                    //Debug.Log("zero folds!");
                    return thickness; //if there are no folds the wing span will just be the thickness of the paper
                default:
                    float noseAngleInDegrees = getNoseAngle();
                    //Debug.Log("num of folds for wing span " + numberOfFoldds);
                    // Convert angle to radians because Mathf.Cos expects radians
                    float NoseAngleInRadians = noseAngleInDegrees * Mathf.Deg2Rad;

                    // The opposite side of a triangle formed by the hypotenuse (which will always be the radius for pizza example), nose angle, and right angle will be the wingspan
                    float wingspan = GetOppositeSideLength(NoseAngleInRadians, radius);

                    //wingspan is the longest distance from the middle to the wing edge
                    return wingspan;
            }
        }

        //nose angle starts at 90 and halves each fold
        private float getNoseAngle()
        {
            //angle will halve every fold because sides touch and crease becomes new hypotenuse (or side)
            return NOSE_STARTING_ANGLE * Mathf.Pow((.5f), numberOfFoldds);
        }

        //gets surface area for one side of front of plane using wingspan x width
        private float getFrontArea()
        {
            //if no folds have occured the frontal area will be half of thickness * radius / 2 (half of upright piece of paper)
            switch (numberOfFoldds)
            {
                case 0:
                    //Debug.Log("zero folds!");
                    return thickness * radius / 2; //if there are no folds the frontal area is thickness * radius / 2 (because this is for each side)
                default:
                    //frontal area is the area that goes into the air with forward movement
                    //therefore it will be the thickness of the paper multiplied by the length exposed
                    //there will be two segments (middle and wing) of equal thickness and length (wingspan)
                    float wingSpan = calculateWingspan();
                    //thickness of wing multiplied by wingspan is the area exposed to forward air by one side
                    float segmentArea = getThicknessFolded() * wingSpan;
                    return segmentArea;
            }
        }

        //paper folded upon itself will be around 1.7 times the thickness of the original
        private float getThicknessFolded()
        {
            //thickness of paper increases exponentially per fold
            return thickness * Mathf.Pow(THICKNESS_MULTIPLIER_PER_FOLD, numberOfFoldds);
        }

        public readonly float getMass()
        {
            return mass;
        }

        public readonly float getWingAreaTop()
        {
            return topArea;
        }

        public readonly float getFrontalArea()
        {
            return frontArea;
        }

        public readonly float getSideArea()
        {
            return middleArea;
        }
    }


/*    public PaperAirplanePhysicsAttributes getPhysicsNumbers(PaperAirplaneGeneSequence genes)
    {
        int numFolds = genes.GetNumFolds(); //get number of folds
        float thickness = genes.GetWidth(); //width for thickness of paper
        float radius = genes.GetHeight();//height of paper will be constant radius to form pizza shape instead of rectangular for now
        float mass = getMassOfQuarterCirclularPrism(radius, thickness, genes.GetDensity()); //multiply volume by density to get mass
        //providing radius and thickness for dimensions, mass, and num of folds for surface area calculations
        return new PaperAirplanePhysicsAttributes(numFolds, radius, thickness, mass); //return physics calculations
    }*/

    //gets volume of quarter circle (area * thickness) then mutlply by density to get mass
    private float getMassOfQuarterCirclularPrism(float r, float t, float d)
    {
        return (t * Mathf.PI * Mathf.Pow(r, 2) / 4) * d;
    }
}
