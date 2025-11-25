using Anatidae;
using UnityEngine;

public class SetPlayerNameEditor : MonoBehaviour
{
    [SerializeField] string name;

#if UNITY_EDITOR
    void Awake()
    {
        StartCoroutine(ExtradataManager.FetchExtraData());
        HighscoreManager.PlayerName ??= name;
    }
#endif   
}
