# 시스템 아키텍처

## 1. 문서 목적

본 문서는 **RandomTowerDefence**의 런타임 구조와 시스템 간 책임 경계를 정리한다. 프로젝트는 Unity 6 기반 2D 타워 디펜스로, 전역 서비스·정적 게임 데이터·스테이지 런타임·UI·영구 저장을 분리하고 이벤트로 연결한다.

## 2. 기능 목적

- 로비와 스테이지가 공유하는 데이터 및 저장 기능을 일관된 진입점으로 제공한다.
- 웨이브, 타워, 적, 아이템처럼 수명주기가 다른 시스템을 스테이지 단위로 조립한다.
- JSON 콘텐츠를 코드 수정 없이 교체할 수 있는 런타임 모델로 변환한다.
- UI가 게임 규칙을 직접 소유하지 않도록 Presenter와 이벤트 경계를 둔다.
- 로컬 옵션과 Firebase 영구 진행 데이터를 서로 다른 저장 정책으로 관리한다.

## 3. 해결 대상

기능이 증가하면서 MonoBehaviour 간 직접 참조만으로는 다음 문제를 관리하기 어려워진다.

- 씬 전환 때 공용 데이터와 서비스가 중복 생성될 수 있다.
- UI가 골드 차감, 타워 등록, 웨이브 판정까지 수행하면 규칙이 여러 클래스에 분산된다.
- 콘텐츠를 코드에 하드코딩하면 밸런스 수정마다 재컴파일이 필요하다.
- 전투 중 상태와 계정 영구 상태의 저장 범위가 섞일 수 있다.
- 시스템 호출 순서가 암묵적이면 초기화 오류와 이벤트 누락을 추적하기 어렵다.

## 4. 전체 구조

~~~mermaid
graph TD
    Scene[Unity Scene] --> Bootstrap[Managers / LoadSceneManager]
    Bootstrap --> StaticData[JSON Data Managers]
    Bootstrap --> GlobalState[Player / Meta / Quest / Save]
    Scene --> Stage[StageManager]
    Stage --> Session[RunSessionDataManager]
    Stage --> Grid[GridManager / PathFinder]
    Stage --> Combat[EnemySpawn / EnemyFactory / TowerController]
    Stage --> RunEffects[RunStatUpgradeManager / TowerSkillEffect]
    StaticData --> Stage
    GlobalState --> Stage
    Stage <--> UI[StageUIController / Presenters / Views]
    GlobalState <--> Cloud[SaveDataManager / Firestore]
    Bootstrap --> Local[Sound / Graphic / Input Local JSON]
~~~

## 5. 계층별 책임

| 계층 | 주요 클래스 | 책임 |
|---|---|---|
| 부트스트랩 | [Managers](../../Assets/02.Scripts/Managers/Core/Managers.cs), [LoadSceneManager](../../Assets/02.Scripts/Managers/Core/LoadSceneManager.cs) | 전역 서비스 생성, 데이터 초기화, 씬 준비 상태 통지 |
| 정적 데이터 | [TowerDataManager](../../Assets/02.Scripts/Managers/Data/Tower/TowerDataManager.cs), [WaveDataManager](../../Assets/02.Scripts/Managers/Data/Wave/WaveDataManager.cs) | Resources JSON을 런타임 객체와 UID 인덱스로 변환 |
| 영구 진행 | [PlayerProgressManager](../../Assets/02.Scripts/Managers/Data/Meta/PlayerProgressManager.cs), [TowerMetaUpgradeManager](../../Assets/02.Scripts/Managers/Meta/TowerMetaUpgradeManager.cs), [QuestManager](../../Assets/02.Scripts/Managers/Core/QuestManager.cs) | 계정 단위 진행 상태와 변경 규칙 관리 |
| 스테이지 조정 | [StageManager](../../Assets/02.Scripts/Managers/Stage/StageManager.cs) | 런타임 시스템 생성, 이벤트 연결, 웨이브·보상·종료 흐름 조정 |
| 전투 도메인 | [FieldTowerManager](../../Assets/02.Scripts/Managers/Stage/FieldTowerManager.cs), [EnemyFactory](../../Assets/02.Scripts/Stage/Enemy/EnemyFactory.cs), [PathFinder](../../Assets/02.Scripts/Stage/Path/PathFinder.cs) | 타워 점유 상태, 적 생성, 경로 계산 |
| 프레젠테이션 | [StageUIController](../../Assets/02.Scripts/UI/Controllers/Stage/StageUIController.cs), Presenter, View | 입력 전달, 표시 모델 변환, 패널 전환 |
| 인프라 | [SaveDataManager](../../Assets/02.Scripts/Managers/Data/Save/SaveDataManager.cs), [FirestoreSaveRepository](../../Assets/02.Scripts/Firebase/Save/FirestoreSaveRepository.cs) | 로컬·원격 직렬화, 시간 초과, 데이터 검증, dirty 저장 |

## 6. 클래스 관계

~~~mermaid
classDiagram
    class Managers
    class StageManager
    class StageUIController
    class TowerController
    class EnemySpawn
    class StageWaveManager
    class RunSessionDataManager
    class SaveDataManager

    Managers --> SaveDataManager
    Managers --> StageManager : static data/state
    StageManager *-- RunSessionDataManager
    StageManager *-- StageWaveManager
    StageManager --> EnemySpawn
    StageManager --> StageUIController
    StageUIController --> TowerController
    TowerController --> StageManager
