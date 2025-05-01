using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour {
    public static LaneManager Instance { get; private set; }
    public static List<Lane> Lanes;
    [SerializeField] List<Lane> laneReferences;
    
    void Awake()
    {
        Lanes = laneReferences;
        Instance = this;
    }

    public static Lane GetLane(int id)
    {
        return Lanes[id];
    }
    
    public static void SetLaneCount(int count)
    {
        if (count != 4 && count != 7) {
            Debug.LogError($"Invalid lane count: {count}. Must be 4 or 7.");
            return;
        }

        for (int i = 0; i < Lanes.Count; i++) {
            if (i < count) {
                Lanes[i].gameObject.SetActive(true);
            } else {
                Lanes[i].gameObject.SetActive(false);
            }
        }
    }
}