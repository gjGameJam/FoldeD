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
        //variables that are received and dont change
        private float radius;
        private float thickness;
        private int numberOfFolds;
        //sent variables
        private float middleArea; //the middle part of the paper airplane that you hold
        private float frontArea; //the part that goes into the wind/air
        private float topArea;  //the part that is visible from the top down view
        private float mass;  //weight of paper airplane (Kg)
        private bool canFly; //ability to fly or not
        private Vector3 COM; //center of mass

        //physical attributes of a wing+middle part (half of a plane)
        public PaperAirplanePhysicsAttributes(int numFolds, float radius, float thickness, float density)
        {
            //initialize number of folds, radius, thickness, and calculate mass via volume and density from paper type
            this.numberOfFolds = numFolds;
            this.radius = radius;
            this.thickness = thickness; //this is the thickness of the paper itself, not the entire glider (which is usually multiple folds thick)
            this.mass = -1; //need to calculate mass with volume and density
            this.middleArea = -1;
            this.frontArea = -1;
            this.topArea = -1;
            this.canFly = false;
            middleArea = calculateMiddleArea();
            topArea = calculateTopArea();
            frontArea = calculateFrontArea();
            mass = getMassOfQuarterCirclularPrism(radius, thickness, density);//calculate as quarter of small piece of circular prism
            this.COM = calculateCenterOfMass(); //calculate Center of mass (COM) that can be retrieved via getter later
            this.canFly = true;//allow glider flight only after calculations are complete
        }

        //calculates the middle area of one side of plane (there will be two)
        //since the paper is oriented up (such that the middle SA is largest on 0 folds) no base case exists
        private float calculateMiddleArea()
        {
            //TODO: potentially consider interaction of fold connecting wings?
            float quarterCircleArea = getSAofQuarterCircle(radius); // pi * r^2 / 4 is quarter circle
            float fractionOfAreaDueToFolds = getHalvingOfPaperFromFolds(); //(.5) ^ (# of folds) because the area will get halved every time with the pizza (quarter circular sector) model
            return quarterCircleArea * fractionOfAreaDueToFolds; //quarter circle * fractionOfAreaDueToFolds will be the middle area (held as thrown) given number of folds
        }

        //calculates the wing area of one side of plane (there will be two)
        private float calculateTopArea()
        {
            //wings do not exist if there have been no folds yet
            switch (numberOfFolds)
            {
                //the wing section won't be equal to the middle section only when folds = 0
                case 0:
                    return thickness * radius; //if there are no folds the wing area will be the side of the paper (thickness * length of paper which will be radius here) pointing up
                default:
                    return getMiddleArea(); //top of wing and bottom of wing (the body) will always be symmetrical (assuming at least one fold has been made)
            }
        }

        //calculates surface area for one side of front of plane using wingspan and thickness
        private float calculateFrontArea()
        {
            switch (numberOfFolds)
            {
                case 0: //if there are no folds the frontal area is thickness * radius (just like top area)
                    //Debug.Log("zero folds!");
                    return thickness * radius; 
                default: //if there are folds, the frontal area is wingspan * thickness * 2 (because front section has two wings worth of air exposure)
                    return getThicknessFolded() * calculateWingspan() * 2; //thickness * wingspan is the area exposed to forward air by one side (there are two)
            }
        }

        //helper function that gets surface area of quarter circle of param radius
        private float getSAofQuarterCircle(float radius)
        {
            //PI * R^2 = circle surface area
            return Mathf.PI * Mathf.Pow(radius, 2) / 4; // circle surface area / 4 = quarter circle surfce area
        }

        // Given a nose angle and radius, calculate the opposite side length
        public float GetOppositeSideLength(float noseAngleRadians, float radius)
        {
            // Use the sine of the nose angle to calculate the opposite side
            float oppositeSide = radius * Mathf.Sin(noseAngleRadians);//goes from 0 at 0 to radius at pi/2 radians
            return oppositeSide; // Return the calculated opposite side length
        }

        //gets the wingspan of one wing given the number of folds and radius
        public float calculateWingspan()
        {
            //if no folds have occured the wing span will be the thickness of the paper
            switch (numberOfFolds)
            {
                case 0: //because paper is upright at folds = 0, the wingspan will just be the paper's thickness
                    return thickness; //return depth of sheet (don't need to divide because there is one each side)
                default:
                    // The opposite side of a triangle formed by the hypotenuse (which will always be the radius for pizza example), nose angle, and right angle will be the wingspan
                    float wingspan = GetOppositeSideLength(getNoseAngleRadians(), radius);
                    return wingspan; //wingspan is the distance from the middle to the wing edge
            }
        }

        //helper function for decay relationship of halving of paper due to folds
        private float getHalvingOfPaperFromFolds(){
            return Mathf.Pow((.5f), numberOfFolds); //.5^(# of folds)
        }

        //nose angle starts at 90 and halves each fold
        private float getNoseAngleDegrees()
        {
            //angle will halve every fold because sides touch and crease becomes new hypotenuse (or side)
            return NOSE_STARTING_ANGLE * getHalvingOfPaperFromFolds();
        }

        //nose angle starts at pi/2 and halves each fold
        private float getNoseAngleRadians()
        {
            //angle will halve every fold because sides touch and crease becomes new hypotenuse (or side)
            return NOSE_STARTING_ANGLE * getHalvingOfPaperFromFolds() * Mathf.Deg2Rad; //same as degree function but multiply by scalar to convert to radians
        }

        //paper folded upon itself will be around 1.7 times the thickness of the original
        //so paper with 0 or 1 folds will just be thickness because first fold forms wing instead of overlapping
        public float getThicknessFolded()
        {
            if (numberOfFolds <= 1)
            {
                return thickness;
            }
            return thickness * Mathf.Pow(THICKNESS_MULTIPLIER_PER_FOLD, numberOfFolds - 1); //thickness of paper increases exponentially per fold after first fold
        }

        //getter for mass
        public readonly float getMass()
        {
            return mass;
        }

        //getter for the surface area of the top of glider
        public readonly float getTopArea()
        {
            return topArea;
        }

        //getter for the surface area of the top of glider
        public readonly float getFrontArea()
        {
            return frontArea;
        }

        //getter for the surface area of the top of glider
        public readonly float getMiddleArea()
        {
            return middleArea;
        }

        //helper function to get mass: gets volume of quarter circle (area * thickness) then mutlply by density to get mass
        private float getMassOfQuarterCirclularPrism(float r, float t, float d)
        {
            return (t * Mathf.PI * Mathf.Pow(r, 2) / 4) * d;
        }

        //function to get center of mass of one side of a glider by creating a triangle with the hypotenuse being the distance to centroid
        private Vector3 getCenterOfMassOfSide(bool rightSide){
            float distFromNoseToCentroid = GetDistanceFromTipToCentroid();//will be used as hypotenuse to calculate position
            float centroidTheta = getNoseAngleRadians() / 2; //the centroid bisects the actual circular sector (it's in the middle) so angle is halved
            //calculate centroid point based on model where back is origin and nose is (radius, 0, 0)
            float forwardOffset = radius - distFromNoseToCentroid * Mathf.Cos(centroidTheta); //1 at 0 radians
            float sideOffset = distFromNoseToCentroid * Mathf.Sin(centroidTheta); //0 at 0 radians
            Vector3 nose = new Vector3(radius, 0, 0); //nose is always radius away from origin
            Vector3 wingCentroidPoint = new Vector3(forwardOffset, 0, sideOffset);
            Vector3 middleCentroidPoint = new Vector3(forwardOffset, -sideOffset, 0); //piece is folded down
            Vector3 noFoldsCentroidPoint = new Vector3(forwardOffset, sideOffset, 0);//piece not folded down
            //right side is negative left side is positive
            if (rightSide){
                wingCentroidPoint.z = -wingCentroidPoint.z;
            }
            //each side will be considered as two circular sectors (the wing and middle) unless folds = 0
            switch (numberOfFolds)
            {
                case 0: 
                    return noFoldsCentroidPoint; //only one large circular sector sticking up
                default: 
                    //two circular sectors connected by a fold that are equal mass
                    return (wingCentroidPoint + middleCentroidPoint) / 2; //calculate average pos then return
            }
        }

        //function to get center of mass assuming a symmetrical glider
        private Vector3 calculateCenterOfMass()
        {
            float distFromNoseToCentroid = GetDistanceFromTipToCentroid();//will be used as hypotenuse to calculate position
            float centroidTheta = getNoseAngleRadians() / 2; //the centroid bisects the actual circular sector (it's in the middle) so angle is halved
            //calculate centroid point based on model where back is origin and nose is (radius, 0, 0)
            float forwardOffset = radius - distFromNoseToCentroid * Mathf.Cos(centroidTheta); //1 at 0 radians
            float sideOffset = distFromNoseToCentroid * Mathf.Sin(centroidTheta); //0 at 0 radians
            Vector3 nose = new Vector3(radius, 0, 0); //nose is always radius away from origin
            Vector3 leftwingCentroidPoint = new Vector3(forwardOffset, 0, sideOffset);
            Vector3 rightwingCentroidPoint = new Vector3(forwardOffset, 0, -sideOffset);
            Vector3 middleCentroidPoint = new Vector3(forwardOffset, -sideOffset, 0); //piece is folded down
            Vector3 noFoldsCentroidPoint = new Vector3(forwardOffset, sideOffset, 0);//piece not folded down
            //each side will be considered as two circular sectors (the wing and middle) unless folds = 0
            switch (numberOfFolds)
            {
                case 0:
                    return noFoldsCentroidPoint; //only one large circular sector sticking up
                default:
                    //two circular sectors connected by a fold that are equal mass
                    return (leftwingCentroidPoint + rightwingCentroidPoint + (2 * middleCentroidPoint)) / 4; //calculate average pos of 4 semicircles 
            }
        }

        //returns the center of mass that was calculated in calculateCenterOfMass (called in constructor)
        public Vector3 getCenterOfMass(){
            return COM;
        }

        //helper function to get distance from the center of circlular sector (tip opposite of curved edge) to centroid
        private float GetDistanceFromTipToCentroid(){
            //4r/3(theta) * sin(theta/2) is the equation for the distance from the center of circlular sector to the centroid
            float theta = getNoseAngleRadians(); // theta in radians
            return (4 * radius / (3f * theta)) * Mathf.Sin(theta / 2f);
        }

        //helper function to get the length of the arc of the circular sector
        //radians = ArcLength / Radius; so rearranging this formula gets us: ArcLength = Radians * radius
        private float getArcLength(){
            return getNoseAngleRadians() * radius;
        }

        //function to allow flight boolean to be flipped
/*        public void AllowFlight()
        {
            canFly = true;
        }*/

        //getter for radius
        public float getRadius()
        {
            return radius;
        }

        //returns if the folding alg is complete and the glider is flight ready
        public bool CanFly(){
            return canFly;
        }

        public override string ToString()
        {
            return $"Paper Airplane Physics Attributes:\n" +
                   $"- Number of Folds: {numberOfFolds}, Radius: {radius}, Thickness: {thickness}, Mass: {mass}, Middle Area: {middleArea}, Front Area: {frontArea}, Top Area: {topArea}";
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

        //todo: replace the usage of this as just radius because it is set then not changed
        // //getter for radius
        // public float getRadius()
        // {
        //     return radius;
        // }

    }
    
}
