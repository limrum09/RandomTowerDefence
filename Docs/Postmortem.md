# 프로젝트 회고

## 1. 프로젝트 목표

RandomTowerDefence는 Unity 6 기반 2D 타워 디펜스 프로젝트다. 상점에서 타워를 구매해 대기열에 보관하고, 장애물과 점유 상태를 검사하는 그리드에 배치한 뒤 데이터 기반 웨이브를 진행하는 흐름을 구현했다.

전투 중에는 런 강화가 적용되고, 스테이지 밖에서는 계정 메타 성장과 저장 데이터가 유지된다. 상점 → 대기열 → 타워 배치 → 웨이브 전투 → 런 강화 → 메타 성장 → 저장으로 이어지는 각 단계를 하나의 클래스에 모으지 않고 역할별 시스템으로 나누는 것을 목표로 했다.

## 2. 최종 결과

기능 개발을 종료하고 포트폴리오 문서화 단계로 전환했다. 현재 코드와 기술 문서에서 확인되는 주요 구현 결과는 다음과 같다.

| 시스템 | 구현 결과 | 상세 문서 |
|---|---|---|
| 타워 건설 | 그리드 좌표 변환, 설치 가능 여부 검사, 타워 생성, 필드 점유 등록, 성공 후 큐 갱신 | [BuildSystem](Systems/BuildSystem.md) |
| 상점 및 대기열 | 구매와 배치 시점 분리, 골드 차감, 타워 UID 보관, 수용 실패 시 환불 | [ShopAndQueueSystem](Systems/ShopAndQueueSystem.md) |
| 웨이브 및 적 | JSON 사전 검증, 적 생성, A* 이동, 생존 적 집계, 복합 종료 조건 처리 | [WaveAndEnemySystem](Systems/WaveAndEnemySystem.md) |
| 메타 성장 | 기본 스탯, 영구 강화, 런 강화 단계를 분리한 최종 스탯 계산 | [MetaUpgradeSystem](Systems/MetaUpgradeSystem.md) |
| 저장 및 불러오기 | 로컬 옵션과 Firestore 계정 진행 분리, 결과 타입화, 데이터 검증, dirty flag 처리 | [SaveLoadSystem](Systems/SaveLoadSystem.md) |
| 퀘스트 및 업적 | ScriptableObject 기반 원본 데이터, 런타임 복제, 공통 보고 API, 완료 이벤트와 저장 | [QuestSystem](Systems/QuestSystem.md) |
| UI 정보 패널 | Presenter/View 역할 분리, 이벤트 기반 표시 갱신, 패널 전환 상태 관리 | [UIInfoPanelSystem](Systems/UIInfoPanelSystem.md) |
| Editor Tooling | Tower·Enemy·Item·Wave JSON 편집과 검증, 퀘스트 에셋 제작, Play Mode 디버깅 보조 | [EditorTooling](Systems/EditorTooling.md) |

## 3. 잘된 점

### 기능별 책임 분리

상점 구매, 대기열 보관, 타워 생성, 필드 점유를 StoreController, QueueUIController, TowerController, FieldTowerManager로 분리했다. 웨이브도 StageWaveManager, EnemySpawn, EnemyFactory, StageManager가 준비, 생성, 생명주기, 완료 판정을 나누어 담당한다.

이 구조를 통해 각 단계가 소유하는 상태와 성공 조건을 구분할 수 있었다. 직접 참조가 완전히 제거된 구조는 아니지만, 구매 실패·설치 실패·필드 등록 실패처럼 서로 다른 실패 지점을 별도로 처리할 수 있다.

### 데이터와 코드의 분리

타워, 적, 아이템, 웨이브, 강화 규칙을 Resources JSON에서 읽도록 구성했다. 밸런스와 콘텐츠 행을 런타임 로직에서 분리하고 UID 기반으로 연결했다.

