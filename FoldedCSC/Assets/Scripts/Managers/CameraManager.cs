using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


/**
 * manager to follow the leading glider each round via lerping to desired position (offset + target pos)
 * 
 * @author Grant Benson
 **/
public class CameraManager : MonoBehaviour
{
    private GameObject currentLeadingGlider; //furthest glider in simulation to follow with camera

    [Header("Camera Settings")]
    private Vector3 offset = new Vector3(10, -5, -20);  // The desired offset from the glider
    private float smoothSpeed = 2f;  // lerp speed
    private float rotationSpeed = 5f; // rotation speed

    //follows lead glider based on largest x val
    void LateUpdate()
    {
        // Find the new lead glider
        GameObject newLeadGlider = FindLeadGlider();
        if (newLeadGlider != null)
        {
            currentLeadingGlider = newLeadGlider;
        }

        if (currentLeadingGlider != null)
        {
            // apply the offset in world space using TransformPoint, which takes into account any rotation/transformations
            Vector3 desiredPosition = currentLeadingGlider.transform.TransformPoint(offset);

            // lerp from current position ot desiredPosition
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            //Debug.Log($"Camera Position: {transform.position}, Lead Glider Position: {currentLeadingGlider.transform.position}, Offset: {offset}");
            //rotate the camera to look at the lead glider's position
            //Vector3 direction = currentLeadingGlider.transform.position - transform.position;
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }
    }

    // find the lead glider based on the highest x val
    GameObject FindLeadGlider()
    {
        GameObject[] gliders = GameObject.FindGameObjectsWithTag("Glider"); //searches through all gameobjects with glider tag (can have various types)
        GameObject leadGlider = null;
        float maxX = float.MinValue;
        //loop through each, checking for null because they could have crashed
        foreach (GameObject glider in gliders)
        {
            if (glider != null)
            {
                float xDist = glider.transform.position.x;
                if (xDist > maxX)
                {
                    maxX = xDist;
                    leadGlider = glider;
                }
            }
        }
        return leadGlider;
    }
}
