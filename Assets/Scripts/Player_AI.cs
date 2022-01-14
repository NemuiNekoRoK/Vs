using UnityEngine;
using System.Collections;
//플레이어클래스 AI가 조작
public class Player_AI : Player_Base {
	//검사 방향
	private	enum	CheckDir {
		//← ↑ → ↓ 순
		Left, Up, Right, Down, EnumMax
	}
	//검사 정보
	private	enum	CheckData {
		X, Y, EnumMax
	}
	private static readonly int[][] CHECK_DIR_LIST = new int[(int)CheckDir.EnumMax][]{//검사 방향
		//								   X  Y
		new int[ (int)CheckData.EnumMax] {-1, 0},
		new int[ (int)CheckData.EnumMax] {0, 1},
		new int[ (int)CheckData.EnumMax] {1, 0},
		new int[ (int)CheckData.EnumMax] {0, -1}
	};
	private static readonly int AI_PRIO_MIN = 99;//AI우선도 최소값
	private static readonly float AI_INTERVAL_MIN = 0.5f;//AI사고간격 최단
	private static readonly float AI_INTERVAL_MAX = 0.8f;//AI사고간격 최장
	private static readonly float AI_IGNORE_DISTANCE = 2.0f;//플레이어에게 더이상 다가가지 않음
	private static readonly float SHOOT_INTERVAL= 1.0f;//발사 간격

