# RandomTowerDefence 프로젝트 요약

> Unity 6 기반 2D 타워 디펜스 프로젝트입니다.  
> 전역 서비스, 스테이지 런타임, 데이터, UI, 영구 저장을 분리하고 C# 이벤트로 연결했습니다.  
> 기능 개발은 종료했으며 현재는 포트폴리오 문서화 단계입니다.

## 1. 한눈에 보는 프로젝트

- 상점에서 타워를 구매해 대기열에 보관하고, 그리드의 유효한 셀에 배치합니다.
- 웨이브와 적 로스터를 JSON 데이터로 관리합니다.
- 적 생성·사망·도착과 타워 설치·강화를 이벤트로 스테이지 상태와 UI에 전달합니다.
- 런 진행과 계정 영구 성장을 분리했습니다.
- 로컬 옵션은 JSON, 계정 진행은 Firebase Firestore에 저장합니다.

## 2. 프로젝트 기본 정보

| 항목 | 내용 |
|---|---|
| 장르 | 2D 타워 디펜스 |
| 엔진 | Unity 6 |
| 언어 | C# |
| 주요 기술 | Firebase Authentication·Firestore, JSON 데이터, DOTween, Unity UI |
| 개발 상태 | 기능 개발 종료, 포트폴리오 정리 단계 |
| 개발 기간 | TODO: 확인 필요 |
| 개발 인원 | TODO: 확인 필요 — 1인 개발 또는 팀 프로젝트 여부 |
| 대상 플랫폼 | TODO: 확인 필요 |

## 3. 핵심 게임 흐름

~~~mermaid
flowchart LR
    Store[상점 상품 구매] --> Queue[타워 대기열]
    Queue --> Build[그리드 배치]
    Build --> Wave[웨이브 전투]
    Wave --> Reward[골드·경험치·결과 보상]
    Reward --> Upgrade[런 강화·메타 성장]
    Upgrade --> Wave
~~~

상점, 대기열, 건설, 전투, 성장 시스템이 하나의 런 흐름으로 연결됩니다. 각 시스템의 상태 소유자는 분리하고, 변경 결과는 이벤트 또는 명시적 반환값으로 전달합니다.

## 4. 담당 역할과 기여 범위

저장소와 기술 문서에서 확인되는 구현 범위는 다음과 같습니다.

- 전역 서비스와 스테이지 런타임 조립 구조
- 타워 건설·이동·합성 및 필드 점유 관리
- 상점·타워 대기열·아이템 슬롯 연결
- 데이터 기반 웨이브, 적 생성, A* 경로 이동
- 런 강화와 계정 메타 성장 계산
- Firebase·로컬 JSON 저장 및 데이터 검증
- 퀘스트·업적 데이터와 Presenter 기반 UI

> TODO: 확인 필요 — 위 범위 중 본인이 직접 설계·구현한 항목, 협업 항목, 기여율을 최종 포트폴리오에 명시해야 합니다.

## 5. 주요 구현 시스템

| 시스템 | 핵심 구현 | 기술 문서 |
|---|---|---|
| 런타임 아키텍처 | Managers가 전역 서비스를 제공하고 StageManager가 스테이지 범위 시스템과 이벤트를 조정 | [SystemArchitecture](../Architecture/SystemArchitecture.md) |
| 타워 건설 | 마우스 좌표 변환, 셀 검증, 프리뷰, 생성, FieldTowerManager 등록, 성공 후 큐 제거 | [BuildSystem](../Systems/BuildSystem.md) |
| 상점·대기열 | 구매와 필드 배치 시점을 분리하고 슬롯 수용 실패 시 골드 환불 | [ShopAndQueueSystem](../Systems/ShopAndQueueSystem.md) |
| 웨이브·적 | WavePrepareResult 기반 사전 검증, EnemyFactory 생성, 스폰 상태와 생존 적 수를 함께 사용한 종료 판정 | [WaveAndEnemySystem](../Systems/WaveAndEnemySystem.md) |
| 메타 성장 | 원본 스탯, 영구 강화 레벨, 런 강화 단계를 분리해 최종 스탯 계산 | [MetaUpgradeSystem](../Systems/MetaUpgradeSystem.md) |
| 저장·불러오기 | 로컬 옵션과 계정 진행 분리, Firestore 결과 타입화, 타임아웃·검증·dirty flag 처리 | [SaveLoadSystem](../Systems/SaveLoadSystem.md) |
| 퀘스트·UI | ScriptableObject 퀘스트 복제, 공통 보고 API, Presenter/View 분리, 패널 전환 상태 관리 | [QuestSystem](../Systems/QuestSystem.md) · [UIInfoPanelSystem](../Systems/UIInfoPanelSystem.md) |

