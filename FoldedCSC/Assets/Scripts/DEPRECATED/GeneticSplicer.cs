using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static GeneticSplicer;

public class GeneticSplicer : MonoBehaviour
{
    public enum PaperType
    {
        Letter,  // Represents standard copy paper
        Cardstock, // Represents cardstock paper
        Newsprint,   // Represents news printed paper
        Photopaper    //Represents paper a photo would be printed onto
    }

    private const float DENSITY_OF_LETTER_PAPER = 800; //800kg/m^3
    private const float DENSITY_OF_CARDSTOCK_PAPER = 1100; //1000kg/m^3
    private const float DENSITY_OF_NEWSPRINT_PAPER = 500; //500kg/m^3
    private const float DENSITY_OF_PHOTOPAPER_PAPER = 1000; //1000kg/m^3
    public struct PaperAirplaneGeneSequence
    {

        private int numberOfFolds;
        private float initialVelocity;
        private float paperLength;
        private float paperHeight;
        private float paperWidth;
        private float paperDensity;
        private int numberOfGenes;
        public float[] geneSequence;

        //constructor
        public PaperAirplaneGeneSequence(int numFolds, float initVelocity, float pLength, float pWidth, float pHeigh, PaperType paperType)
        {
            this.numberOfFolds = numFolds;
            this.initialVelocity = initVelocity;
            this.paperLength = pLength;
            this.paperWidth = pWidth;
            this.paperHeight = pHeigh;
            this.numberOfGenes = 0;
            //init then set
            this.paperDensity = -1;
            geneSequence = new float[6]; //TODO: update this when possible to automatically update size with geneome length
            geneSequence[0] = numberOfFolds;
            geneSequence[1] = initialVelocity;
            geneSequence[2] = paperLength;
            geneSequence[3] = paperWidth;
            geneSequence[4] = paperHeight;
            geneSequence[5] = (float)paperType;  // Convert enum to float
            this.paperDensity = GetDensityOfPaperType(paperType);
            this.numberOfGenes = 6;
            //string methodName = nameof(PaperAirplaneGeneSequence); // Get the constructor name
            //this.numberOfGenes = GetParameterCount(methodName);


        }

        // Parameterless Constructors are in C#10+
        public PaperAirplaneGeneSequence(PaperType paperType)
        {
            this.numberOfFolds = 0;
            this.initialVelocity = 5.0f;
            this.paperLength = 0.2794f; // m (11 inches)
            this.paperWidth = 0.2159f;//m (8.5 inches)
            this.paperHeight = 0.0001f;// m(0.1 mm)
            this.paperDensity = -1f;
            this.numberOfGenes = 6;
            // Initialize an empty gene sequence
            geneSequence = new float[numberOfGenes];

            geneSequence[0] = numberOfFolds;
            geneSequence[1] = initialVelocity;
            geneSequence[2] = paperLength;
            geneSequence[3] = paperWidth;
            geneSequence[4] = paperHeight;
            geneSequence[5] = (float)paperType;  // Convert enum to float}

            this.paperDensity = GetDensityOfPaperType(paperType);
        }

        // Method to get the number of parameters for a specific method by name
        int GetParameterCount(string methodName)
        {
            // Get the type of the current class
            var type = this.GetType();

            // Get the method information for the specified method
            MethodInfo method = type.GetMethod(methodName);

            // Check if the method exists
            if (method != null)
            {
                // Return the number of parameters
                return method.GetParameters().Length;
            }
            else
            {
                Debug.LogError($"Method '{methodName}' not found.");
                return -1; // Return -1 to indicate the method was not found
            }
        }

        public readonly int GetNumFolds()
        {
            return numberOfFolds;
        }

        public readonly float GetHeight()
        {
            return paperHeight;
        }

        public readonly float GetLength()
        {
            return paperLength;
        }

        public readonly float GetWidth()
        {
            return paperWidth;
        }

        public readonly float GetDensity()
        {
            return paperDensity;
        }

        public readonly int GetNumGenes()
        {
            return numberOfGenes;
        }

        public float getVolume()
        {
            return paperLength * paperHeight * paperWidth;
        }

        public float getMass()
        {
            return getVolume() * paperDensity;
        }

        public float GetDensityOfPaperType(PaperType paperType)
        {
            switch (paperType)
            {
                case PaperType.Letter:
                    return DENSITY_OF_LETTER_PAPER;
                case PaperType.Cardstock:
                    return DENSITY_OF_CARDSTOCK_PAPER;
                case PaperType.Newsprint:
                    return DENSITY_OF_NEWSPRINT_PAPER;
                case PaperType.Photopaper:
                    return DENSITY_OF_PHOTOPAPER_PAPER;
                default:
                    Debug.Log("Unknown paper type selected.");
                    break;
            }

            return -1;//-1 to propogate error
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public PaperAirplaneGeneSequence[] GeneratePaperAirplaneOffspring(float[] parent1, float[] parent2, int numChildren)
    {
        //create array to be populated with children
        PaperAirplaneGeneSequence[] returnPlanes = new PaperAirplaneGeneSequence[numChildren];
        int numGenes = parent1.Length;
        //for each child
        for (int i = 0; i < numChildren; i++)
        {
            // Create a new gene sequence for the child
            float[] childGeneSequence = new float[numGenes];

            // Calculate the splice point based on the child index (linear interpolation)
            int splicePoint = (i * numGenes) / numChildren;  // Moves from 0 to numGenes

            for (int j = 0; j < numGenes; j++)
            {
                // Copy genes from p1 or p2 based on the splice point
                if (j < splicePoint)
                {
                    childGeneSequence[j] = parent1[j];  // Take from parent 1
                }
                else
                {
                    childGeneSequence[j] = parent2[j];  // Take from parent 2
                }
            }

            // Create the child airplane using the new gene sequence
            returnPlanes[i] = new PaperAirplaneGeneSequence(
                (int)childGeneSequence[0],  // numFolds
                childGeneSequence[1],       // initVelocity
                childGeneSequence[2],       // paperLength
                childGeneSequence[3],       // paperWidth
                childGeneSequence[4],       // paperHeight
                (PaperType)(int)childGeneSequence[5]  // paperType (convert back to enum)
            );
        }

        //splicing complete
        return returnPlanes;


    }
}
