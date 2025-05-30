using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraDolly : MonoBehaviour
{
    [SerializeField] public List<Transform> waypoints;
    [SerializeField] float timeSpentMoving = 1f;

    [HideInInspector] public int currentWaypointIndex = 0;
    [HideInInspector] public int targetWaypointIndex = 0;

    void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints set for CameraDolly.");
            return;
        }

        transform.position = waypoints[currentWaypointIndex].position;
    }

    public void MoveToNextWaypoint()
    {
        if (targetWaypointIndex < waypoints.Count - 1)
        {
            targetWaypointIndex++;
            StopAllCoroutines();
            StartCoroutine(MoveToWaypoint(waypoints[targetWaypointIndex]));
        }
        else
        {
            Debug.LogWarning("Already at the last waypoint.");
        }
    }

    public void MoveToPreviousWaypoint()
    {
        if (targetWaypointIndex > 0)
        {
            targetWaypointIndex--;
            StopAllCoroutines();
            StartCoroutine(MoveToWaypoint(waypoints[targetWaypointIndex]));
        }
        else
        {
            Debug.LogWarning("Already at the first waypoint.");
        }
    }

    public void MoveToWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Count)
        {
            Debug.LogError("Invalid waypoint index.");
            return;
        }

        if (targetWaypointIndex != index)
        {
            targetWaypointIndex = index;
            StopAllCoroutines();
            StartCoroutine(MoveToWaypoint(waypoints[targetWaypointIndex]));
        }
    }

    private IEnumerator MoveToWaypoint(Transform targetWaypoint)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 endPosition = targetWaypoint.position;
        Quaternion endRotation = targetWaypoint.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < timeSpentMoving)
        {
            float t = elapsedTime / timeSpentMoving;
            float ease = t * t * (3f - 2f * t); // Smoothstep easing

            transform.position = Vector3.Lerp(startPosition, endPosition, ease);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, ease);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        currentWaypointIndex = waypoints.IndexOf(targetWaypoint);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraDolly))]
public class CameraDollyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraDolly cameraDolly = (CameraDolly)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Move to Next Waypoint"))
        {
            if (cameraDolly.waypoints.Count > 0)
            {
                cameraDolly.MoveToNextWaypoint();
            }
            else
            {
                Debug.LogWarning("No waypoints set for CameraDolly.");
            }
        }

        if (GUILayout.Button("Move to previous Waypoint"))
        {
            if (cameraDolly.waypoints.Count > 0)
            {
                cameraDolly.MoveToPreviousWaypoint();
            }
            else
            {
                Debug.LogWarning("No waypoints set for CameraDolly.");
            }
        }
    }
}
#endif