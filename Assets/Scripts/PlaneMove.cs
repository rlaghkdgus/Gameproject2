using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMove : MonoBehaviour
{
    public Animator PlaneMoveAnim;
    public bool PlaneMoveEnabled = false;

    private void Start()
    {
        PlaneMoveAnim = GetComponent<Animator>();
    }
    void OnMouseDown()
    {
        if(PlaneMoveEnabled)
        {
            return;
        }
        else 
        {
            StartCoroutine(PlaneAnimDelay());
        }
    }
    IEnumerator PlaneAnimDelay()
    {
        PlaneMoveEnabled = true;
        PlaneMoveAnim.SetTrigger("move");
        yield return new WaitForSecondsRealtime(3.0f);
        PlaneMoveEnabled = false;
    }
}
