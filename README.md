# RandomTowerDefence

## 1. 프로젝트 소개

RandomTowerDefence는 Unity 6 기반의 2D 타워 디펜스 프로젝트입니다. 플레이어는 그리드에 장애물을 설치해 타워 배치 가능한 기반 셀을 만들고, 상점에서 구매한 타워를 대기열에 저장한 뒤 해당 장애물 셀 위에 타워를 배치합니다. 웨이브와 적 로스터는 JSON 데이터 기반으로 처리되며, 전투 중 적용되는 런 강화와 계정 단위로 유지되는 메타 성장은 분리되어 있습니다. 또한 로컬 옵션은 JSON으로, 계정 진행 데이터는 Firebase Firestore로 저장해 목적에 따라 저장 방식을 구분했습니다. 현재 기능 개발은 완료했으며, 기술 구조와 문제 해결 과정을 포트폴리오 문서로 정리했습니다.

## 2. 플레이 영상 / GIF

- TODO: 30초 플레이 영상 추가
- TODO: 상점 → 대기열 → 타워 설치 GIF 추가
- TODO: 웨이브 전투 GIF 추가
- TODO: 메타 성장 UI GIF 추가
- TODO: 주요 화면 스크린샷 추가

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
- Unity Editor 도구 구현
- 기술 문서 작성

## 5. 핵심 구현 시스템

| 시스템 | 구현 내용 | 상세 문서 |
|---|---|---|
| 타워 건설 | 그리드 셀 검증, 타워 생성, 필드 점유 등록, 성공 후 큐 갱신 | [BuildSystem](Docs/Systems/BuildSystem.md) |
| 상점 및 대기열 | 구매와 배치 시점 분리, 골드 처리, 실패 시 환불, 설치 성공 후 큐 제거 | [ShopAndQueueSystem](Docs/Systems/ShopAndQueueSystem.md) |
| 웨이브 및 적 | JSON 사전 검증, 적 생성·이동, 생존 적 집계와 웨이브 종료 판정 | [WaveAndEnemySystem](Docs/Systems/WaveAndEnemySystem.md) |
| 메타 성장 | 기본 스탯, 계정 영구 강화, 전투 중 런 강화를 분리한 능력치 계산 | [MetaUpgradeSystem](Docs/Systems/MetaUpgradeSystem.md) |
| 저장 및 불러오기 | 로컬 옵션과 계정 진행 분리, Firestore 오류 상태와 데이터 검증 처리 | [SaveLoadSystem](Docs/Systems/SaveLoadSystem.md) |
| 퀘스트 및 업적 | ScriptableObject 원본, 런타임 진행 객체, 공통 보고 API와 저장 관리 | [QuestSystem](Docs/Systems/QuestSystem.md) |
| UI 정보 패널 | Presenter/View 역할 분리, 이벤트 기반 갱신과 패널 전환 상태 관리 | [UIInfoPanelSystem](Docs/Systems/UIInfoPanelSystem.md) |
| Editor Tooling | JSON·퀘스트 에셋 제작, 데이터 검증, Play Mode 디버깅을 보조하는 Unity Editor 확장 | [EditorTooling](Docs/Systems/EditorTooling.md) |

## 6. 기술 스택

| 기술 | 사용 위치 |
|---|---|
| Unity 6 | 2D 게임 런타임, 씬과 컴포넌트 구성 |
| C# | 게임플레이 시스템, 데이터 모델, 이벤트 기반 연결 |
| Firebase Authentication | 사용자 인증과 계정 저장 초기화 |
| Firebase Firestore | 플레이어 진행, 메타 성장, 퀘스트·업적 데이터 저장 |
| JSON Data | Resources 기반 게임 데이터와 로컬 옵션 관리 |
| DOTween | 정보 패널 이동과 화면 전환 Sequence 제어 |
| Unity UI | View 컴포넌트, 입력 전달, 정보 패널과 세션 상태 표시 |
| ScriptableObject | 퀘스트·업적 원본 데이터 구성 |
| UnityEditor API | 데이터 편집 창, Custom Inspector, CustomPropertyDrawer 구현 |

## 7. 대표 문제 해결 사례

| 사례 | 문제 | 해결 결과 | 상세 |
|---|---|---|---|
| 구매와 배치 시점 분리 | 골드, 큐, 생성 객체, 필드 점유가 서로 다른 시점에 변경됨 | 수용 실패 시 환불하고 필드 등록 성공 후에만 큐 제거 | [Case Study](Docs/Portfolio/CaseStudy.md#사례-1-타워-구매와-건설-흐름-분리) |
| 웨이브 종료 조건 | 스폰 완료와 마지막 적 제거가 동시에 발생하지 않음 | isSpawning과 aliveEnemyCnt를 함께 검사해 완료 판정 | [Case Study](Docs/Portfolio/CaseStudy.md#사례-2-웨이브-종료-조건-처리) |
| 저장 실패와 검증 | 문서 없음과 네트워크·권한·손상 오류를 구분해야 함 | 결과 상태, 제한 시간, 저장 모델 검증과 dirty flag 적용 | [Case Study](Docs/Portfolio/CaseStudy.md#사례-3-저장-실패와-데이터-검증) |

## 8. 기술 문서

### 시작 문서

- [기술 문서 메인](Docs/README.md)
- [프로젝트 요약](Docs/Portfolio/ProjectSummary.md)
- [케이스 스터디](Docs/Portfolio/CaseStudy.md)
- [프로젝트 회고](Docs/Postmortem.md)

### 아키텍처

- [시스템 아키텍처](Docs/Architecture/SystemArchitecture.md)
- [이벤트 흐름](Docs/Architecture/EventFlow.md)
- [데이터 흐름](Docs/Architecture/DataFlow.md)

### 시스템

- [타워 건설](Docs/Systems/BuildSystem.md)
- [상점 및 대기열](Docs/Systems/ShopAndQueueSystem.md)
- [웨이브 및 적](Docs/Systems/WaveAndEnemySystem.md)
- [메타 성장](Docs/Systems/MetaUpgradeSystem.md)
- [저장 및 불러오기](Docs/Systems/SaveLoadSystem.md)
- [퀘스트 및 업적](Docs/Systems/QuestSystem.md)
- [UI 정보 패널](Docs/Systems/UIInfoPanelSystem.md)
- [Editor Tooling](Docs/Systems/EditorTooling.md)

## 9. 개선 방향

- StageManager와 TowerController에 집중된 조정 책임의 세분화 검토
- Unity Profiler 기반으로 생성·파괴, 경로 탐색, 타겟 탐색 비용 측정
- 저장 데이터의 schemaVersion과 마이그레이션 정책 검토
- 핵심 규칙과 실패 흐름에 대한 자동화 테스트 추가
- Editor Tool의 공통 편집 기반과 통합 검증 리포트 검토
