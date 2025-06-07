using UnityEngine;

[CreateAssetMenu(fileName = "SongSelectSceneData", menuName = "Scriptable Objects/SongSelectSceneData")]
public class SongSelectSceneData : ScriptableObject
{
    [SerializeField] public Gradient ratingColorGradient;

    public Color GetColorForRating(float rating)
    {
        return ratingColorGradient.Evaluate(Mathf.Clamp01((rating - 3f) / 12f));
    }
}
