using UnityEngine;

[CreateAssetMenu(fileName = "SongSelectSceneData", menuName = "Scriptable Objects/SongSelectSceneData")]
public class SongSelectSceneData : ScriptableObject
{
    [SerializeField] public Gradient ratingColorGradient;
    [SerializeField] public Gradient uIRatingColorGradient;
    [SerializeField] public Color pRankColor;
    [SerializeField] public Color sRankColor;
    [SerializeField] public Color aRankColor;
    [SerializeField] public Color bRankColor;
    [SerializeField] public Color cRankColor;
    [SerializeField] public Color dRankColor;
    [SerializeField] public Color fRankColor;

    public Color GetColorForRating(float rating)
    {
        return ratingColorGradient.Evaluate(Mathf.Clamp01((rating - 3f) / 12f));
    }

    public Color GetColorForRatingUI(float rating)
    {
        return uIRatingColorGradient.Evaluate(Mathf.Clamp01((rating - 3f) / 12f));
    }
}
