using UnityEngine;
using Photon.Pun;

public class ButtonHandler : MonoBehaviourPun
{
    // 버튼 클릭 이벤트 핸들러
    public void OnButtonClick()
    {
        // PhotonView를 통해 RPC 호출
        photonView.RPC("GoGameSceneRPC", RpcTarget.MasterClient);
    }
}
