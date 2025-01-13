using System.Collections.Generic;
using System;
using UnityEngine;
using NUnit.Framework;
using System.Linq;

/**
 * manager to save best and reproduce children for best competitors via splice method
 * 
 * @author Grant Benson
 **/
public class GeneManger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log("gene manager exists");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //main function of gene manger accepts two gene sequences and specified number of children with spliced genes of parents
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


    //helper function that takes in two float arrays and splices them into a gene sequence
    GeneSequence GetGeneAtSplicePoint(GeneSequence parent1, GeneSequence parent2, int splicePoint)
    {
        // Convert parent1 genes to list
        List<float> list1 = new List<float>(parent1.geneSequence);
        // Convert parent2 genes to list
        List<float> list2 = new List<float>(parent2.geneSequence);
        // remove 0 to splice point from list1
        list1.RemoveRange(0, splicePoint);
        // remove splice point to end of list2
        int fullRange = list2.Count;
        list2.RemoveRange(splicePoint, fullRange - splicePoint);
        // combine arrays
        float[] combined = list1.Concat(list2).ToArray();
        Debug.Log($"P1: {parent1.geneSequence} and P2: {parent2.geneSequence} made {combined}"); //consider making print function for testing
        return new GeneSequence(combined);//creates new gene sequence with spliced array
    }
}
