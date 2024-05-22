using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,       //�ƹ��͵� ���Ҷ�
    StartDraw,  //������ ī�带 �޴� �ܰ�
    Select,     //�÷��̾ ī�带 �����ϴ� �ܰ�
    delay,      //ī�� ������ üũ���� �����̿� ����
    SelectFin,  //�÷��̾ ���ÿϷḦ �� ����
    End         //�����÷��̾�� ���� �ѱ��
}
public enum GameState
{
    Idle,       //�ƹ��͵� �������� �ʴ� ����
    WaitAction, //������ ��ٸ��� ����
    ActionEnd,   //�÷��̾��� �ൿ�� ���� ����
    GameEnd     //������ ���� ����
}
public enum GuardState
{
    Idle,
    DoCheckGuard,
    GuardSelect,
    End
}
public enum StrikeState
{
    Idle,
    ReadyStrike,
    SetStrike
}

