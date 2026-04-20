using UnityEngine;

public class TestLogin : MonoBehaviour
{
# if UNITY_EDITOR
    [SerializeField] string Name;
    void Awake()
    {
        if(OnlineDataManager.Data.PlayerInfo.Name is not null) return;
        StartCoroutine(OnlineDataManager.GetPlayer(Name, () => {} ));
    }
# endif
}