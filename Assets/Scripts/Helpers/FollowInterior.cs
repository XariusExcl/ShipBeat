using UnityEngine;

public class ParentToInterior : MonoBehaviour
{
    Transform interior;

    void Start()
    {
        interior = GameObject.Find("Interior")?.transform;
        if (interior is not null)
            transform.parent = interior;
        else Debug.LogWarning("ParentToInterior: Interior not found!");
    }
}
