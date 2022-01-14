using UnityEngine;
using System.Collections;

//필드 생성 프로그램
public class Field : MonoBehaviour {
	public GameObject m_blockObject = null;//블록 프리팹
	public GameObject m_player1Object = null;//플레이어1 프리팹
	public GameObject m_player2Object = null;//플레이어2 프리팹

	public static readonly int FIELD_GRID_X = 9;//필드 X그리드 수
	public static readonly int FIELD_GRID_Y = 9;//필드 Y그리드 수
	public static readonly float BLOCK_SCALE = 2.0f;//블록 스케일(블록 1개 크기)
	public static readonly Vector3 BLOCK_OFFSET = new Vector3(1,1,1);//블록 배치 오프셋
	//배치물 종류
	public enum ObjectKind {
		Empty, Block, Player1, Player2
	}
	public static readonly int[] GRID_OBJECT_DATA = new int[]{//배치 데이터
		//0 - 공백, 1 = 블록
		1,	1,	1,	1,	1,	1,	1,	1,	1,
		1,	2,	0,	0,	0,	0,	0,	0,	1,
		1,	0,	1,	1,	1,	0,	1,	0,	1,
		1,	0,	0,	0,	0,	0,	0,	0,	1,
		1,	0,	1,	0,	1,	1,	1,	0,	1,
		1,	0,	1,	0,	1,	0,	0,	0,	1,
		1,	0,	1,	0,	0,	0,	1,	0,	1,
		1,	0,	0,	0,	1,	0,	0,	3,	1,
		1,	1,	1,	1,	1,	1,	1,	1,	1,

		//배치할때 위 아래가 반전되므로 주의할 것
	};
	private GameObject m_blockParent = null;//생성된 블록의 부모 오브젝트
	//시작시 호출되는 함수
	private void Awake() {
		//필드 초기화
		InitializeField();
	}
	//필드 초기화.
	//배열변수 초기화. 기둥과 외벽 생성
	private void InitializeField() {
		//블록 부모 생성
		m_blockParent = new GameObject();
		m_blockParent.name = "BlockParent";
		m_blockParent.transform.parent = transform;
		//블록 생성
		GameObject originalObject;//생성된 블록의 원본 오브젝트
		GameObject instanceObject;//블록을 우선적으로 넣는 변수
		Vector3 position;//블록 생성 위치
		//바깥테두리와 기둥을 세운다
		int gridX;
		int gridY;
		for(gridX=0; gridX<FIELD_GRID_X; gridX++) {
			for(gridY=0; gridY<FIELD_GRID_Y; gridY++) {
				switch((ObjectKind)GRID_OBJECT_DATA[gridX + (gridY *FIELD_GRID_X)]) {
				case ObjectKind.Block://벽
					originalObject = m_blockObject;
					break;
				case ObjectKind.Player1://플레이어
					originalObject = m_player1Object;
					break;
				case ObjectKind.Player2://플레이어
					originalObject = m_player2Object;
					break;
				default://디폴트 : 공백
					originalObject = null;
					break;
				}
				if(null == originalObject) {
					continue;
				}
				//블록 생성 위치
				position = new Vector3(gridX * BLOCK_SCALE, 0, gridY * BLOCK_SCALE) + BLOCK_OFFSET;
				//블록 생성	
				instanceObject = Instantiate(originalObject, position, originalObject.transform.rotation) as GameObject;
				//이름 변경
				instanceObject.name	= "" + originalObject.name +"("+ gridX +","+ gridY +")";//그리드 위치를 적음
				//로컬 스케일(크기)변경
				instanceObject.transform.localScale	= (Vector3.one	*BLOCK_SCALE);//Vector3.one 은 new Vector3( 1f, 1f, 1f)과 동일
				//앞의 부모 아래에 붙임
				instanceObject.transform.parent = m_blockParent.transform;
			}
		}
	}
}
