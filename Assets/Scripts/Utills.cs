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

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour   // 싱글톤을 적용시킬 클래스에
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
public class Data<T>//옵저버 패턴, Value가 변할때마다 메소드 호출
{
    private T v;//이게 변할때마다 메소드 호출
    public T Value
    {
        get { return v; }//밸류를 받아옴
        set//밸류가 변할때마다
        {
            v = value;
            onChange?.Invoke(value);//데이터에 구독된 함수를 호출
        }
    }
    public Action<T> onChange;//onChange로 데이터 안에 메소드 추가 및 제거(구독)
}