JSON 직접 편집의 반복 작업은 Tower, Enemy, Item, Wave EditorWindow로 보조했다. Validate 탭에서 중복 UID, enum 변환, 누락 참조, 잘못된 수치 등을 확인하도록 구성해 데이터 기반 설계에 검증 절차를 함께 두었다.

### 이벤트 기반 연결

C# 이벤트를 사용해 적 생성·사망·도착, 타워 설치, 세션 상태, UI 입력, 퀘스트·업적 보고를 연결했다. StageManager와 StageUIController가 조정자 역할을 맡고, Presenter와 View는 표시 변환과 Unity UI 조작을 나누어 담당한다.

이벤트 발행자와 구독자의 관계는 코드만으로 추적하기 어려울 수 있어 [EventFlow](Architecture/EventFlow.md)에 발행·구독 흐름과 생명주기 해제를 함께 정리했다.

### 저장 경계와 실패 처리

기기별 입력·사운드·그래픽 옵션은 로컬 JSON에, 계정 진행·메타 성장·퀘스트 데이터는 Firebase Firestore에 저장한다. Firestore 로드는 문서 없음, 네트워크, 권한, 시간 초과, 데이터 손상을 구분하고 역직렬화 이후 IValidSaveData 검증을 수행한다.

### 개발 도구와 문서화

퀘스트 제작, 업적 확인, 스테이지·로비 진행 테스트, 조건부 Inspector 표시를 위한 Editor 확장 도구를 작성했다. 기능 개발 종료 후에는 README, Architecture, Systems, Case Study 문서로 클래스 책임과 데이터·이벤트 흐름, 현재 한계를 정리했다.

## 4. 어려웠던 점

### 구매와 실제 배치 사이의 상태 관리

상점에서 골드를 지불하는 시점과 타워가 필드에 배치되는 시점이 다르다. 골드, 큐 UID, 생성된 GameObject, 필드 셀 점유가 각각 다른 객체에 존재하므로 일부 단계만 성공한 상태를 처리해야 했다.

구매 상품이 대기열에 들어가지 못하면 골드를 환불하고, 설치에 실패하면 큐를 유지하며, 필드 등록이 실패하면 생성한 타워를 제거하도록 단계별 성공 조건을 구분했다.

### 웨이브 종료 조건

적 스폰 코루틴의 종료와 필드의 마지막 적 제거는 서로 다른 시점에 발생한다. 하나의 이벤트만 기준으로 삼으면 적이 남은 상태에서 웨이브가 끝나거나, 더 생성될 적이 있는데 생존 적 수가 0으로 판단될 수 있다.

isSpawning과 aliveEnemyCnt를 별도 상태로 관리하고 스폰 종료, 적 사망, 적 도착 시 동일한 CheckWaveEnd 조건을 다시 검사하도록 구성했다.

### 런 상태와 계정 진행 분리

스테이지 안에서만 유지되는 골드·생명·웨이브 상태와 계정에 남는 메타 재화·연구·업적은 수명주기가 다르다. RunSessionDataManager가 런 상태를 보유하고 Player·Meta·Quest 데이터와 SaveDataManager가 영구 진행을 담당하도록 경계를 나눴다.

### UI 전환과 입력 중복

정보 패널은 DOTween 이동과 Animator 효과가 연속해서 실행된다. 새 전환이 기존 Sequence와 겹치지 않도록 활성 Tween을 종료하고, CanvasGroup 입력 상태와 완료 콜백을 전환 단계에 맞춰 관리했다.

### 저장 오류 구분

원격 문서가 없는 신규 사용자와 실제 네트워크·권한 오류를 같은 실패로 처리할 수 없었다. FireStoreLoadResult 상태와 기본 데이터 생성 경로를 분리하고, 로컬 저장은 임시 파일 작성과 기존 파일 백업 후 교체하는 순서로 구성했다.

## 5. 주요 문제 해결 사례

### 구매와 배치 시점 분리

