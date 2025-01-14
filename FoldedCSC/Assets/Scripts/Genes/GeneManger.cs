using System.Collections.Generic;
using System;
using UnityEngine;
using NUnit.Framework;
using System.Linq;
using UnityEngine.SceneManagement;

/**
 * manager to save best and reproduce children for best competitors via splice method
 * 
 * @author Grant Benson
 **/
public class GeneManger : MonoBehaviour
{

    private int maxNumberOfFolds = 8;
    private float mutationChance = .5f;
    private float minThrowSpeed = 5; // m/s
    private float maxThrowSpeed = 15; // m/s
    private float minDensity = 600; // kg/m^3
    private float maxDensity = 1100; // kg/m^3
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log("gene manager exists");
        // Initialize two parent gene sequences
        //testing found bug in get gene at splice and fix resolved problem
        float[] parent1Genes = { 3, 12.5f, 800f, 0.3f, 0.2f, 0.001f }; // Example values for Parent 1
        float[] parent2Genes = { 2, 10.0f, 750f, 0.28f, 0.18f, 0.0012f }; // Example values for Parent 2

        GeneSequence parent1 = new GeneSequence(parent1Genes);
        GeneSequence parent2 = new GeneSequence(parent2Genes);

        // Create an instance of the gene manager and generate offspring
        GeneSequence[] offspring = GeneratePaperAirplaneOffspring(parent1, parent2, 5); // Generate 5 offspring

        // Print out the results
        for (int i = 0; i < offspring.Length; i++)
        {
            Debug.Log($"Offspring {i + 1}:");
            Debug.Log($"  Number of Folds: {offspring[i].GetNumberOfFolds()}");
            Debug.Log($"  Initial Velocity: {offspring[i].GetInitialVelocity()}");
            Debug.Log($"  Paper Density: {offspring[i].GetPaperDensity()}");
            Debug.Log($"  Paper Length: {offspring[i].GetPaperLength()}");
            Debug.Log($"  Paper Height: {offspring[i].GetPaperHeight()}");
            Debug.Log($"  Paper Width: {offspring[i].GetPaperWidth()}");
            //Console.WriteLine();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //main function of gene manger accepts two gene sequences and specified number of children with spliced genes of parents (keeps parent 1 in next generation to ensure no loss)
    public GeneSequence[] GeneratePaperAirplaneOffspring(GeneSequence parent1, GeneSequence parent2, int numChildren)
    {
        GeneSequence[] returnPlanes = new GeneSequence[numChildren]; //create array to return
        returnPlanes[0] = parent1;//add top competitor back to ensure no loss in fitness score between rounds/generations
        //loop from 1 (already added best competitor) to desired number of children
        for (int i = 1; i < numChildren; i++)
        {
            //calculates and sets gene sequence at index (uses i as splice index too)
            returnPlanes[i] = GetGeneAtSplicePoint(parent1, parent2, i);
        }

        return returnPlanes; //returns array of gene sequences to be used in glider instantiation
    }

    //helper function to get gene sequence given two sequences and a splice point
    GeneSequence GetGeneAtSplicePoint(GeneSequence parent1, GeneSequence parent2, int splicePoint)
    {
        // convert parent1 and parent2 gene arrays to lists for easier manipulation
        float[] parent1Genes = parent1.geneSequence;
        float[] parent2Genes = parent2.geneSequence;

        // validate splice points
        if (splicePoint < 0 || splicePoint > parent1Genes.Length || splicePoint > parent2Genes.Length)
        {
            //Debug.LogError($"Invalid splice point: {splicePoint}");
            return null;
        }

        // create a new array for the spliced genes
        float[] combined = new float[parent1Genes.Length];

        // copy the first part from parent1 up to the splice point
        Array.Copy(parent1Genes, 0, combined, 0, splicePoint);

        // copy the remaining part from parent2 starting at the splice point
        Array.Copy(parent2Genes, splicePoint, combined, splicePoint, parent2Genes.Length - splicePoint);

        // mutate combined array before converting into gene sequence
        return new GeneSequence(MutateArray(combined));
    }

    //helper function to mutate gene float array before initialization as gene sequence
    float[] MutateArray(float[] nonMutated)
    {
        //init mutated array
        float[] mutated = new float[nonMutated.Length];
        //handle number of folds separately because it is an int
        if (CanMutate())
        {
            //flip a coin on either going up or down one fold
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= .5f)
            {
                if (nonMutated[0] - 1 >= 0)
                {
                    mutated[0] = nonMutated[0] - 1; //decrement if result will be 0 or greater
                }
                
            }
            else
            {
                mutated[0] = Clamp(nonMutated[0] + 1, 0, maxNumberOfFolds);
            }
            
        }
        else
        {
            mutated[0] = nonMutated[0];
        }

        //loop over non mutated array
        for (int i = 1; i < nonMutated.Length; i++)
        {
            float toBeAdded = nonMutated[i];
            if (CanMutate())
            {
                toBeAdded = toBeAdded * UnityEngine.Random.Range(.9f, 1.1f);
                if (i  == 1) //clamp initial velocity to numbers that a human throw could realistically achieve
                {
                    
                    toBeAdded = Clamp(toBeAdded, minThrowSpeed, maxThrowSpeed);
                }
                else if (i == 2) //clamp paper density to achievable range of everyday pulp based papers
                {
                    toBeAdded = Clamp(toBeAdded, minDensity, maxDensity);
                }
                //consider clamping paper dimensions to achievable bounds
            }

            //add to mutated array regardless if mutation took place
            mutated[i] = toBeAdded;
        }

        return mutated;
    }

    //helper clamp function
    private float Clamp(float value, float min, float max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    //returns true if random number is less than mutation chance
    private bool CanMutate()
    {
        return UnityEngine.Random.Range(0.01f, 1.0f) <= mutationChance;
    }
}