~~~

**Managers**는 전역 서비스 접근점이고, **StageManager**는 스테이지 범위 객체의 조정자다. 실제 상태 저장은 RunSessionDataManager, FieldTowerManager 등 전용 객체가 담당하고 StageManager는 이벤트를 연결한다.

## 7. 초기화와 생명주기

~~~mermaid
sequenceDiagram
    participant M as Managers
    participant D as Data Managers
    participant S as SaveDataManager
    participant L as LoadSceneManager
    participant G as StageManager
    participant U as StageUIController

    M->>D: Init()
    M->>S: LoadLocalOptionData()
    alt 로그인 사용자
        M->>S: LoadAllData()
    else 비로그인 사용자
        M->>D: 기본 진행 데이터 적용
    end
    M->>L: NotifyDataLoaded()
    G->>G: 런타임 매니저 생성 및 이벤트 연결
    U->>L: NotifySceneUIReady()
    G->>L: NotifySceneManagerReady()
~~~

- Managers는 DontDestroyOnLoad 싱글 인스턴스로 유지된다.
- 정적 데이터 매니저는 Managers.Awake에서 한 번 초기화된다.
- 스테이지 전용 객체는 StageManager.Awake에서 생성되고 씬 종료 시 이벤트를 해제한다.
- UI는 Presenter 생성과 바인딩을 마친 뒤 씬 준비 완료를 통지한다.

## 8. 사용한 디자인

| 디자인 | 적용 위치 | 의도 |
|---|---|---|
| Service Locator | Managers | 씬과 일반 C# 객체에서 공용 서비스 접근 통일 |
| Singleton | Managers, LoadSceneManager, FirebaseInitializer | 전역 수명주기 보장 |
| Mediator / Facade | StageManager, StageUIController | 다수 하위 시스템의 상호작용 집중 |
| Observer | C# event Action | UI·전투·퀘스트 상태 변경 전파 |
| Factory | EnemyFactory | 적 생성과 초기화 절차 캡슐화 |
| Repository | FirestoreSaveRepository | Firebase SDK 결과를 도메인 결과 형식으로 변환 |
| MVP | Stage Presenter/View 계층 | 표시 변환과 View 조작 분리 |
| Data-Driven | Resources JSON + DataManager | 콘텐츠와 실행 로직 분리 |
| Dirty Flag | SaveDataManager | 변경된 저장 영역만 기록 |

## 9. 핵심 구현

전역 접근은 프로퍼티를 통해 단일 인스턴스의 서비스로 연결된다.

~~~csharp
public static TowerDataManager TowerData => Instance.tower;
public static SaveDataManager Save => Instance.saveDataManager;
public static QuestManager QuestMgr => Instance.quest;
~~~

스테이지는 하위 런타임 객체를 직접 생성한 뒤 필요한 참조만 주입한다.

~~~csharp
Grid = new GridManager();
Path = new PathFinder();
sessionManager = new RunSessionDataManager();
fieldTowerManager = new FieldTowerManager();

Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);
fieldTowerManager.Init(Grid);
Path.Init(Grid);
~~~

Unity 컴포넌트가 아닌 규칙 객체를 일반 C# 객체로 생성해 씬 오브젝트 수를 줄이고, 스테이지 상태가 전역 상태로 누출되는 것을 막는다.

## 10. 성능 고려 사항

- 타워·웨이브·적 데이터는 초기화 시 Dictionary로 인덱싱한다.
- 전투 효과는 PoolManager를 통해 재사용한다.
- 이벤트 기반 UI 갱신으로 매 프레임 전체 UI를 폴링하지 않는다.
- 로컬 저장은 dirty flag가 설정된 항목만 직렬화한다.
- 현재 적 본체와 타워는 Instantiate/Destroy를 사용하므로 대량 생성 구간에서 GC 및 메인 스레드 스파이크가 발생할 수 있다.
- StageManager와 TowerController의 책임이 커 호출 경로 추적 비용이 증가한다.

## 11. 개선 가능성

1. Managers를 인터페이스 기반 Composition Root로 대체해 테스트 대역 주입을 가능하게 한다.
2. StageManager를 WaveFlow, Reward, StageResult, RuntimeComposition으로 분리한다.
3. 적과 타워에도 풀링을 적용하고 공통 경로를 캐싱한다.
4. 이벤트 구독을 명시적 Disposable 또는 EventBus 채널로 관리한다.
5. 저장 모델에 schemaVersion과 마이그레이션 단계를 추가한다.
6. 데이터 검증을 에디터 단계와 CI에서 자동 실행한다.

## 12. 포트폴리오 핵심

- 187개 직접 작성 스크립트 규모에서 **전역 서비스와 스테이지 범위 상태를 분리**했다.
- 데이터, 전투, UI, 저장에 서로 다른 패턴을 적용하고 StageManager에서 실행 흐름을 조합했다.
- Service Locator 의존성과 대형 조정자 클래스의 한계까지 기술적 트레이드오프로 설명할 수 있다.