- **Problem:** 구매 성공 이후 큐 수용이나 필드 설치가 실패하면 골드와 배치 상태가 어긋날 수 있었다.
- **Solution:** 구매, UID 보관, 설치 요청, 셀 검증, 필드 등록을 별도 책임으로 나누었다.
- **Result:** 큐 수용 실패 시 골드를 환불하고, FieldTowerManager 등록 성공 시에만 설치 완료 이벤트로 큐를 제거한다.

### 웨이브 종료 조건 처리

- **Problem:** 스폰 완료와 생존 적 0명은 동시에 발생하지 않는다.
- **Solution:** isSpawning과 aliveEnemyCnt를 함께 검사하는 단일 종료 조건을 여러 종료 가능 지점에서 호출했다.
- **Result:** 스폰이 끝나고 필드에 생존 적이 없을 때만 다음 웨이브 또는 스테이지 종료 흐름으로 이동한다.

### 저장 실패와 데이터 검증

- **Problem:** 문서 누락, 연결 오류, 권한, 시간 초과, 역직렬화 이후의 잘못된 값을 구분해야 했다.
- **Solution:** FireStoreLoadResult, 제한 시간, IValidSaveData, 저장 영역별 dirty flag를 적용했다.
- **Result:** 신규 사용자 기본값 생성과 실제 오류 처리가 분리되고, 변경된 원격 저장 영역만 저장 대상이 된다.

### Editor Tooling을 통한 데이터 제작 보조

- **Problem:** 여러 JSON과 ScriptableObject를 직접 수정하면 UID, enum, 참조, 수치 입력을 반복해서 확인해야 했다.
- **Solution:** 데이터 종류별 EditorWindow, Validate 탭, 퀘스트 에셋 생성 도구, Play Mode 디버거를 구성했다.
- **Result:** Unity Editor 안에서 데이터 편집과 검증, 테스트 입력을 수행할 수 있는 제작 흐름이 마련되었다.

세부 문제 정의와 처리 순서는 [Case Study](Portfolio/CaseStudy.md)에서 확인할 수 있다.

## 6. 아쉬운 점

- StageManager와 TowerController에 여러 흐름의 조정 책임이 집중되어 있어 기능이 증가하면 변경 범위가 커질 수 있다.
- Enemy와 Tower의 Instantiate/Destroy, 적별 A* 경로 계산, TowerAttack의 범위 탐색이 실제 병목인지 확인할 프로파일러 수치가 없다.
- 저장 모델에 schemaVersion과 명시적인 마이그레이션 정책이 없다.
- 종료 시 저장 외에 체크포인트, 재시도 큐, 오프라인 복구 정책을 더 구체화할 수 있다.
- 현재 저장소에서 자동화 테스트 파일은 확인되지 않았다. 그리드 검증, 큐 유지, 웨이브 종료, 저장 모델 검증을 자동화할 여지가 있다.
- Editor Tool의 Undo/Redo 지원 범위와 변경 이력 관리가 확인되지 않았고, 검증 결과를 통합 리포트로 내보내는 기능은 없다.
- 플레이 영상, GIF, 주요 화면 스크린샷이 아직 공개 문서에 추가되지 않았다.

## 7. 다시 개발한다면

1. StageFlow, BuildFlow, WaveFlow처럼 흐름별 조정 객체를 두어 StageManager와 TowerController의 책임을 나눈다.
2. Unity Profiler로 생성·파괴와 경로 탐색 비용을 먼저 측정한 뒤 Object Pool 적용 여부를 결정한다.
3. TowerAttack의 OverlapCircleAll을 NonAlloc API 또는 적 공간 인덱스 구조와 비교한다.
4. 저장 데이터에 schemaVersion을 추가하고 버전별 마이그레이션과 실패 복구 정책을 정의한다.
5. 건설 가능 셀 판정, 대기열 유지, 웨이브 완료 조건, 저장 모델 유효성 같은 핵심 규칙부터 자동화 테스트를 작성한다.
6. 데이터별 EditorWindow의 공통 저장·검색·검증 기능을 추출하고, 여러 데이터의 참조 오류를 한 번에 확인하는 통합 검증 도구를 검토한다.

