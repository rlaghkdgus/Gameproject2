using UnityEngine;
using Photon.Pun;

public class ButtonHandler : MonoBehaviourPun
{
    // ��ư Ŭ�� �̺�Ʈ �ڵ鷯
    public void OnButtonClick()
    {
        // PhotonView�� ���� RPC ȣ��
        photonView.RPC("GoGameSceneRPC", RpcTarget.MasterClient);
    }
}
