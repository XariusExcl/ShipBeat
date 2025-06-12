using UnityEngine;

public class TutoBotBehaviour : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        // if scene name is Menu, set the anim triggers to Type
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
        {
            animator.SetTrigger("handsType");
        }
        else
        {
            animator.SetTrigger("handsIdle");
        }
    }
}