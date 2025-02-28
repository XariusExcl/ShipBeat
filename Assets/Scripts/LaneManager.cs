using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour {
    public static List<Lane> Lanes;
    [SerializeField] List<Lane> laneReferences;
    
    void Start()
    {
        Lanes = laneReferences;
    }

    public static Lane GetLane(int id)
    {
        return Lanes[id];
    }
}