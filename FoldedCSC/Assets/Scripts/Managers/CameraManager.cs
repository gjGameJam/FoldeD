using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

//manager to follow the leading glider each round; goes back to the start once all gliders land
//script is meant to be attatched to a camera
public class CameraManager : MonoBehaviour
{

    //reference to the game manager, which knows the gliders and starting point
    [SerializeField] private GameManager GManager;
    private Camera cam; //reference to camera
    GameObject currentLeadingGlider; //glider that is being followed (uses camera attatched to glider)


    [Header("Camera timer variables")]
    private float timer = 0f;  // Timer to track time passed between leading glider check
    private readonly float interval = .2f;  // Time interval in seconds for how long to wait between checking leading glider


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame, follows leading glider and tries to get new leading paper glider every interval
    void Update()
    {
        // Increment the timer by the time passed since the last frame
        timer += Time.deltaTime;
        // Check if interval amount of time has passed
        if (timer >= interval)
        {
            // get which glider is in the lead (best fitness score)
            GameObject newLeadGlider = GManager.GetFarthestGlider();
            //if null round over go to start
            if (newLeadGlider == null){
                Debug.Log("no gliders for camera to follow");
            }
            if (currentLeadingGlider != newLeadGlider)
            {
                // if the lead glider is different from current one being followed, change camera being used
                //consider making camera switch function that disables camera in order to not render (improve performance)
                cam = newLeadGlider.GetComponent<Camera>();//get camera of newLeadGlider could be .GetComponentInChildren<Camera>();
            }

            // Reset the timer to reuse
            timer = 0f;
        
        }
        else
        {
            //keep tracking lead glider
        }

    }

    
}
