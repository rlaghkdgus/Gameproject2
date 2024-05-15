using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShootingManager : MonoBehaviourPun
{
    public GameObject birdparentobj;
    public GameObject birdparentobj2;
    [SerializeField] Transform p1hitbox;
    [SerializeField] Transform p2hitbox;
    public List<Transform> p1birdTransform = new List<Transform>();
    public List<Transform> p2birdTransform = new List<Transform>();
    public List <GameObject> p1Bird = new List<GameObject>();
    public List <GameObject> p2Bird = new List<GameObject>();
    public GameObject birdPrefab;
    public GameObject bird2Prefab;
    Vector3 bird1rot = new Vector3(-90f, 360f, 0f);
    Vector3 bird2rot = new Vector3(-90f, 180f, 0f);

    private static ShootingManager _instance;

    public static ShootingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ShootingManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(ShootingManager).Name);
                    _instance = singleton.AddComponent<ShootingManager>();
                }
            }
            return _instance;
        }
    }

    public void AddBird(Transform birdtransform, GameObject BirdObject, GameObject birdPf , List<GameObject> birdlist, Vector3 rot)
    {
        var birdObject = Instantiate(birdPf, birdtransform.position, Utills.QI);
        birdObject.transform.rotation = Quaternion.Euler(rot);
        birdObject.transform.parent = BirdObject.transform;
        birdlist.Add(birdObject);
    }
    private void SetBird(PlayerState _pState)
    {
        if(_pState == PlayerState.StartDraw)
        {
            if(TurnSys.Instance.sPlayerIndex.Value == 0)
            {
                for(int i = p1Bird.Count; i <8; i++)
                {
                    AddBird(p1birdTransform[i], birdparentobj, birdPrefab, p1Bird, bird1rot);
                }
            }
            else if(TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                for (int i = p2Bird.Count; i < 8; i++)
                {
                    AddBird(p2birdTransform[i], birdparentobj2, bird2Prefab, p2Bird, bird2rot);
                }
            }
        }
    }
    private void Start()
    {
        for(int i  = 0; i< 7; i++)
        {
            AddBird(p1birdTransform[i], birdparentobj, birdPrefab,p1Bird, bird1rot);
            AddBird(p2birdTransform[i], birdparentobj2, bird2Prefab,p2Bird, bird2rot);
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        GameManager.Instance.S_State.onChange += Shootingbird;
        GameManager.Instance.player[0].pState.onChange += SetBird;
        GameManager.Instance.player[1].pState.onChange += SetBird;
    }
    private void Shootingbird(StrikeState _sState)
    {
        if(_sState == StrikeState.SetStrike)
        {
            StartCoroutine(ShotBird());
        }
    }

    public void DestoyGuardBird()
    {
        StartCoroutine(DestroyGBird());
    }
    public void DestroyTurnEndBird()
    {
        StartCoroutine(DestroyTBird());
    }
    IEnumerator DestroyTBird()
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            Destroy(p1Bird[7]);
            yield return new WaitForSeconds(0.15f);
            p1Bird.RemoveAt(7);
        }
        else if(TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            Destroy(p2Bird[7]);
            yield return new WaitForSeconds(0.15f);
            p2Bird.RemoveAt(7);
        }
    }
    IEnumerator DestroyGBird()
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (GameManager.Instance.player[0].stairCheck == true || GameManager.Instance.player[0].tripleCheck == true)
            {
                for (int i = 2; i >= 0; i--)
                {
                    Destroy(p2Bird[i]);
                    yield return new WaitForSeconds(0.15f);
                    p2Bird.RemoveAt(i);
                }
            }
            else if(GameManager.Instance.player[0].doubleCheck == true)
            {
                for (int i = 1; i >= 0; i--)
                {
                    Destroy(p2Bird[i]);
                    yield return new WaitForSeconds(0.15f);
                    p2Bird.RemoveAt(i);
                }
            }
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (GameManager.Instance.player[1].stairCheck == true || GameManager.Instance.player[1].tripleCheck == true)
            {
                for (int i = 2; i >= 0; i--)
                {
                    Destroy(p1Bird[i]);
                    yield return new WaitForSeconds(0.15f);
                    p1Bird.RemoveAt(i);
                }
            }
            else if (GameManager.Instance.player[1].doubleCheck == true)
            {
                for (int i = 1; i >= 0; i--)
                {
                    Destroy(p1Bird[i]);
                    yield return new WaitForSeconds(0.15f);
                    p1Bird.RemoveAt(i);
                }
            }
        }
    }
    IEnumerator ShotBird()
    {
        Debug.Log("abc");
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (GameManager.Instance.player[0].stairCheck == true || GameManager.Instance.player[0].tripleCheck == true)
            {
                for (int i = 2; i >= 0; i--)
                {
                    while (true)
                    {
                        p1Bird[i].transform.position = Vector3.MoveTowards(p1Bird[i].transform.position, p2hitbox.transform.position, 25f * Time.deltaTime);
                        if (p1Bird[i].transform.position.x >= p2hitbox.transform.position.x-0.1)
                        {
                            Destroy(p1Bird[i]);
                            yield return new WaitForSecondsRealtime(0.1f);
                            p1Bird.RemoveAt(i);
                            break;
                        }
                        yield return null;
                    }
                }
            }
            else if (GameManager.Instance.player[0].doubleCheck == true)
            {
                for (int i = 1; i >= 0; i--)
                {
                    while (true)
                    {
                        p1Bird[i].transform.position = Vector3.MoveTowards(p1Bird[i].transform.position, p2hitbox.transform.position, 25f * Time.deltaTime);
                        if (p1Bird[i].transform.position.x >= p2hitbox.transform.position.x - 0.1)
                        {
                            Destroy(p1Bird[i]);
                            yield return new WaitForSecondsRealtime(0.1f);
                            p1Bird.RemoveAt(i);
                            break;
                        }
                        yield return null;
                    }
                }
            }
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (GameManager.Instance.player[1].stairCheck == true || GameManager.Instance.player[1].tripleCheck == true)
            {
                for (int i = 2; i >= 0; i--)
                {
                    while (true)
                    {
                        p2Bird[i].transform.position = Vector3.MoveTowards(p2Bird[i].transform.position, p1hitbox.transform.position, 25f * Time.deltaTime);
                        if (p2Bird[i].transform.position.x <= p1hitbox.transform.position.x + 0.1)
                        {
                            Destroy(p2Bird[i]);
                            yield return new WaitForSecondsRealtime(0.1f);
                            p2Bird.RemoveAt(i);
                            break;
                        }
                        yield return null;
                    }
                }
            }
            else if (GameManager.Instance.player[1].doubleCheck == true)
            {
                for (int i = 1; i >= 0; i--)
                {
                    while (true)
                    {
                        p2Bird[i].transform.position = Vector3.MoveTowards(p2Bird[i].transform.position, p1hitbox.transform.position, 25f * Time.deltaTime);
                        if (p2Bird[i].transform.position.x <= p1hitbox.transform.position.x + 0.1)
                        {
                            Destroy(p2Bird[i]);
                            yield return new WaitForSecondsRealtime(0.1f);
                            p2Bird.RemoveAt(i);
                            break;
                        }
                        yield return null;
                    }
                }
            }
        }

    }
  
}
