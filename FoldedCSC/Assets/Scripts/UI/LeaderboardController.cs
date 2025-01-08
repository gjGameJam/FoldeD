using System.Collections.Generic;
using TMPro;
using UnityEngine;


/**
 * leaderboard manager will receive the winner of each round from gamemanager and will evaluate placement (if glider isn't already on leaderboard)
 * 
 */
public class LeaderboardController// : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> leaderboardEntries; //leader board entries 1-5



    //function to update leaderboard item (if necessary)
    void UpdateLeaderboard(string gliderName, float distanceTravelled)
    {

    }





/*    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
