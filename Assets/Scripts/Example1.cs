using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example1 : MonoBehaviour
{
    [SerializeField]
    public List<int> card = new List<int>();

    private void Awake()
    {

        Check();
    }
   
    private void Check()
    {
        int headCheck = 0;
        int bodyCheck = 0;
        int fourcardCheck = 0;
        for(var i = 0; i < card.Count; i++)
        {
            if (i + 1 >= card.Count)
            {
                if(i == 6)
                {
                    if(card[6]==card[7])
                    {
                        headCheck++;
                    }
                }
                break;
            }

            int temp = card[i];
            if (temp == card[i + 1])         // HEAD CHECK
            {
                if (i + 2 >= card.Count)
                {
                    if(card[6]==card[7])
                    {
                        headCheck++;
                    }
                    break;
                } 

                if (temp == card[i + 2])//같은수 3개가 몸통일경우
                {
                    bodyCheck++;
                    i++;
                    i++;
                    Debug.Log("HEAD => BODY");
                    if (i == 7)
                        break;
                    if (temp == card[i + 1])//11112233의경우 체크
                    {
                        Debug.Log(i);
                        bodyCheck--;
                        headCheck++;
                        headCheck++;//헤드체크 두번 추가
                        fourcardCheck++;
                        i++;
                        Debug.Log("Body->2Head");
                    }
                }
                else
                {
                    Debug.Log("HEAD");
                    headCheck++;
                    i++;
                    if (i >= card.Count) 
                        break;
                }
            }
            else                             // BODY CHECK
            {
                if (i + 2 >= card.Count)
                    if (card[5] == card[6] - 1 && card[6] == card[7] - 1)
                    {
                        bodyCheck++;
                        break;
                    }
                if (temp + 1 == card[i + 1] && temp + 2 == card[i + 2])//연속수3개가 몸통일경우
                {
                    i++;
                    i++;
                    bodyCheck++;
                    Debug.Log("BODY");
                }
            }
            
        }
        if(headCheck == 1 && bodyCheck == 2)
        {
            Debug.Log("Check");
        }
        else if(headCheck == 3)
        {
            if (card[0] + 3 == card[5])
            {
                Debug.Log("Check");
            }
            else if (card[0] + 3 == card[7])
            {
               
                Debug.Log("Check");
            }
            else if (card[2] + 3 == card[7])
            {
                if (card[0] <= card[7] - 4)
                    return;
                Debug.Log("Check");
            }
            else if (fourcardCheck == 1)
            { 
                if ( card[0] +2 == card[5])
                {
                    Debug.Log("Check");
                }

            }
        }
        else if(headCheck == 4)
        {
            if(card[0] == card[2] -1 && card[2] == card[4] -1 && card[4] == card[6] -1)
            {
                Debug.Log("Check");
            }
        }
        else if(headCheck == 2 && bodyCheck == 1)
        {
            if(card[0] == card[1]-1 && card[1] == card[3] -1 &&  card[3] == card[5] -1)
            {
                if (card[0] <= card[7] - 4)
                    return;
                Debug.Log("Check");
            }
            else if (card[0] == card[3] - 1 && card[3] == card[5] - 1 && card[5] == card[7] - 1)
            {
                Debug.Log("Check");
            }
            
        }
        else if (bodyCheck == 2)
        {
            if(card[3] == card[7] -2)
            {
                Debug.Log("Check");
            }
            else if (card[0] == card[4] - 2)
            {
                Debug.Log("Check");
            }
            else if (card[0] == card[7] - 2)
            {
                Debug.Log("Check");
            }
        }
        else if (headCheck == 1 && bodyCheck == 1)
        {
            if(card[0] == card[6]-3 && card[6] == card[7] -1)
            {
                Debug.Log("Check");
            }
        }

       
    }
}
