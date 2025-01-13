using UnityEngine;

/**
 * each glider will have a gene sequence that stores relevant information for the gene manager to use
 * avoiding using monobehavior to optimize performance (won't need start/update/etc)
 * 
 * @author Grant Benson
 **/
public class GeneSequence
{
    private int numberOfFolds; //# of folds that paper will undergo
    private float initialVelocity; //initial launch velocity of glider (5 - 15 m/s for human throwing)
    private float paperDensity; //density of paper in kg/m^3
    private float paperLength; //measurement for how long piece of paper is (x length in meters)
    private float paperHeight; //measurement for how tall piece of paper is (y length in meters)
    private float paperWidth; //measurement for how thick piece of paper is (z length in meters)
    public float[] geneSequence;//array to store/splice all gene values

    // Constructor
    public GeneSequence(int numberOfFolds, float initialVelocity, float paperDensity, float paperLength, float paperHeight, float paperWidth)
    {
        this.numberOfFolds = numberOfFolds;
        this.initialVelocity = initialVelocity;
        this.paperDensity = paperDensity;
        this.paperLength = paperLength;
        this.paperHeight = paperHeight;
        this.paperWidth = paperWidth;
        this.geneSequence = new float[6] { numberOfFolds, initialVelocity, paperDensity, paperLength, paperHeight, paperWidth };//stores # folds, Vinit, density, length, height, and width
    }

    // Constructor with only geneSequence, copies data to fields (used in gene manager for splicing)
    public GeneSequence(float[] geneSequence)
    {
        this.geneSequence = geneSequence;
        this.numberOfFolds = (int)geneSequence[0];
        this.initialVelocity = geneSequence[1];
        this.paperDensity = geneSequence[2];
        this.paperLength = geneSequence[3];
        this.paperHeight = geneSequence[4];
        this.paperWidth = geneSequence[5];
    }

}
