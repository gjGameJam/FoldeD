using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


/**
 * leaderboard manager will receive the winner of each round from gamemanager and will evaluate placement (if glider isn't already on leaderboard)
 * 
 * @author Grant Benson
 **/
public class LeaderboardController :  MonoBehaviour 
{
    [SerializeField] private List<TextMeshProUGUI> leaderboardText; //leader board entries 1-5
    //private Dictionary<string, FloatIntPair> leaderBoardNameDataPairs = new Dictionary<string, FloatIntPair>();
    public List<LeaderboardEntry> leaderBoardEntries = new List<LeaderboardEntry>();
    private int maxLeaderBoardSize = 5;

    //struct for the value in map leaderboards where keys are glider names such that no glider repeats on the leaderboard
    public struct LeaderboardEntry
    {
        public string Name;
        public float DistanceTravelled;
        public int GenerationNumber;

        //data to display on each leaderboard entry (currently not using generation number)
        public LeaderboardEntry(string newName, float floatValue, int intValue)
        {
            Name = newName;
            DistanceTravelled = floatValue;
            GenerationNumber = intValue;
        }

    }



    //function to add leaderboard item (if non-duplicate name)
    public void AddToLeaderboard(string gliderName, int generationNumber, float distanceTravelled)
    {
        // check if the player's name already exists
        foreach (var entry in leaderBoardEntries)
        {
            if (entry.Name == gliderName)
            {
                Debug.Log($"Player {gliderName} is already in the leaderboard.");
                return; // Prevent adding duplicate
            }
        }

        //create struct for new results
        LeaderboardEntry newResults = new LeaderboardEntry(gliderName, distanceTravelled, generationNumber);
        //add new entry
        leaderBoardEntries.Add(newResults);
        // Sort by DistanceTravelled in descending order such that highest score is on top
        leaderBoardEntries.Sort((a, b) => b.DistanceTravelled.CompareTo(a.DistanceTravelled));

        // trim to maxLeaderBoardSize
        if (leaderBoardEntries.Count > maxLeaderBoardSize)
        {
            //remopve last leaderboard entry due to new addition above
            leaderBoardEntries.RemoveAt(leaderBoardEntries.Count - 1);
        }

        //update leaderboard user interface
        UpdateLeaderboardUI();
    }

    //function to use the leaderBoardEntries list to update leaderboardText list
    private void UpdateLeaderboardUI()
    {
        //loop through each entry, updating contents of leaderboardText with entryData
        int index = 0;
        foreach (LeaderboardEntry entryData in leaderBoardEntries)
        {
            //use custom toString method to use as text, incrementing index to edit each loop
            leaderboardText[index++].text = $"{entryData.Name} (Gen#{entryData.GenerationNumber}) flew {entryData.DistanceTravelled:F3} Meters";
        }
    }

    }
