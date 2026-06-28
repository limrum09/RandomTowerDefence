# RandomTowerDefence

## 1. 프로젝트 소개

RandomTowerDefence는 Unity 6 기반 2D 타워 디펜스 프로젝트입니다. 상점에서 구매한 타워를 대기열에 보관한 뒤 그리드의 유효한 셀에 배치하고, JSON 데이터로 구성된 웨이브를 진행합니다. 전투 중 적용되는 런 강화와 계정에 유지되는 메타 성장을 분리했으며, 로컬 JSON과 Firebase Firestore를 용도에 따라 나누어 저장합니다. 기능 개발을 종료하고 기술 구조와 문제 해결 과정을 문서화한 상태입니다.

## 2. 플레이 영상 / GIF

- TODO: 30초 플레이 영상 추가
- TODO: 상점 → 대기열 → 타워 설치 GIF 추가
- TODO: 웨이브 전투 GIF 추가
- TODO: 메타 성장 UI GIF 추가

## 3. 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 장르 | 2D 타워 디펜스 |
| 엔진 | Unity 6 |
| 언어 | C# |
| 개발 기간 | 2026.04.13 ~ 2026.06.26 |
| 개발 인원 | 1인 개발 |
| 대상 플랫폼 | Steam |
| 개발 상태 | 기능 개발 종료, 포트폴리오 문서화 단계 |

## 4. 담당 역할

1인 개발 기준 전체 구현을 담당했습니다.

- 전체 프로그래밍
- 시스템 설계
- UI 구현
- 데이터 구조 설계
- 저장·로드 구현
- 기술 문서 작성

## 5. 핵심 구현 시스템

| 시스템 | 구현 내용 | 상세 문서 |
|---|---|---|
| 타워 건설 | 마우스 위치를 그리드 셀로 변환하고 배치 가능 여부를 검사한 뒤, 생성·필드 점유 등록·큐 갱신을 순서대로 처리 | [BuildSystem](Docs/Systems/BuildSystem.md) |
| 상점 및 타워 대기열 | 구매와 필드 배치 시점을 분리하고, 상품 수용 실패 시 골드를 환불하며 설치 성공 후에만 큐를 제거 | [ShopAndQueueSystem](Docs/Systems/ShopAndQueueSystem.md) |
| 웨이브 및 적 | JSON 웨이브 데이터를 사전 검증하고 적 생성·이동·사망·도착 이벤트와 생존 적 수를 이용해 웨이브 상태를 관리 | [WaveAndEnemySystem](Docs/Systems/WaveAndEnemySystem.md) |
| 메타 성장 | 타워 원본 스탯, 계정 영구 강화, 전투 중 런 강화를 분리해 최종 능력치를 계산 | [MetaUpgradeSystem](Docs/Systems/MetaUpgradeSystem.md) |
| 저장 및 불러오기 | 로컬 옵션과 계정 진행 데이터를 분리하고 Firestore 오류 상태, 데이터 검증, 타임아웃, dirty flag를 처리 | [SaveLoadSystem](Docs/Systems/SaveLoadSystem.md) |
| 퀘스트 및 업적 | ScriptableObject 원본을 런타임 객체로 복제하고 공통 보고 API, 진행도·완료 이벤트, 저장 데이터를 관리 | [QuestSystem](Docs/Systems/QuestSystem.md) |
| UI 정보 패널 | View, Presenter, StageUIController의 역할을 나누고 이벤트 기반 갱신과 패널 전환 상태를 관리 | [UIInfoPanelSystem](Docs/Systems/UIInfoPanelSystem.md) |

## 6. 기술 스택

