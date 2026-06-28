# Editor Tooling

## 1. 개요

Assets/02.Scripts/Editor 아래의 Unity Editor 확장 도구는 반복적인 JSON 데이터 제작, ScriptableObject 생성, 데이터 검증, Play Mode 디버깅을 보조한다. 런타임 시스템과 별도로 EditorWindow, CustomEditor, CustomPropertyDrawer를 사용해 프로젝트 데이터와 테스트 상태를 다룬다.

## 2. 구현 목적

- JSON 직접 수정 과정에서 발생할 수 있는 UID, enum, 참조, 수치 입력 오류를 줄인다.
- Tower, Enemy, Item, Wave 데이터를 EditorWindow에서 확인하고 수정한다.
- Quest, Target, Condition ScriptableObject 에셋 생성을 도구화한다.
- Play Mode에서 업적, 스테이지, 로비 진행 상태를 빠르게 테스트한다.
- 조건부 Inspector 표시와 활성 상태를 CustomPropertyDrawer로 제어한다.

## 3. 주요 Editor 도구

| 도구 | 실제 파일 위치 | 역할 |
|---|---|---|
| TowerTableEditorWindow | [Assets/02.Scripts/Editor/Tower/TowerTableEditorWindow.cs](../../Assets/02.Scripts/Editor/Tower/TowerTableEditorWindow.cs) | Tower, Tower Skill, Session Upgrade JSON 편집과 검증 |
| EnemyTableEditorWindow | [Assets/02.Scripts/Editor/Enemy/EnemyTableEditorWindow.cs](../../Assets/02.Scripts/Editor/Enemy/EnemyTableEditorWindow.cs) | Enemy와 Enemy Skill JSON 편집 및 런타임 공식 미리보기 |
| ItemTableEditorWindow | [Assets/02.Scripts/Editor/Item/ItemTableEditorWindow.cs](../../Assets/02.Scripts/Editor/Item/ItemTableEditorWindow.cs) | Item JSON 편집, 아이콘·런타임 미리보기, 검증 |
| WaveEnemyRosterEditorWindow | [Assets/02.Scripts/Editor/Wave/WaveEnemyRosterEditorWindow.cs](../../Assets/02.Scripts/Editor/Wave/WaveEnemyRosterEditorWindow.cs) | 난이도별 웨이브 로스터 편집과 참조·수치 검증 |
| QuestEditorWindow | [Assets/02.Scripts/Editor/QuestEditorWindow.cs](../../Assets/02.Scripts/Editor/QuestEditorWindow.cs) | Quest, Target, Condition 에셋 생성·검색·편집·삭제 |
| QuestCreatePopupWindow | [Assets/02.Scripts/Editor/QuestCreatePopupWindow.cs](../../Assets/02.Scripts/Editor/QuestCreatePopupWindow.cs) | 새 퀘스트 관련 에셋 이름 입력과 생성 콜백 호출 |
| AchievementDebuggerWindow | [Assets/02.Scripts/Editor/AchievementDebuggerWindow.cs](../../Assets/02.Scripts/Editor/AchievementDebuggerWindow.cs) | Play Mode 업적 목록 조회와 테스트 완료 처리 |
| StageSceneDebuggerEditor | [Assets/02.Scripts/Editor/Debuggers/StageSceneDebuggerEditor.cs](../../Assets/02.Scripts/Editor/Debuggers/StageSceneDebuggerEditor.cs) | StageSceneDebugger의 테스트용 Custom Inspector |
| LobbyProgressDebuggerEditor | [Assets/02.Scripts/Editor/Debuggers/LobbyProgressDebuggerEditor.cs](../../Assets/02.Scripts/Editor/Debuggers/LobbyProgressDebuggerEditor.cs) | LobbyProgressDebugger의 메타 진행 테스트용 Custom Inspector |
| ConditionalFieldDrawer | [Assets/02.Scripts/Editor/ConditionalFieldDrawer.cs](../../Assets/02.Scripts/Editor/ConditionalFieldDrawer.cs) | ConditionFieldAtrribute 조건에 따른 Inspector 필드 제어 |
| AnimatorDatas | [Assets/02.Scripts/Editor/AnimatorDatas.cs](../../Assets/02.Scripts/Editor/AnimatorDatas.cs) | AnimatorController 목록을 갱신하는 Editor-assisted asset utility |

## 4. 데이터 제작 흐름

### Tower 데이터

TowerTableEditorWindow는 Tools/Tower Table Editor 메뉴로 연다. Tower, Tower Skill, Session Upgrade, Validate 탭에서 다음 JSON을 편집한다.

- Assets/Resources/Data/TowerData.json
- Assets/Resources/Data/TowerSkillData.json
- Assets/Resources/Data/TowerSessionUpgradeData.json

Add, Duplicate, Delete, Save, Auto Save, 검색을 제공한다. Tower 아이콘 Preview와 Tower·Skill·Session Upgrade의 Runtime Preview를 표시하며 Validate 탭에서 데이터 관계와 입력값을 검사한다.

### Enemy 데이터

EnemyTableEditorWindow는 Tools/Enemy Table Editor 메뉴로 연다. Enemy와 Skill 탭에서 EnemyData.json과 EnemySkillData.json을 편집한다.

