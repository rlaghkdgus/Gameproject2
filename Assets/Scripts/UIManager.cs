using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class UIManager : Singleton<UIManager>
{
    public int CharaNum;
    public PhotonView PV;

    
    public void ChangeEmBer()
    {
        CharaNum = 1;
    }
    public void ChangeEmilia()
    {
        CharaNum = 2;
    }
    public void ChangeFogel()
    {
        CharaNum = 3;
    }


}