	private float m_aiInterval = 0f;//AI사고 갱신 시간
	private float m_shootInterval = 0f;//발사 간격
	private PlayerInput m_pressInput = PlayerInput.Move_Left;//AI의 입력 종류
	//입력 처리 검사
	protected override void GetInput(){
		//유저가 조종하는 플레이어의 오브젝트를 얻음
		GameObject mainObject = Player_Key.m_mainPlayer;
		if(null == mainObject) {
			//플레이어가 없으면 사고 중지
			return;
		}
		//AI사고 갱신 시간
		m_aiInterval -= Time.deltaTime;
		//사격 사고 갱신 시간
		m_shootInterval -= Time.deltaTime;
		//플레이어와 자신의 거리 계산
		Vector3 aiSubPosition = (transform.position	-mainObject.transform.position);
		aiSubPosition.y = 0f;
		//거리가 생기면 움직임
		if( aiSubPosition.magnitude > AI_IGNORE_DISTANCE) {
			//일정 시간마다 AI 갱신
			if(m_aiInterval < 0f) {
				//다음 사고까지 기다릴 시간. 랜덤
				m_aiInterval = Random.Range(AI_INTERVAL_MIN, AI_INTERVAL_MAX);
				//AI의 현위치로부터 우선순위를 얻음
				int[] prioTable = GetMovePrioTable();
				//우선순위가 가장 높은 방향을 가져옴
				int highest = AI_PRIO_MIN;
				int i;
				for(i=0; i<(int)CheckDir.EnumMax; i++) {
					//값이 작을수록 우선순위가 높음
					if( highest > prioTable[ i]) {
						//우선순위 갱신
						highest		= prioTable[ i];
					}
				}
				//어느 방향 우선순위가 높은지를 결정
				PlayerInput	pressInput = PlayerInput.Move_Left;
				if(highest == prioTable[(int)CheckDir.Left]) {
					pressInput = PlayerInput.Move_Left;
				} else
				if(highest == prioTable[(int)CheckDir.Right]) {
					pressInput = PlayerInput.Move_Right;
				} else
				if(highest == prioTable[(int)CheckDir.Up]) {
					pressInput = PlayerInput.Move_Up;
				} else
				if(highest == prioTable[(int)CheckDir.Down]) {
					pressInput = PlayerInput.Move_Down;
				}
				m_pressInput = pressInput;
			}
			//입력
			m_playerInput[(int)m_pressInput] = true;
		}
		//사격 사고를 실지할지 판단
		if(m_shootInterval < 0f) {
			//X 혹은 Z 방향의 거리가 가까울 경우 직선에 있다고 판단하면 사격
			if( (Mathf.Abs( aiSubPosition.x) < 1f) || (Mathf.Abs( aiSubPosition.z) < 1f)) {
				m_playerInput[ (int)PlayerInput.Shoot]	= true;
				//다음 사격 대기시간(연속발사 방지)
				m_shootInterval	= SHOOT_INTERVAL;
			}
		}
	}
	//위치를 그리드로 변환. 그리드X
	private int GetGridX(float posX) {
		//그리드의 범위 안에 들어오도록 Mathf.Clamp로 제한
		return Mathf.Clamp((int)((posX) / Field.BLOCK_SCALE),0,(Field.FIELD_GRID_X -1));
	}
	//위치를 그리드로 변환. 그리드Y
	private int GetGridY(float posZ) {
		//유니티에서 XZ 평면은 지평선이다.
		return	Mathf.Clamp((int)((posZ) / Field.BLOCK_SCALE),0,(Field.FIELD_GRID_Y -1));
	}
	//AI가 이동할 때의 우선순위 가져오기
	private int[] GetMovePrioTable() {
		int	i, j;
		//AI 위치
		Vector3 aiPosition = transform.position;
		//그리드로 변환
		int aiX = GetGridX(aiPosition.x);
		int aiY = GetGridY(aiPosition.z);
		//유저가 조종하고 있는 플레이어 오브젝트를 얻어옴
		GameObject mainObject = Player_Key.m_mainPlayer;
		//곡격 목표 위치를 얻어옴
		Vector3 playerPosition	= mainObject.transform.position;
		//그리드 변환
		int playerX = GetGridX(playerPosition.x);
		int playerY = GetGridY(playerPosition.z);
		int playerGrid = playerX + (playerY *Field.FIELD_GRID_X);

		//그리드의 각 위치 우선순위를 저장하는 배열
		int[] calcGrid = new int[(Field.FIELD_GRID_X *Field.FIELD_GRID_Y)];
		//초기화
		for(i=0; i<(Field.FIELD_GRID_X *Field.FIELD_GRID_Y); i++) {
			//우선순위를 최소로 설정
			calcGrid[i] = AI_PRIO_MIN;
		}
		//플레이어의 위치에 1대입
		calcGrid[playerGrid] = 1;
		//우선순위 검사. 1부터 시행한다
		int checkPrio = 1;
		//검사용 변수
		int checkX;
		int checkY;
		int tempX;
		int tempY;
		int tempGrid;
		//검사시에는 true로 지정
		bool update;
		do {
			//초기화
			update = false;
			//검사 시작
			for(i=0; i<(Field.FIELD_GRID_X *Field.FIELD_GRID_Y); i++) {
				//검사 우선순위가 아니면 무시
				if(checkPrio!=calcGrid[i]) {
					continue;
				}
				//이 그리드가 검사 우선순위를 나타 낸다
				checkX	= (i % Field.FIELD_GRID_X);
				checkY	= (i / Field.FIELD_GRID_X);

				//상하좌우 검사
				for(j=0; j<(int)CheckDir.EnumMax; j++) {
					//조사할 곳의 인접한 곳
					tempX = (checkX + CHECK_DIR_LIST[j][(int)CheckData.X]);
					tempY = (checkY + CHECK_DIR_LIST[j][(int)CheckData.Y]);
					//그리드 밖인지 여부
					if((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y)) {
						//그리드 밖이므로 무시
						continue;
					}
					//조사
					tempGrid = (tempX +(tempY *Field.FIELD_GRID_X));
					//인접한 벽인지 검사
					if( Field.ObjectKind.Block==(Field.ObjectKind)Field.GRID_OBJECT_DATA[ tempGrid]) {
						//벽이라면 무시
						continue;
					}

					//이곳의 우선순위가 현재 검사하는 곳의 우선순위 보다 높으면 갱신
					if(calcGrid[ tempGrid] > (checkPrio +1)) {
						//값 갱신
						calcGrid[ tempGrid] = (checkPrio +1);//다음 검사에서의 우선순위
						update = true;//플래그를 true로 지정
					}
				}
			}

			//검사 우선순위 +1
			checkPrio++;
			//갱신되었다면 루프를 다시 돈다
		} while(update);
		//AI의 주변 우선순위 테이블
		int[] prioTable = new int[(int)CheckDir.EnumMax];
		//우선순위 테이블을 작성했으면 AI 주변의 우선순위를 가져옴
		for(i=0; i<(int)CheckDir.EnumMax; i++) {
			//조사할 곳에서 인접한 곳
			tempX = (aiX + CHECK_DIR_LIST[i][(int)CheckData.X]);
			tempY = (aiY + CHECK_DIR_LIST[i][(int)CheckData.Y]);
			//영역 밖인지 체크
			if((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y)) {
				//영역 외부이므로 우선순위를 최소화
				prioTable[i] = AI_PRIO_MIN;
				continue;
			}
			//이곳의 우선순위 대압
			tempGrid = (tempX + (tempY * Field.FIELD_GRID_X));
			prioTable[i] = calcGrid[tempGrid];
		}
		//우선순위 테이블을 로그로 출력
		{
			//디버그용 문자열
			string temp = "";

			//우선순위 테이블을 작성 했으면 AI 주변의 우선순위를 가져옴
			temp += "PRIO TABLE\n";
			for(tempY=0; tempY<Field.FIELD_GRID_Y; tempY++) {
				for(tempX=0; tempX<Field.FIELD_GRID_X; tempX++) {
					//Y축은 위아래가 거꾸로 출력. 미리 뒤집어 둔다
					temp += "\t\t" + calcGrid[tempX +((Field.FIELD_GRID_Y -1 -tempY) * Field.FIELD_GRID_X)] +"";
					//자신의 위치
					if((aiX==tempX) && (aiY==(Field.FIELD_GRID_Y -1 -tempY))) {
						temp += "*";
					}
				}
				temp += "\n";
			}
			temp += "\n";
			//이동방향별 우선순위 정보
			temp += "RESULT\n";
			for(i=0; i<(int)CheckDir.EnumMax; i++) {
				//이곳의 우선순위 대입
				temp += "\t"+ prioTable[i] +"\t"+ (CheckDir)i +"\n";
			}
			//출력
			Debug.Log( ""+ temp);
		}
		//4방향 우선순위 반환
		return prioTable;
	}
}
