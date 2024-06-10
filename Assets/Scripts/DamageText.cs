using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{
    public float moveSpeed; //�ؽ�Ʈ �̵� �ӵ�
    public float alphaSpeed; //���� ��ȯ �ӵ�
    public TMP_Text damageText;
    public float destoryTime; // �ؽ�Ʈ �ı� �ӵ�
    public int damage;
    Color alpha;
    // Start is called before the first frame update
    void Start()
    {
        damageText = GetComponent<TMP_Text>();
        damageText.text = "" + damage;
        alpha = damageText.color;
        Invoke("DestroyObj", destoryTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed, 0));
        alpha.a = Mathf.Lerp(alpha.a,0, Time.deltaTime * alphaSpeed);
        damageText.color = alpha;
    }
    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
