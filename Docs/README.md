# RandomTowerDefence 기술 문서

## 문서 목적

이 페이지는 RandomTowerDefence의 포트폴리오·아키텍처·시스템 구현·회고 문서를 찾기 위한 기술 문서 메인입니다.

프로젝트의 전체 구조를 먼저 확인하려면 **ProjectSummary → SystemArchitecture → 개별 Systems 문서** 순서로 읽을 수 있습니다.

## Portfolio

- [ProjectSummary](Portfolio/ProjectSummary.md) — 채용 담당자가 프로젝트 범위, 핵심 시스템, 기술적 특징을 빠르게 파악할 수 있도록 요약합니다.
- [CaseStudy](Portfolio/CaseStudy.md) — 프로젝트 배경, 문제 정의, 설계 선택, 구현 결과를 사례 중심으로 정리하기 위한 문서입니다.
- [InterviewQA](Portfolio/InterviewQA.md) — 아키텍처, 게임플레이, 데이터, 저장, 성능에 관한 면접 예상 질문과 답변을 정리합니다.

## Architecture

- [SystemArchitecture](Architecture/SystemArchitecture.md) — 전역 서비스, 스테이지 런타임, 데이터, UI, 저장 계층의 책임과 클래스 관계를 설명합니다.
- [EventFlow](Architecture/EventFlow.md) — 적 생명주기, 타워 건설, UI, 퀘스트에서 발생하는 이벤트의 발행·구독 흐름을 설명합니다.
- [DataFlow](Architecture/DataFlow.md) — Resources JSON, 런 세션 상태, 영구 진행 데이터가 로드·사용·저장되는 경로를 설명합니다.

## Systems

- [BuildSystem](Systems/BuildSystem.md) — 상점과 대기열에서 시작해 셀 검증, 타워 생성, 필드 등록, 큐 갱신으로 이어지는 건설 흐름을 설명합니다.
- [ShopAndQueueSystem](Systems/ShopAndQueueSystem.md) — 상품 구매, 골드 처리, 타워 대기열, 설치 요청의 책임 분리와 실패 시 환불 처리를 설명합니다.
- [WaveAndEnemySystem](Systems/WaveAndEnemySystem.md) — 웨이브 데이터 검증, 적 생성, A* 이동, 생존 적 집계와 웨이브 종료 판정을 설명합니다.
- [MetaUpgradeSystem](Systems/MetaUpgradeSystem.md) — 원본 스탯, 계정 메타 강화, 런 강화의 계산 순서와 저장 데이터 구조를 설명합니다.
- [SaveLoadSystem](Systems/SaveLoadSystem.md) — 로컬 옵션과 Firestore 진행 저장, 데이터 검증, 타임아웃, dirty flag 처리를 설명합니다.
- [QuestSystem](Systems/QuestSystem.md) — ScriptableObject 퀘스트 복제, 공통 보고 API, 진행도·완료 이벤트와 업적 저장을 설명합니다.
- [UIInfoPanelSystem](Systems/UIInfoPanelSystem.md) — Presenter/View 역할 분리, StageUIController 이벤트 연결, 정보 패널 전환 흐름을 설명합니다.
- [EditorTooling](Systems/EditorTooling.md) — JSON·ScriptableObject 제작, 데이터 검증, Play Mode 디버깅을 보조하는 Unity Editor 확장 도구를 설명합니다.

## Postmortem

- [Postmortem](Postmortem.md) — 프로젝트의 결과, 문제 해결 경험, 기술적 부채와 향후 개선 방향을 정리하기 위한 회고 문서입니다.
