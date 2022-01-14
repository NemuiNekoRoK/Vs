using UnityEngine;
using System.Collections;

 //플레이어 클래스 베이스
 //캐릭터의 이동, 메커니즘(모션)의통제
public class Player_Base : HitObject {
	//플레이어 조작
	protected enum PlayerInput {
		Move_Left, Move_Up, Move_Right, Move_Down, Shoot, EnumMax
	}

	private	static readonly	float MOVE_ROTATION_Y_LEFT = -90f;
	private	static readonly	float MOVE_ROTATION_Y_UP = 0f;
	private	static readonly	float MOVE_ROTATION_Y_RIGHT = 90f;
	private	static readonly	float MOVE_ROTATION_Y_DOWN = 180f;

	public float MOVE_SPEED	= 5.0f;
	public GameObject playerObject = null;//움직이는 대상 모델
	public GameObject bulletObject = null;//탄 프리팹
	public GameObject hitEffectPrefab = null;//히트 이펙트 프리팹

	private	float m_rotationY = 0.0f; //플레이어 회전 각도
	protected bool[] m_playerInput = new bool[ (int)PlayerInput.EnumMax];//키 입력
	protected bool m_playerDeadFlag = false; //플레이어 사망 플래그



	//매 프레임마다 호출되는 함수
	private void Update() {
		//플레이어 사망 상태
		if( m_playerDeadFlag) {
			//전체 처리 무시
			return;
		}
		//플래그 초기화
		ClearInput();
		GetInput();

		//이동
		CheckMove();
	}

	//입력 처리 검사
	private void ClearInput() {
		//플래그 초기화
		int	i;
		for(i=0; i<(int)PlayerInput.EnumMax; i++) {
			m_playerInput[i]	= false;
		}
	}
	//입력처리 검사
	protected virtual void GetInput() {
	}

	//입력처리 검사
	private void CheckMove() {

		//애니메이터(매커니즘)get
		Animator animator = playerObject.GetComponent<Animator>();

		//총에 맞지 않으면 이동 OK
		float moveSpeed	= MOVE_SPEED;//이동속도
		bool shootFlag = false;//탄 발사 여부 플래그

		//이동과 회전
		{
			//키 입력으로 이동과 회전
			if(m_playerInput[(int)PlayerInput.Move_Left]) {
				m_rotationY = MOVE_ROTATION_Y_LEFT;
			} else
			if(m_playerInput[(int)PlayerInput.Move_Up]) {
				m_rotationY = MOVE_ROTATION_Y_UP;
			} else
			if(m_playerInput[(int)PlayerInput.Move_Right]) {
				m_rotationY = MOVE_ROTATION_Y_RIGHT;
			} else
			if(m_playerInput[(int)PlayerInput.Move_Down]) {
				m_rotationY = MOVE_ROTATION_Y_DOWN;
			} else{
				//입력x == 이동 x
				moveSpeed = 0f;
			}

			//이동 방향을 오일러 각으로 입력
			transform.rotation = Quaternion.Euler( 0, m_rotationY, 0);//Y축 회전으로 캐릭터 방향을 옆으로 변경

			//이동량을 Transform에 넘겨주어 이동
			transform.position += ((transform.rotation *(Vector3.forward *moveSpeed)) *Time.deltaTime);
		}

		//사격
		{
			//사격 버튼을 눌렸는지 여부
			if(m_playerInput[(int)PlayerInput.Shoot]) {
				//발사
				shootFlag = true;
				//탄 생성 위치
				Vector3 vecBulletPos = transform.position;
				//진행 방향 지정(조금 앞)
				vecBulletPos += (transform.rotation	*Vector3.forward);
				//Y는 높이를 조금 올린다
				vecBulletPos.y = 2.0f;
				//탄 생성
				Instantiate(bulletObject, vecBulletPos, transform.rotation);
			} else {
				//발사X
				shootFlag = false;
			}
		}
		//메카님
		{
			//Animator에 값 전달
			animator.SetFloat("Speed", moveSpeed);//이동량
			animator.SetBool("Shoot", shootFlag);//발사 플래그
		}
	}

	//컬라이더가 무언가에 닿으면 호출되는 함수
	//자신의 GameObject에 컬라이더(IsTrigger를 지정)와 리지드바디를 설정하면 호출됨
	private	void OnTriggerEnter(Collider hitCollider) {
		//히트 해도 되는지 판단
		if(false==IsHitOK(hitCollider.gameObject)) {
			//해당 오브젝트에서 닿으면 안됨
			return;
		}
		//탄에 맞음
		{
			//에니메이터(메카님)을 얻음
			Animator animator = playerObject.GetComponent<Animator>();
			//메카님에게 사망했음을 알림
			animator.SetBool("Dead", true);//사망 플래그
		}
		//히트 이펙트가 있는가?
		if(null!=hitEffectPrefab) {
			//자신의 현재 위치에서 히트 효과 표현
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
		}
		//해당 플레이어는 죽은상태로 설정
		m_playerDeadFlag = true;
	}
}
