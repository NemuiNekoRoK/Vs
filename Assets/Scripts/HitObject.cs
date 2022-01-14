using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//컬라이더로 접촉하는 오브젝트의 기저 클래스
public class HitObject : MonoBehaviour{
    //충돌 밮ㄴ정 그룹
    public enum HitGroup{Player1, Player2, Other}

    public HitGroup m_hitGroup = HitGroup.Player1; //자신의 충돌 판정 그룹

    //컬라이더가 닿아도 괜찮은지 확인
    protected bool IsHitOK(GameObject hittedObject){
        //상대가 같은 스크립트를 가지고 있는가?
        HitObject hit = hittedObject.GetComponent<HitObject>();
        //같은 스크립트를 갖고있지 않으면 닿지 않아도 됨
        if(null == hit) return false;
        //동일 그룹간 판정 무시
        if(m_hitGroup == hit.m_hitGroup) return false;
        //다른 그룹끼리 충돌
        return true;
    }
}
