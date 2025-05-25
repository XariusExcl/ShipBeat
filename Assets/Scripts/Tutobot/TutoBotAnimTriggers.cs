using UnityEngine;

public class TutoBotAnimTriggers : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void CloseAAA()
    {
        animator.SetTrigger("closeAAA");
    }

    public void Guruguru()
    {
        animator.SetTrigger("guruguru");
    }

    public void Half()
    {
        animator.SetTrigger("half");
    }

    public void Inequal()
    {
        animator.SetTrigger("inequal");
    }

    public void Open()
    {
        animator.SetTrigger("open");
    }

    public void Point()
    {
        animator.SetTrigger("point");
    }

    public void RechargeOn()
    {
        animator.SetTrigger("rechargeOn");
    }

    public void Sleep()
    {
        animator.SetTrigger("sleep");
    }

    public void Smile()
    {
        animator.SetTrigger("smile");
    }

    public void HandLExplain()
    {
        animator.SetTrigger("handLExplain");
    }

    public void HandLFist()
    {
        animator.SetTrigger("handLFist");
    }

    public void HandLPoint()
    {
        animator.SetTrigger("handLPoint");
    }

    public void HandLSplayed()
    {
        animator.SetTrigger("handLSplayed");
    }

    public void HandLThumbsup()
    {
        animator.SetTrigger("handLThumbsup");
    }

    public void HandRExplain()
    {
        animator.SetTrigger("handRExplain");
    }

    public void HandRFist()
    {
        animator.SetTrigger("handRFist");
    }

    public void HandRPoint()
    {
        animator.SetTrigger("handRPoint");
    }

    public void HandRSplayed()
    {
        animator.SetTrigger("handRSplayed");
    }

    public void HandRThumbsup()
    {
        animator.SetTrigger("handRThumbsup");
    }
}
