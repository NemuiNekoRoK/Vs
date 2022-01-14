using UnityEngine;
using System.Collections;

//탄 클래스
//뭐든 접촉하며 이펙트를 출력하며 소멸
public class Bullet : HitObject {
	private	static readonly float bulletMoveSpeed = 10.0f;	//탄이 1초간 이동하는 속도
	public GameObject hitEffectPrefab = null;
	//매 프레임마다 호출되는 함수
	private	void Update() {
		//이동
		{
			//1초간 이동량
			Vector3	vecAddPos = (Vector3.forward *bulletMoveSpeed);
	/*
		Vector3.forward 는 new Vector3( 0f, 0f, 1f)와 동일
		Vector3 에 transform.rotation 을 걸면,그 방향으로 꺾는다
		그때、Vector3 는 Z+ 의 방향을 정면이라고 생각한다
	*/
			//이동량, 회전량은 Time.deltaTime을 걸어서 실행환경(프레임 수 차이)에따른 차이가 발생하지 않도록 한다
			transform.position	+= ((transform.rotation *vecAddPos) *Time.deltaTime);
		}
	}

	//컬라이더가 어떤 물체에 닿으면 호출되는 함수
	//자신의 GameObject에 컬라이더와 IsTrigger를 On으로 함
	//리지드바디를 적용하면호출 가능한 상태가됨
	private void OnTriggerEnter(Collider hitCollider) {
		//히트시 효과 검사
		if(false==IsHitOK(hitCollider.gameObject)) {
			//히트 효과 표현
			return;
		}

		//히트 효과를 표현할지 판정
		if(null!=hitEffectPrefab) {
			//자신의 위치에서 히트 효과 표현
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}

		//해당 게임오브젝트를 하이어라키에서 삭제
		Destroy( gameObject);
	}
}
