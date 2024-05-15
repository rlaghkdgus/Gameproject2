using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SyncObject : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;

    // Network synchronized variables
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private bool networkCardState;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Writing data to network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(networkCardState);
        }
        else
        {
            // Reading data from network
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkCardState = (bool)stream.ReceiveNext();
        }
    }
}