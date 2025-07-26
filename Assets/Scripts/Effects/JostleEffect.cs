using UnityEngine;

public class JostleEffect : MonoBehaviour
{    [SerializeField] float jostleAmount = 0.5f;
    Vector3 origin;
    Vector3 jostle;

    void Start()
    {
        origin = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, origin + jostle, .5f);
        jostle = Vector3.Lerp(jostle, new Vector3(0, 0, 0), 4f * Time.deltaTime);
    }

    public void Jostle(ButtonState direction)
    {
        jostle = new Vector3(0, 0, 0);
        switch (direction)
        {
            case ButtonState.Right:
                jostle.x = -jostleAmount;
                break;
            case ButtonState.Left:
                jostle.x = jostleAmount;
                break;
        }
    }
}
