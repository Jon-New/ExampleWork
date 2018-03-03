using UnityEngine;
using System.Collections;
using UnityEditor;

/*
    This script changes how the script appears in the inspector in Unity.

    There are two reasons:
        - Displays the internal variables in plain language
        - To only display the variables used by the currently selected type

    This helps prevent errors when using the visibility script.
*/

[CustomEditor(typeof(Visibility))]
public class VisibilityEditor : Editor {

    public override void OnInspectorGUI()
    {
        Visibility myTarget = (Visibility)target;

        myTarget.visibilityType = (Visibility.VisibilityType) EditorGUILayout.EnumPopup("Visibility Type", myTarget.visibilityType);

        if (myTarget.visibilityType == Visibility.VisibilityType.Interval)
        {
            myTarget.intervalType = (Visibility.IntervalType) EditorGUILayout.EnumPopup("Interval Type", myTarget.intervalType);

            if (myTarget.intervalType == Visibility.IntervalType.Static)
            {
                myTarget.iStatic_VisibleTime = EditorGUILayout.FloatField("Time Visible", myTarget.iStatic_VisibleTime);
                myTarget.iStatic_InvisibleTime = EditorGUILayout.FloatField("Time Invisible", myTarget.iStatic_InvisibleTime);
            }
            else if (myTarget.intervalType == Visibility.IntervalType.Random)
            {
                myTarget.iRandom_MinVis = EditorGUILayout.FloatField("Minimum Time Visible", myTarget.iRandom_MinVis);
                myTarget.iRandom_MaxVis = EditorGUILayout.FloatField("Maximum Time Visible", myTarget.iRandom_MaxVis);
                myTarget.iRandom_MinInvis = EditorGUILayout.FloatField("Minimum Time Invisible", myTarget.iRandom_MinInvis);
                myTarget.iRandom_MaxInvis = EditorGUILayout.FloatField("Maximum Time Invisible", myTarget.iRandom_MaxInvis);
            }
        }
        else if (myTarget.visibilityType == Visibility.VisibilityType.Distance)
        {
            myTarget.distanceType = (Visibility.DistanceType) EditorGUILayout.EnumPopup("Distance Type", myTarget.distanceType);

            if (myTarget.distanceType == Visibility.DistanceType.Inside)
                myTarget.dInside_Distance = EditorGUILayout.FloatField("Distance", myTarget.dInside_Distance);
            else if (myTarget.distanceType == Visibility.DistanceType.Outside)
                myTarget.dOutside_Distance = EditorGUILayout.FloatField("Distance", myTarget.dOutside_Distance);
            else if (myTarget.distanceType == Visibility.DistanceType.Between)
            {
                myTarget.dBetween_MinDis = EditorGUILayout.FloatField("Minimum Distance", myTarget.dBetween_MinDis);
                myTarget.dBetween_MaxDis = EditorGUILayout.FloatField("Maximum Distance", myTarget.dBetween_MaxDis);
            }
        }
    }
}
