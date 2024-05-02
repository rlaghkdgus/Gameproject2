using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}
public class Utills : MonoBehaviour
{
    public static Quaternion QI => Quaternion.identity;

}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour   // �̱����� �����ų Ŭ������
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                    Debug.LogError("Not Activated : " + typeof(T));
            }
            return instance;
        }
    }
}

public static class ExtensionList
{
    public static void Swap<T>(this List<T> list, int from, int to)
    {
        T tmp = list[from];
        list[from] = list[to];
        list[to] = tmp;
    }
}
public class Data<T>//������ ����, Value�� ���Ҷ����� �޼ҵ� ȣ��
{
    private T v;//�̰� ���Ҷ����� �޼ҵ� ȣ��
    public T Value
    {
        get { return v; }//����� �޾ƿ�
        set//����� ���Ҷ�����
        {
            v = value;
            onChange?.Invoke(value);//�����Ϳ� ������ �Լ��� ȣ��
        }
    }
    public Action<T> onChange;//onChange�� ������ �ȿ� �޼ҵ� �߰� �� ����(����)
}
