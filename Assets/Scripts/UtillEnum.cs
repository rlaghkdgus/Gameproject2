using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,       //아무것도 안할때
    StartDraw,  //선택전 카드를 받는 단계
    Select,     //플레이어가 카드를 선택하는 단계
    delay,      //카드 선택후 체크동안 딜레이용 상태
    SelectFin,  //플레이어가 선택완료를 한 상태
    End         //다음플레이어에게 턴을 넘기기
}
public enum GameState
{
    Idle,       //아무것도 동작하지 않는 상태
    WaitAction, //동작을 기다리는 상태
    ActionEnd,   //플레이어의 행동이 끝난 상태
    GameEnd     //게임이 끝난 상태
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

