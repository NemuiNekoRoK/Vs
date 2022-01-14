using UnityEngine;
using System.Collections;

//플레이어 클래스 (키 입력으로 조종)
public class Player_Key : Player_Base {

	public static GameObject m_mainPlayer = null; //유저가 조종하는 플레이어(AI용)
	//초기화로 자기 자신의 오브젝트를 기억
	private void Awake() {
		m_mainPlayer = gameObject;
	}
	public KeyCode KEYCODE_MOVE_LEFT = KeyCode.A;
	public KeyCode KEYCODE_MOVE_UP = KeyCode.W;
	public KeyCode KEYCODE_MOVE_RIGHT = KeyCode.D;
	public KeyCode KEYCODE_MOVE_DOWN = KeyCode.S;
	public KeyCode KEYCODE_SHOOT = KeyCode.Space;

	//입력처리 체크
	protected override void GetInput() {
		//좌우이동
		if(Input.GetKey(KEYCODE_MOVE_LEFT)) {
			m_playerInput[(int)PlayerInput.Move_Left] = true;
		} else
		if(Input.GetKey(KEYCODE_MOVE_RIGHT)) {
			m_playerInput[(int)PlayerInput.Move_Right] = true;
		}

		//상하이동
		if(Input.GetKey(KEYCODE_MOVE_UP)) {
			m_playerInput[(int)PlayerInput.Move_Up]	= true;
		} else
		if(Input.GetKey( KEYCODE_MOVE_DOWN)) {
			m_playerInput[(int)PlayerInput.Move_Down] = true;
		}
		//발사
		if(Input.GetKeyDown( KEYCODE_SHOOT)) {//누르는 순간에만 유효. GetKeyDown을 사용
			m_playerInput[(int)PlayerInput.Shoot]	= true;
		}
	}
}
