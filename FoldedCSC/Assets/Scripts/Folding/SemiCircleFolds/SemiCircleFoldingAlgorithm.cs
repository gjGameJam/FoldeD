using UnityEngine;

/**
 * Algorithm to get surface areas of paper airplane such that the piece of paper is a half circle and each wing is a symmetrical quarter circle
 * given a radius and number of folds, provide surface area of each face making contact with air.
 * 
 * @author Grant Benson
 **/
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
            density: 0.2f,
            canFly: false
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
        private bool canFly;
        //sent variables
        private float middleArea; //the middle part of the paper airplane that you hold
        private float frontArea; //the part that goes into the wind/air
        private float topArea;  //the part that is visible from the top down view
        private float mass;  //weight of paper airplane (Kg)

        //physical attributes of a wing+middle part (half of a plane)
        public PaperAirplanePhysicsAttributes(int numFolds, float radius, float thickness, float density, bool canFly)
        {
            //initialize number of folds, radius, thickness, and calculate mass via volume and density from paper type
            this.numberOfFoldds = numFolds;
            this.radius = radius;
            this.thickness = thickness;
            this.mass = density; //need to calculate mass with volume and density
            this.middleArea = -1;
            this.frontArea = -1;
            this.topArea = -1;
            this.canFly = false;
            middleArea = getMiddleArea();
            topArea = getTopArea();
            frontArea = getFrontArea();
            mass = getMassOfQuarterCirclularPrism(radius, thickness, density);//calculate as quarter of small piece of circular prism
            canFly = true;//allow glider flight only after calculations are complete
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
        public float calculateWingspan()
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
        //TODO: change function name to getNoseAngleDegrees
        private float getNoseAngle()
        {
            //angle will halve every fold because sides touch and crease becomes new hypotenuse (or side)
            return NOSE_STARTING_ANGLE * Mathf.Pow((.5f), numberOfFoldds);
        }

        private float getNoseAngleRadians()
        {
            //angle will halve every fold because sides touch and crease becomes new hypotenuse (or side)
            float noseAngleInDegrees = getNoseAngle();
            return noseAngleInDegrees * Mathf.Deg2Rad; //multiply by scalar to convert to radians and return result
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
        public float getThicknessFolded()
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

        //helper function to get mass: gets volume of quarter circle (area * thickness) then mutlply by density to get mass
        private float getMassOfQuarterCirclularPrism(float r, float t, float d)
        {
            return (t * Mathf.PI * Mathf.Pow(r, 2) / 4) * d;
        }

        // //helper function to get mass of a circular sector
        // private float getMassOfCircularSector(){
        //     //since we have already calculated mass via the quarter of circular prism function on start
        //     //we can merely check if a fold has been performed and divide into two sections per side if so
        //     if (numberOfFoldds == 0){
        //         return mass;
        //     }
        //     return mass / 2; //now there will be two sections per side
        // }


        //TODO: go over sin and cos functions and ensure radians are being used
        //function to get center of mass of one side of a glider by creating a triangle with the hypotenuse being the distance to centroid
        private Vector3 getCenterOfMassOfSide(bool rightSide){
            float distFromNoseToCentroid = GetDistanceFromTipToCentroid();//will be used as hypotenuse to calculate position
            float centroidTriangleRadians = getNoseAngleRadians() / 2; //the centroid bisects the actual circular sector (it's in the middle) so angle is halved
            Vector3 nose = new Vector3(getRadius(), 0, 0); //nose is always radius away from origin
            //calculate centroid point based on model where back is origin and nose is (radius, 0, 0)
            float forwardOffset = getRadius() - distFromNoseToCentroid * Mathf.Cos(centroidTriangleRadians); //1 at 0 radians
            float sideOffset = distFromNoseToCentroid * Mathf.Sin(centroidTriangleRadians); //0 at 0 radians
            Vector3 wingCentroidPoint = new Vector3(xOffset, 0, sideOffset);
            Vector3 middleCentroidPoint = new Vector3(xOffset, -sideOffset, 0); //piece is folded down
            Vector3 noFoldsCentroidPoint = new Vector3(xOffset, sideOffset, 0);//piece not folded down
            //right side is negative left side is positive
            if (rightSide){
                wingCentroidPoint.z = -wingCentroidPoint.z;
            }
            
            //each side will be considered as two circular sectors (the wing and middle) unless folds = 0
            if (numberOfFoldds == 0){
                //only one large circular sector sticking up
                return mass * noFoldsCentroidPoint;
            }
            else{
                //two circular sectors connected by a fold
                return mass * ((wingCentroidPoint + middleCentroidPoint) / 2);
            }
        }

        //helper function to get distance from the center of circlular sector (tip opposite of curved edge) to centroid
        private float GetDistanceFromTipToCentroid(){
            //4r/3(theta) * sin(theta/2) is the equation for the distance from the center of circlular sector to the centroid
            return (4 * getRadius() / 3f) * Mathf.Sin(getNoseAngleRadians() / 2);
        }

        //helper function to get the length of the arc of the circular sector
        private float getArcLength(){
            //radians = ArcLength / Radius; so rearranging this formula gets us:
            //ArcLength = Radians * radius
            return getNoseAngleRadians() * getRadius();
        }

        //getter for radius
        public float getRadius()
        {
            return radius;
        }

        //function to allow flight boolean to be flipped
        public void AllowFlight()
        {
            canFly = true;
        }
    }

    
}
