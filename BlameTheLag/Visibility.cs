using UnityEngine;
using System.Collections;

/*
    This script handles functionality to make the object it is attached to become visible or invisible based on different rules.
*/

public class Visibility : MonoBehaviour {

    //This script has an accompanying custom editor script.  Changing the name of a variable will create errors.

    public enum VisibilityType {Interval, Distance};
    public enum IntervalType {Static, Random};
    public enum DistanceType {Inside, Outside, Between};

    //The process by which the object changes its visibility
    public VisibilityType visibilityType;
    //If the time intervals are a static number or randomized
    public IntervalType intervalType;
    //The area in which the object will be visible
    public DistanceType distanceType;

    //How long the object will be visible, in seconds
    public float iStatic_VisibleTime;
    //How long the object will be invisible, in seconds
    public float iStatic_InvisibleTime;

    //The minimum time that the object can be visible for, in seconds
    public float iRandom_MinVis;
    //The maximum time that the object can be visible for, in seconds
    public float iRandom_MaxVis;
    //The minimum time that the object can be invisible for, in seconds
    public float iRandom_MinInvis;
    //The maximum time that the object can be invisible for, in seconds
    public float iRandom_MaxInvis;

    private float i_timer;
    private bool goingInvisible = true;

    //The distance at which the object will become visible
    public float dInside_Distance;
    //The distance at which the object will become visible
    public float dOutside_Distance;
    //The lower value of the area where the object is visible
    public float dBetween_MinDis;
    //The higher value of the area where the object is visible
    public float dBetween_MaxDis;

    public float alphaValue;
    public Color tempColor;

    private Transform ball;
    private SpriteRenderer rend;

    void Start ()
    {
        VariableDebugs();
        ball = GameObject.FindGameObjectWithTag("Player").transform;
        rend = gameObject.GetComponent<SpriteRenderer>();

        if (visibilityType == VisibilityType.Interval)
        {
            if (intervalType == IntervalType.Static)
                i_timer = iStatic_VisibleTime;
            else
                i_timer = Random.Range(iRandom_MinVis, iRandom_MaxVis);
        }
    }