## 6. 핵심 기술과 설계

- **Data-Driven**: 타워, 적, 아이템, 웨이브, 강화 규칙을 Resources JSON에서 로드합니다.
- **Service Locator**: Managers를 통해 공용 데이터와 저장 서비스에 접근합니다.
- **Mediator 역할**: StageManager와 StageUIController가 하위 시스템의 호출과 이벤트를 연결합니다.
- **Observer**: C# event Action과 Func로 전투·UI·퀘스트 상태를 전달합니다.
- **Factory**: EnemyFactory가 적 생성과 필수 컴포넌트 초기화를 담당합니다.
- **Repository**: FirestoreSaveRepository가 Firebase 로드 결과와 오류 상태를 변환합니다.
- **Presenter/View 분리**: UI 표시 변환과 Unity View 조작을 분리합니다.
- **Dirty Flag**: 변경된 저장 영역만 기록합니다.

## 7. 확인 가능한 구현 규모와 결과

- Assets/02.Scripts 기준 C# 스크립트 187개
- 타워 데이터 36행: 6종족 × 6등급
- 웨이브 데이터 240행, 적 로스터 424행
- 적 데이터 26행, 적 스킬 데이터 13행
- 아이템 데이터 21행
- 메타 강화 데이터 76행
- 현지화 데이터 196행

구현 결과:

- 타워 등록 성공 후에만 대기열 슬롯을 제거합니다.
- 타워 합성 실패 시 제거한 재료 타워의 복구를 시도합니다.
- 웨이브 시작 전에 적·스킬 UID와 스폰 수치의 유효성을 검사합니다.
- 스폰 종료와 생존 적 수를 함께 검사해 웨이브 종료를 판정합니다.
- Firestore 문서 누락, 시간 초과, 권한, 네트워크, 데이터 손상을 구분합니다.

## 8. 현재 한계와 개선 방향

- Enemy와 Tower의 반복 Instantiate/Destroy 비용은 프로파일러 확인 후 풀링 적용을 검토할 수 있습니다.
- 적마다 계산하는 A* 경로는 시작·목표 셀 기준 캐시와 PriorityQueue로 개선할 수 있습니다.
- StageManager와 TowerController의 책임을 더 작은 흐름 객체로 분리할 수 있습니다.
- 종료 시 async void 저장 대신 안전한 체크포인트와 재시도 큐가 필요합니다.
- TODO: 확인 필요 — KillEnemy, CollectItem, UpgradeTower 퀘스트의 실제 게임플레이 보고 연결은 현재 문서 검수에서 확인되지 않았습니다.

## 9. 플레이 자료와 면접 포인트

### 플레이 자료

- 플레이 영상: TODO: 링크 추가
- 스크린샷 / GIF: TODO: 자료 추가
- 실행 빌드: TODO: 링크 또는 실행 방법 추가

### 면접에서 설명할 핵심

1. 구매·큐·GameObject·필드 점유 상태의 일관성을 어떻게 유지했는가
2. 스폰 완료와 생존 적 수를 어떻게 하나의 웨이브 종료 조건으로 구성했는가
3. 정적 콘텐츠, 런 상태, 계정 저장 데이터를 어떤 기준으로 분리했는가
4. 이벤트 기반 연결의 장점과 현재 Service Locator·대형 Manager 구조의 한계는 무엇인가
5. 실패 복구, 저장 검증, 성능 병목을 어떤 순서로 개선할 것인가
