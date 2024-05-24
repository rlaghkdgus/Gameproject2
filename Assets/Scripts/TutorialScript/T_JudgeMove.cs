using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_JudgeMove : MonoBehaviour
{
    [SerializeField]
    private GameObject myJudge;
    [SerializeField]
    private GameObject eneJudge;
    private Rigidbody myRigid;
    private Rigidbody eneRigid;
    [SerializeField]
    private float jumpPower;
    private bool firstTurn = true;
    private void Awake()
    {
        myRigid = myJudge.GetComponent<Rigidbody>();
        eneRigid = eneJudge.GetComponent<Rigidbody>();
        T_TurnSys.Instance.gState.onChange += JudgeAnim;
    }
    private void JudgeAnim(GameState _gState) // 턴이 끝남
    {
        if (_gState == GameState.ActionEnd)
        {
            int playerIndex = TurnSys.Instance.sPlayerIndex.Value;
            StartCoroutine(MoveJudge(playerIndex));
        }
    }
    IEnumerator MoveJudge(int turnNum)
    {
        if (turnNum == -1)
        {
            while (true)
            {
                if (T_TurnSys.Instance.sPlayerIndex.Value != -1)
                {
                    turnNum = T_TurnSys.Instance.sPlayerIndex.Value;
                    break;
                }
                yield return null;
            }
        }

        if (firstTurn == true)
        {
            firstTurn = false;
            if (turnNum == 1)
            {
                turnNum = 0;
            }
            else
            {
                turnNum = 1;
            }
        }

        myRigid.useGravity = true;
        eneRigid.useGravity = true;
        yield return new WaitForSecondsRealtime(0.1f);
        if (turnNum == 1)
        {
            myRigid.velocity = Vector3.zero;
            myJudge.transform.localPosition = new Vector3(0, -2, 0);
            myRigid.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
        else if (turnNum == 0)
        {
            eneRigid.velocity = Vector3.zero;
            eneJudge.transform.localPosition = new Vector3(0, -2, 0);
            eneRigid.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
        yield return null;
        yield return new WaitForSecondsRealtime(1f);
        while (true)
        {
            if (turnNum == 1)
            {
                eneRigid.useGravity = false;
                if (myJudge.transform.position.y <= 0.5f)
                {
                    myRigid.velocity = Vector3.zero;
                    myRigid.useGravity = false;
                    myJudge.transform.localPosition = new Vector3(0, 0.5f, 0);
                    break;
                }
            }
            else if (turnNum == 0)
            {
                myRigid.useGravity = false;
                if (eneJudge.transform.position.y <= 0.5f)
                {
                    eneRigid.velocity = Vector3.zero;
                    eneRigid.useGravity = false;
                    eneJudge.transform.localPosition = new Vector3(0, 0.5f, 0);
                    break;
                }
            }
            yield return null;
        }
        yield return null;
    }
}