| 기술 | 사용 위치 |
|---|---|
| Unity 6 | 2D 게임 런타임, 씬과 컴포넌트 구성 |
| C# | 게임플레이 시스템, 데이터 모델, 이벤트 기반 시스템 연결 |
| Firebase Authentication | 사용자 인증과 계정 저장 초기화 |
| Firebase Firestore | 플레이어 진행, 메타 성장, 퀘스트·업적 데이터 저장 |
| JSON Data | Resources 기반 타워·적·웨이브·아이템·강화 데이터와 로컬 옵션 관리 |
| DOTween | 정보 패널 이동과 화면 전환 Sequence 제어 |
| Unity UI | View 컴포넌트, 입력 전달, 정보 패널과 세션 상태 표시 |
| ScriptableObject | 퀘스트·업적 원본 데이터 구성 |

## 7. 대표 문제 해결 사례

### 7.1 구매와 배치 시점 분리

- **Problem:** 구매 시점과 실제 배치 시점이 달라 골드, 대기열, 생성 객체, 필드 점유 상태가 어긋날 수 있었습니다.
- **Solution:** StoreController → QueueUIController → TowerController → FieldTowerManager로 구매, 보관, 배치 검증, 필드 등록 책임을 분리했습니다.
- **Result:** 대기열 수용 실패 시 골드를 환불하고, 필드 등록 성공 시에만 설치 완료 이벤트로 큐를 제거합니다.

### 7.2 웨이브 종료 조건 처리

- **Problem:** 적 스폰 완료와 필드의 마지막 적 제거가 서로 다른 시점에 발생합니다.
- **Solution:** isSpawning과 aliveEnemyCnt를 별도로 관리하고 스폰 종료, 적 사망, 적 도착 시 동일한 종료 조건을 다시 검사합니다.
- **Result:** 스폰이 끝나고 생존 적이 0명인 경우에만 웨이브 완료 흐름으로 이동합니다.

### 7.3 저장 실패와 데이터 검증

- **Problem:** 신규 사용자 문서 누락과 네트워크·권한·시간 초과·데이터 손상을 같은 실패로 처리하기 어려웠습니다.
- **Solution:** FireStoreLoadResult로 로드 상태를 구분하고 IValidSaveData 검증, 타임아웃, 저장 영역별 dirty flag를 적용했습니다.
- **Result:** 기본 데이터 생성과 실제 오류 처리를 분리하고, 변경된 원격 저장 영역만 기록합니다.

자세한 설계 판단과 한계는 [Case Study](Docs/Portfolio/CaseStudy.md)에서 확인할 수 있습니다.

## 8. 기술 문서

### 문서 안내

- [기술 문서 메인](Docs/README.md)
- [프로젝트 요약](Docs/Portfolio/ProjectSummary.md)
- [케이스 스터디](Docs/Portfolio/CaseStudy.md)

### 아키텍처

- [시스템 아키텍처](Docs/Architecture/SystemArchitecture.md)
- [이벤트 흐름](Docs/Architecture/EventFlow.md)
- [데이터 흐름](Docs/Architecture/DataFlow.md)

### 시스템

- [타워 건설 시스템](Docs/Systems/BuildSystem.md)
- [상점 및 대기열 시스템](Docs/Systems/ShopAndQueueSystem.md)
- [웨이브 및 적 시스템](Docs/Systems/WaveAndEnemySystem.md)
- [메타 성장 시스템](Docs/Systems/MetaUpgradeSystem.md)
- [저장 및 불러오기 시스템](Docs/Systems/SaveLoadSystem.md)
- [퀘스트 및 업적 시스템](Docs/Systems/QuestSystem.md)
- [UI 정보 패널 시스템](Docs/Systems/UIInfoPanelSystem.md)

## 9. 개선 방향

- 플레이 영상과 핵심 흐름 GIF를 추가해 구현 결과를 시각적으로 제시
- StageManager와 TowerController에 집중된 조정 책임의 세분화 검토
- Unity Profiler 기반으로 적·타워 생성과 경로 탐색 비용 측정
- 저장 데이터의 schemaVersion과 마이그레이션 정책 검토
- 건설, 큐 상태, 웨이브 종료, 저장 검증 흐름에 자동화 테스트 추가

## 10. 연락 / 포트폴리오

- TODO: 포트폴리오 사이트 링크 추가
