using UnityEngine;
using System.Collections;
//정해진 시간이 지나면 사라지는 객체용 스크립트
public class Timer : MonoBehaviour {
	public float fTimeLimit = 1f;//각 프리팹의 생존 시간
	private float m_fTimeLeft = 0f;//남은 생존시간
	//초기화시 호출되는 함수
	private void Awake() {
		//제한시간 지정
		m_fTimeLeft = fTimeLimit;
	}
	//매 프레임마다 호출되는 함수
	private void Update() {
		//지정된 시간 까지
		m_fTimeLeft -= Time.deltaTime;
		if(m_fTimeLeft < 0f) {//시간 끝
			//이GameObject를 하이어라키에서 삭제
			Destroy( gameObject);
		}
	}
}