Enemy와 Skill 행에 대해 Add, Duplicate, Delete, Save, Auto Save, 검색을 제공한다. Runtime Formula Preview에서 레벨과 수치 계산식을 확인한다.

### Item 데이터

ItemTableEditorWindow는 Tools/Item Table Editor 메뉴로 연다. Item과 Validate 탭에서 ItemData.json을 편집한다.

Add, Duplicate, Delete, Auto Save, 검색, 아이콘 Preview, Runtime Preview를 제공한다. Validate 탭에서는 중복 UID, enum 파싱 실패, 아이콘 누락, 음수 구매·판매 가격, 적용·제거 처리 누락 가능성을 검사한다.

### Wave 로스터 데이터

WaveEnemyRosterEditorWindow는 Tools/Wave Enemy Roster Editor 메뉴로 연다. Easy, Normal, Hard, Hell, Validate 탭을 제공하며 WaveData, WaveEnemyRosterData, EnemyData를 로드한다.

난이도별 웨이브를 선택해 Add Roster, Duplicate, Delete, Sort Wave, Auto Save, 검색을 수행한다. Validate 탭에서는 난이도별 웨이브와 로스터 연결, 존재하지 않는 Enemy UID, 잘못된 생성 순서·수량·레벨·시작 시간·간격을 검사한다.

### Quest 에셋

QuestEditorWindow는 Tools/Quest Editor 메뉴로 연다. Quest, Target, Condition 탭에서 Quest와 Achievement 에셋을 생성·검색·편집·삭제한다.

Target 탭에서는 EnemyTarget과 UIDTarget을 생성하고, Condition 탭에서는 IsAchievementCompleteCondition을 생성한다. QuestCreatePopupWindow는 새 에셋 이름이 비어 있는지 검사한 뒤 생성 콜백을 호출한다. 이 흐름은 ScriptableObject 기반 퀘스트 제작을 보조한다.

## 5. 검증과 디버깅

### 데이터 검증

- TowerTableEditorWindow의 Validate 탭은 중복 UID, enum 변환, Skill과 NextGrade 참조, Session Upgrade와 Tower 연결, 등급별 데이터 누락을 검사한다.
- ItemTableEditorWindow의 Validate 탭은 중복 UID, enum 변환, 아이콘, 가격, 적용·제거 처리 범위를 검사한다.
- WaveEnemyRosterEditorWindow의 Validate 탭은 웨이브·로스터 연결, 난이도 구분, Enemy UID, 생성 순서·수량·레벨·시간 값을 검사한다.

### Play Mode 디버깅

- AchievementDebuggerWindow는 Tools/Achievement Debugger 메뉴로 여는 Play Mode 전용 도구다. Active Achievements와 Complete Achievements를 검색·새로고침하고, 활성 업적에 테스트용 Complete 처리를 실행할 수 있다.
- StageSceneDebuggerEditor는 StageSceneDebugger의 Custom Inspector다. 난이도와 웨이브 적용, 골드·생명·무료 장애물 추가 버튼을 제공한다.
- LobbyProgressDebuggerEditor는 LobbyProgressDebugger의 Custom Inspector다. 메타 재화와 경험치를 테스트용으로 추가하는 버튼을 제공한다.

## 6. Inspector 보조 기능

ConditionalFieldDrawer는 ConditionFieldAtrribute를 위한 CustomPropertyDrawer다. Boolean, ObjectReference, String, Integer, Enum 조건 필드를 평가한다.

- ShowIf는 조건이 참일 때 필드를 표시하고 HideIf는 조건이 참일 때 필드를 숨기는 용도로 사용한다.
- EnableIf는 조건이 참일 때 필드를 활성화하고 DisableIf는 조건이 참일 때 필드를 비활성화하는 용도로 사용한다.
- 조건에 맞지 않는 불필요한 필드를 복잡한 데이터 에셋에서 줄여 Inspector 가독성을 높이고 입력 실수를 줄이는 것이 목적이다.

AnimatorDatas는 같은 Editor 폴더에 있는 보조 ScriptableObject다. RefreshAnimators ContextMenu에서 AssetDatabase.FindAssets("t:AnimatorController")로 AnimatorController를 검색하고, 이름이 Ani로 끝나는 컨트롤러 목록을 갱신한다.

## 7. 설계 의도

반복적인 JSON 수정, ScriptableObject 생성, Play Mode 테스트를 Unity Editor 도구로 보조해 수작업 입력 오류를 줄이고 확인 절차를 단축하는 방향으로 구성했다.

데이터별 EditorWindow는 각 JSON 구조와 런타임 계산 규칙을 직접 사용한다. 퀘스트 제작은 에셋 유형별 생성 경로를 제공하고, 디버거는 런타임 관리자에 테스트 입력을 전달하며, Inspector 보조 기능은 데이터 입력 시점의 표시 상태를 제어한다.

## 8. 한계와 개선 가능성

- 일부 도구는 프로젝트 내부 JSON Row, enum, Resources 경로, 런타임 관리자 구조에 강하게 의존한다.
- TODO: 확인 필요 — Undo/Redo 지원 범위
- 변경 이력, 검증 결과 내보내기와 같은 일괄 검증 리포트, 데이터 간 참조 검증 범위는 확장할 수 있다.
- Addressables 또는 별도 데이터 파이프라인을 도입할 경우 고정 경로와 Resources 조회 방식을 함께 개선할 필요가 있다.
