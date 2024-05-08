using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniManager : MonoBehaviour
{
    public Animator Judgeanim;
    public GameObject targetPosition1;
    public GameObject targetPosition2;
    public GameObject targetPosition3;
    public GameObject targetPosition4;
    public bool JudgePosition = false;
    public bool Judge1 = false;
    // 회전할 속도
    public float rotationSpeed = 30f;

    // 목표 회전 각도


    // 회전이 완료되었는지 여부


    // Start is called before the first frame update

    private void Awake()
    {
        TurnSys.Instance.gState.onChange += JudgeAnim;
    }

    private void JudgeAnim(GameState _gState)
    {
        if (_gState == GameState.ActionEnd)
        {
            StartCoroutine(MoveJudge());
        }
    }

    void Start()
    {
        Judgeanim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (JudgePosition == true)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition1.transform.position, 0.01f);
            if (gameObject.transform.position == targetPosition1.transform.position)
            {
                transform.position = targetPosition3.transform.position;
                Judge1 = true;
            }
            if (Judge1 == true)
            {
                transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition4.transform.position, 0.01f);
            }
        }
        else if (JudgePosition == false)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition2.transform.position, 0.01f);
            if (gameObject.transform.position == targetPosition2.transform.position)
            {
                transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetPosition2.transform.rotation, 100 * Time.deltaTime);
            }
        }
    }
    IEnumerator MoveJudge()
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            Judgeanim.SetTrigger("FlyBird");
            yield return new WaitForSeconds(0.5f);
            JudgePosition = true;
        }
        if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            Judgeanim.SetTrigger("FlyBird");
            yield return new WaitForSeconds(0.5f);
            JudgePosition = false;
        }
    }
}