    //A set of debug statements that lets the user know if a variable hasn't been set.
    void VariableDebugs()
    {
        if (visibilityType == VisibilityType.Distance)
        {
            if (distanceType == DistanceType.Inside)
            {
                if (dInside_Distance == 0)
                    Debug.Log("The variable \"Distance\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
            }
            else if (distanceType == DistanceType.Outside)
            {
                if (dOutside_Distance == 0)
                    Debug.Log("The variable \"Distance\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
            }
            else if (distanceType == DistanceType.Between)
            {
                if (dBetween_MinDis == 0)
                    Debug.Log("The variable \"Minimum Distance\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
                if (dBetween_MaxDis == 0)
                    Debug.Log("The variable \"Maximum Distance\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");

                if (dBetween_MinDis >= dBetween_MaxDis)
                    Debug.Log("The variable \"Minimum Distance\" on " + gameObject.name + " should be less than the variable \"Maximum Distance\".  Change it in the inspector.");
            }

        }
        else if (visibilityType == VisibilityType.Interval)
        {
            if (intervalType == IntervalType.Static)
            {
                if (iStatic_VisibleTime == 0)
                    Debug.Log("The variable \"Time Visible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
                if (iStatic_InvisibleTime == 0)
                    Debug.Log("The variable \"Time Invisible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
            }
            else if (intervalType == IntervalType.Random)
            {
                if (iRandom_MinVis == 0)
                    Debug.Log("The variable \"Minimum Time Visible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
                if (iRandom_MaxVis == 0)
                    Debug.Log("The variable \"Maximum Time Visible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
                if (iRandom_MinInvis == 0)
                    Debug.Log("The variable \"Minimum Time Invisible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");
                if (iRandom_MaxInvis == 0)
                    Debug.Log("The variable \"Maximum Time Visible\" on " + gameObject.name + " is set to 0.  Change it in the inspector.");

                if (iRandom_MinVis >= iRandom_MaxVis)
                    Debug.Log("The variable \"Minimum Time Visible\" on " + gameObject.name + " should be less than the variable \"Maximum Time Visible\".  Change it in the inspector.");
                if (iRandom_MinInvis >= iRandom_MaxInvis)
                    Debug.Log("The variable \"Minimum Time Invisible\" on " + gameObject.name + " should be less than the variable \"Maximum Time Invisible\".  Change it in the inspector.");
            }
        }
    }
	
	void Update ()
    {
        //Begins block that deals with distance-based visibility
        if (visibilityType == VisibilityType.Distance)
        {
            if (distanceType == DistanceType.Inside) //If the object is visible when the player object is close to it
            {
                alphaValue = (dInside_Distance - Vector2.Distance(transform.position, ball.position));

                if (alphaValue > 1)
                    alphaValue = 1;
                else if (alphaValue < 0)
                    alphaValue = 0;
                
                tempColor = rend.color;
                tempColor.a = alphaValue;
                rend.color = tempColor;
            }
            else if (distanceType == DistanceType.Outside) //If the object is visible when the player object is far from it
            {
                alphaValue = (Vector2.Distance(transform.position, ball.position) - dOutside_Distance);

                if (alphaValue > 1)
                    alphaValue = 1;
                else if (alphaValue < 0)
                    alphaValue = 0; 

                tempColor = rend.color;
                tempColor.a = alphaValue;
                rend.color = tempColor;
            }
            else if (distanceType == DistanceType.Between) //If the object is visible when in between a lower and upper number
            {
                if (Vector2.Distance(transform.position, ball.position) > (dBetween_MinDis + dBetween_MaxDis)/2)
                    alphaValue  = dBetween_MaxDis - Vector2.Distance(transform.position, ball.position);
                else
                    alphaValue = Vector2.Distance(transform.position, ball.position) - dBetween_MinDis;


                if (alphaValue > 1)
                    alphaValue = 1;
                else if (alphaValue < 0)
                    alphaValue = 0; 

                tempColor = rend.color;
                tempColor.a = alphaValue;
                rend.color = tempColor;
            }
        }
        //Ends block that deals with distance-based visibility

        //Begins block that deals with time-based visibility
        else if (visibilityType == VisibilityType.Interval)
        {
            if (intervalType == IntervalType.Static)
            {
                i_timer -= Time.deltaTime;

                if (i_timer <= 0)
                {
                    if (goingInvisible)
                    {
                        i_timer = iStatic_InvisibleTime;
                        goingInvisible = false;
                    }
                    else
                    {
                        i_timer = iStatic_VisibleTime;
                        goingInvisible = true;
                    }
                }
                else if (i_timer <= 1)
                {
                    if (goingInvisible)
                    {
                        tempColor = rend.color;
                        tempColor.a = i_timer;
                        rend.color = tempColor;
                    }
                    else
                    {
                        tempColor = rend.color;
                        tempColor.a = 1-i_timer;
                        rend.color = tempColor;
                    }
                }
            }
            else if (intervalType == IntervalType.Random)
            {
                i_timer -= Time.deltaTime;

                if (i_timer <= 0)
                {
                    if (goingInvisible)
                    {
                        i_timer = Random.Range(iRandom_MinInvis, iRandom_MaxInvis);
                        goingInvisible = false;
                    }
                    else
                    {
                        i_timer = Random.Range(iRandom_MinVis, iRandom_MaxVis);
                        goingInvisible = true;
                    }
                }
                else if (i_timer <= 1)
                {
                    if (goingInvisible)
                    {
                        tempColor = rend.color;
                        tempColor.a = i_timer;
                        rend.color = tempColor;
                    }
                    else
                    {
                        tempColor = rend.color;
                        tempColor.a = 1-i_timer;
                        rend.color = tempColor;
                    }
                }
            }
        }
        //Ends block that deals with time-based visibility
    }
}
