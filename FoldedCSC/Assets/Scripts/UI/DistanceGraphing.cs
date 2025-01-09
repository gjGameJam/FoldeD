using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DistanceGraphing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //referenced: https://www.youtube.com/watch?v=CmU5-v-v1Qo to learn about line renderers
    // I plan to have this graph as a collection of points (being the furthest distance in each round)
    // and connect each point at the end of each round once the data is available

    private RectTransform graphContainer;
    [SerializeField] private Sprite dataPointSprite; //sprite for data point (consider using paper airplane)
    private float dataPointSize = 25.0f;
    int roundNumber = 0; //int to keep track of x position on graph (updates each time data point is added)
                         //get max height and width of graph
    float graphHeight, graphWidth, maxFlightDistance; //keep track of graph's size in order to rescale data points appropriately

    float dataPointOffsetPerRound = 50.0f;
    float initialDataPointXOffset = 10.0f;
    float initialDataPointYOffset = 7.5f;

    //List<float> testList = new List<float>() { 0.0f, 3.3f, 3.5f, 2.5f, 4.322f, 10.79f, 50.4f, 99.1f, 99.9f };
    List<float> testList = new List<float>();

    void Start()
    {
        graphContainer = GetComponent<RectTransform>();
        //get max height and width of graph
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        Debug.Log($"graph height: {graphHeight}");
        Debug.Log($"graph width: {graphWidth}");

        maxFlightDistance = 100; //max distance at 100 meters for graph
        RenderDataPoints(testList);
    }

    /*    private void Awake()
        {

        }*/

    //deletes all children from the distance graph
    void DeleteAllChildren()
    {
        foreach (Transform child in graphContainer)
        {
            Destroy(child.gameObject);  // Destroys the child gameObject
        }
    }

    

    //adds value to data set and rerenders graph
    public void AddToGraph(float val)
    {
        //adds new value to list
        testList.Add(val);
        Debug.Log("Item added. List size: " + testList.Count);
        //removes current data points and connections on graph
        DeleteAllChildren();
        //renders data points and connections of list with new addition
        RenderDataPoints(testList);
    }

    //function for adding a data point to the graph
    GameObject AddDataPoint(int index, float distance)
    {
        //add data point to graph
        Vector2 AnchoredPosition = GetPlacementForDatapoint(index, distance);
        //create new data point object
        GameObject gameObject = new GameObject("circle", typeof(Image));
        //add data point as child of distance graph
        gameObject.transform.SetParent(graphContainer, false);
        //update sprite
        gameObject.GetComponent<Image>().sprite = dataPointSprite;
        //get rect transform of data point
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        //set data point transform to calculated position
        rectTransform.anchoredPosition = AnchoredPosition;
        //scale up and anchor to bottom corner
        rectTransform.sizeDelta = new Vector2(dataPointSize, dataPointSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        //and create line from previous data point to connect them

        return gameObject;
    }


    //calculates where to put data point
    Vector2 GetPlacementForDatapoint(int index, float distance)
    {
        //add initial offsets and space according to other data on x axis, y axis is height travelled
        Vector2 newVec = new Vector2(initialDataPointXOffset + (index * getDataPointSpacingByRound()), distance + initialDataPointYOffset);

        return newVec;
    }

    float getDataPointSpacingByRound()
    {
        //if there are less than 10 just use the basic offset
        if (testList.Count <= 10)
        {
            return graphContainer.sizeDelta.x / 10;
        }
        else
        {
            Debug.Log($"graph width: {graphWidth}");
            //if there are a lot of data points compress the space to the width / number of data points
            return graphContainer.sizeDelta.x / testList.Count;
        }
    }

    //renders all data points (consider skipping current data point render if next is the same)
    private void RenderDataPoints(List<float> values)
    {
        //want to connect each data point (assuming it has a previous data point)
        GameObject previousDataPoint = null;
        for (int i = 0; i < values.Count; i++)
        {
            //float xPos = i * xOffsetPerPoint;
            float yPos = (values[i] / maxFlightDistance) * 200; //divide flight by max distance and scale up by scalar
            GameObject dataPoint = AddDataPoint(i, yPos);
            //with new data point, connect it with the previous data pont (if it exists)
            if (previousDataPoint != null)
            {
                //last data point exists, create connection
                createDataConnection(previousDataPoint.GetComponent<RectTransform>().anchoredPosition, dataPoint.GetComponent<RectTransform>().anchoredPosition);
            }
            //and update previous data to current data for next loop
            previousDataPoint = dataPoint;
        }
        
    }

    //connects two data points
    private void createDataConnection(Vector2 PosA, Vector2 PosB)
    {
        GameObject gameObject = new GameObject("dataConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f); //white with half transparency
        Vector2 dir = (PosB - PosA).normalized; //get unit vector from point a to point b
        float distance = Vector2.Distance(PosA, PosB); //get distance between points
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        //anchor to first data point and scale up to rectangle
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(distance, 3f); //span entire distance at width of 3
        rect.anchoredPosition = PosA + dir * distance * .5f; //place exactly between point A and point B
        //rotate z to match direction
        rect.localEulerAngles = new Vector3(0,0, ConvertDirectionToDegrees(dir));
    }

    //converts unit 2d direction vector to a degree from 0 to 360
    float ConvertDirectionToDegrees(Vector2 dir)
    {
        // calc the angle in radians and convert to degrees
        float angleInDegrees = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // ensure the angle is in the range [0, 360] by adding 360 degrees on unit circle
        if (angleInDegrees < 0)
        {
            angleInDegrees += 360;
        }

        //Debug.Log($"Direction angle: {angleInDegrees} degrees");
        return angleInDegrees;
    }

}
