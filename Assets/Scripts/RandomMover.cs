using UnityEngine;
using System.Collections;

public class RandomMover : Singleton<RandomMover>
{
    // �̵� �ӵ�
    public float speed = 5f;
  
    // x�� y ����
    public Vector2 xRange = new Vector2(-10f, 10f);
    public Vector2 yRange = new Vector2(-5f, 5f);

    // ���� Ÿ�̸� ����
    public Vector2 timeRange = new Vector2(1.5f, 2.5f);

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        ShootingManager.Instance.Sh_State.onChange += RandomMove;
    }
    void Start()
    {
        StartCoroutine(MoveRandomly());
    }
    private void RandomMove(ShootingState _sState)
    {
        if(_sState == ShootingState.Idle)
        {
            StartCoroutine(MoveRandomly());
        }
    }
    IEnumerator MoveRandomly()
    {
        while (true)
        {
            float randomTime = Random.Range(timeRange.x, timeRange.y);
            yield return new WaitForSeconds(randomTime);
            if (ShootingManager.Instance.Sh_State.Value == ShootingState.Shooting)
                break;
            // ���� ��ġ ����
            float randomX = Random.Range(xRange.x, xRange.y);
            float randomY = Random.Range(yRange.x, yRange.y);
            targetPosition = new Vector3(randomX, randomY, transform.position.z);

            
            // ������Ʈ�� ��ǥ ������ ������ ������ �̵�
            while ((transform.position - targetPosition).magnitude > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                if (ShootingManager.Instance.Sh_State.Value == ShootingState.Shooting)
                    break;
                yield return null;
            }

           
           
        }
    }
}