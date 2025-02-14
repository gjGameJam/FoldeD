using UnityEngine;
using System.Collections.Generic;
using System;

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
    private float initialAngle; //angle in which the glider will be thrown (in degrees)
    public float[] geneSequence;//array to store/splice all gene values

    private const uint FNV1A_OFFSET_BASIS = 2166136261;  // FNV-1a 32-bit offset basis
    private const uint FNV1A_FNV_32_PRIME = 16777619;  // FNV-1a prime

    // Constructor
    public GeneSequence(int numberOfFolds, float initialVelocity, float paperDensity, float paperLength, float paperHeight, float paperWidth, float initialAngle)
    {
        this.numberOfFolds = numberOfFolds;
        this.initialVelocity = initialVelocity;
        this.paperDensity = paperDensity;
        this.paperLength = paperLength;
        this.paperHeight = paperHeight;
        this.paperWidth = paperWidth;
        this.initialAngle = initialAngle;
        this.geneSequence = new float[7] { numberOfFolds, initialVelocity, paperDensity, paperLength, paperHeight, paperWidth, initialAngle};//stores # folds, Vinit, density, length, height, and width
    }

    // Constructor with only geneSequence, copies data to fields (used in gene manager for splicing)
    public GeneSequence(float[] geneSequence)
    {
        this.geneSequence = geneSequence;
        //numberOfFolds, initialVelocity, paperDensity, paperLength, paperHeight, paperWidth, initialAngle
        this.numberOfFolds = (int)geneSequence[0];
        this.initialVelocity = geneSequence[1];
        this.paperDensity = geneSequence[2];
        this.paperLength = geneSequence[3];
        this.paperHeight = geneSequence[4];
        this.paperWidth = geneSequence[5];
        this.initialAngle = geneSequence[6];
    }

    // Clone method to create deep copy of object (not shared)
    public GeneSequence Clone()
    {
        // instantiate new object with same values
        GeneSequence clone = new GeneSequence(
            this.numberOfFolds,
            this.initialVelocity,
            this.paperDensity,
            this.paperLength,
            this.paperHeight,
            this.paperWidth,
            this.initialAngle
        );

        // deep copy the geneSequence array to ensure it's not shared
        clone.geneSequence = (float[])this.geneSequence.Clone();
        //then return copy
        return clone;
    }

    //data storing color, animals, and verbs for hash val conversion to name
    private static readonly string[] colors = { 
        "Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Black", "White", "Cyan", "Magenta", 
        "Crimson", "Teal", "Lime", "Azure", "Maroon", "Gold", "Silver", "Bronze", "Turquoise", "Olive", 
        "Violet", "Amber", "Indigo", "Lavender", "Ruby", "Emerald", "Sapphire", "Rose", "Pearl", "Coral", 
        "Ivory", "Beige", "Navy", "Charcoal", "Mint", "Fuchsia", "Salmon", "Burgundy", "Lilac", "Mustard"
    };
    private static readonly string[] animals = { 
        "Falcon", "Tiger", "Wolf", "Eagle", "Shark", "Panther", "Cobra", "Fox", "Hawk", "Lynx", 
        "Leopard", "Jaguar", "Viper", "Bison", "Ocelot", "Griffon", "Hyena", "Raven", "Stallion", "Bull", 
        "Cheetah", "Cougar", "Dragon", "Hound", "Kraken", "Lizard", "Mongoose", "Orca", "Puma", "Scorpion", 
        "Tarantula", "Wolverine", "Coyote", "Gazelle", "Jackal", "Condor", "Pelican", "Barracuda", "Bobcat", "Mastiff"
    };
    private static readonly string[] verbs = { 
        "Soaring", "Roaring", "Gliding", "Hunting", "Striking", "Leaping", "Charging", "Diving", "Prowling", 
        "Sprinting", "Creeping", "Surging", "Crashing", "Lunging", "Slashing", "Pouncing", "Swooping", "Darting", "Galloping", 
        "Flanking", "Bolting", "Bounding", "Dashing", "Swarming", "Slithering", "Coiling", "Stampeding", "Prowling", "Snapping", 
        "Hovering", "Swooshing", "Vaulting", "Weaving", "Ambushing", "Scouting", "Circling", "Thrashing", "Snarling", "Racing"
    };
    
    // //function to get random name based on hashed gene sequence
    // public string getName()
    // {
    //     int hashVal = Mathf.Abs(getHashVal()); // convert hash to positive int

    //     string color = colors[hashVal % colors.Length];
    //     string animal = animals[(hashVal / colors.Length) % animals.Length];
    //     string verb = verbs[(hashVal / (colors.Length * animals.Length)) % verbs.Length];
    //     //concat the color, animal, and verb
    //     return $"{color}{animal}{verb}";
    //     //return "timothyGene";
    // }

    //function to get random name based on hashed gene sequence
    public string getName()
    {
        uint hashVal = getHashVal(); //get positive hash val and 
        //hash to get uints then mod to ensure no negatives and valid items are selected from arrays
        uint colorHash = Fnv1aHash(BitConverter.GetBytes(hashVal));
        uint animalHash = Fnv1aHash(BitConverter.GetBytes(hashVal + 1));
        uint verbHash = Fnv1aHash(BitConverter.GetBytes(hashVal + 2));
        //Debug.Log($"Colors: {colorHash}, Animals: {animalHash}, Verbs: {verbHash}");
        string color = colors[colorHash % colors.Length];
        string animal = animals[animalHash % animals.Length];
        string verb = verbs[verbHash % verbs.Length];
        //concat the color, ver, and animal
        return $"{color}{verb}{animal}";
    }

    //hash function for converting gene sequence array via Fnv1aHash to uint
    private uint getHashVal()
    {
        byte[] byteArray = FloatArrayToBytes(geneSequence);
        return Fnv1aHash(byteArray);
    }

    //helper function to convert floats to bytes
    private byte[] FloatArrayToBytes(float[] floatArray)
    {
        byte[] bytes = new byte[floatArray.Length * sizeof(float)];
        Buffer.BlockCopy(floatArray, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    //Fnv1aHash hash of byte array to signed int 
    private uint Fnv1aHash(byte[] data)
    {
        uint hash = FNV1A_OFFSET_BASIS; //start with offset
        foreach (byte b in data)
        {
            hash ^= b; // XOR with byte
            hash *= FNV1A_FNV_32_PRIME; // then multiply by prime
        }
        return hash;
    }

    public float GetInitialAngle(){
        return initialAngle;
    }

    public int GetNumberOfFolds()
    {
        return (int)numberOfFolds;
    }

    public float GetInitialVelocity()
    {
        return initialVelocity;
    }

    public float GetPaperDensity()
    {
        return paperDensity;
    }

    public float GetPaperLength()
    {
        return paperLength;
    }

    public float GetPaperHeight()
    {
        return paperHeight;
    }

    public float GetPaperWidth()
    {
        return paperWidth;
    }


}