## 8. 기술적 부채

| 항목 | 현재 구조의 한계 | 개선 방향 |
|---|---|---|
| Manager 의존성 | Managers와 StageManager에 서비스 접근과 흐름 조정이 집중됨 | 명시적 의존성 주입과 흐름별 조정 객체 검토 |
| 데이터 경로 | Resources 문자열 경로와 프로젝트 내부 JSON 구조에 의존함 | 강타입 ID, 설정 객체, Addressables 또는 별도 데이터 파이프라인 비교 |
| 생성 비용 | Enemy와 Tower가 Instantiate/Destroy를 사용함 | 프로파일러 측정 후 Object Pool 적용 판단 |
| 탐색 비용 | 적별 A*와 TowerAttack 범위 탐색 비용의 측정 자료가 없음 | 경로 캐시, PriorityQueue, NonAlloc 또는 공간 인덱스 비교 |
| 저장 복구 | 타임아웃과 오류 구분은 있으나 재시도·마이그레이션 정책을 확장할 수 있음 | 체크포인트, 재시도 큐, schemaVersion과 마이그레이션 정의 |
| Editor Tool | 프로젝트 데이터 구조에 강하게 결합되고 Undo/Redo·변경 이력·일괄 리포트 범위가 제한적임 | 공통 편집 기반과 통합 검증 리포트 검토 |
| 자동화 검증 | 현재 저장소에서 자동화 테스트 파일이 확인되지 않음 | 순수 규칙과 실패 경로부터 테스트 가능한 단위로 분리 |

## 9. 배운 점

- **상태 소유자와 조정자의 분리:** 데이터를 가진 객체와 여러 시스템을 연결하는 객체의 역할을 구분해야 실패 지점과 변경 범위를 설명할 수 있다.
- **이벤트와 문서화:** 이벤트 기반 구조는 직접 참조를 줄일 수 있지만, 발행자·구독자·해제 시점을 함께 기록해야 전체 흐름을 추적할 수 있다.
- **데이터와 검증 도구:** 데이터 기반 설계는 파일 분리만으로 끝나지 않는다. UID, enum, 참조, 수치 범위를 확인하는 제작 도구가 함께 있어야 안정적으로 운영할 수 있다.
- **저장 실패 흐름:** 정상 저장뿐 아니라 문서 없음, 네트워크, 권한, 시간 초과, 손상 데이터와 부분 변경 상태를 먼저 정의해야 한다.
- **UI 책임 분리:** View, Presenter, Controller의 역할을 나누면 표시 변환, Unity UI 조작, 게임 명령 전달 위치가 명확해진다.
- **포트폴리오 문서화:** 기능 목록뿐 아니라 해결하려던 문제, 선택한 구조, 실제 처리 순서, 검증되지 않은 부분과 개선 방향을 함께 제시해야 기술적 판단을 전달할 수 있다.

## 10. 포트폴리오 마무리 메시지

RandomTowerDefence는 Unity 6 기반 타워 디펜스의 플레이 흐름과 함께 데이터 기반 콘텐츠 관리, 이벤트 기반 런타임 연결, 저장 실패 처리, UI 책임 분리, Unity Editor 제작 도구를 구현하고 문서화한 프로젝트다. 구현 결과뿐 아니라 현재 구조의 한계와 다음 검증 항목까지 정리함으로써 시스템을 나누고 연결하는 과정에서의 기술적 판단을 보여 준다.

## 남은 TODO

- 플레이 영상 추가
- 상점 → 대기열 → 타워 설치 GIF 추가
- 웨이브 전투 GIF 추가
- 메타 성장 UI GIF 추가
- 주요 화면 스크린샷 추가
- 성능 측정 자료가 필요하면 프로파일러 결과 추가
