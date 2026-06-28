# 퀘스트 및 업적 시스템

## 1. 개요

게임플레이 시스템이 카테고리·대상·증가량만 보고하면 등록된 퀘스트와 업적이 조건을 평가하고 진행도를 갱신하는 범용 시스템이다. 원본은 ScriptableObject로 제작하고 런타임에는 복제본을 사용한다.

## 2. 구현 목적

- QuestCategory로 정의된 사건을 동일한 QuestRecieveReport 인터페이스로 전달할 수 있게 한다.
- 퀘스트 원본 에셋이 플레이 중 변경되지 않게 한다.
- 대상 비교와 선행 조건을 교체 가능한 객체로 구성한다.
- 업적 진행도와 완료 상태를 저장한다.
- 전용 EditorWindow에서 퀘스트·대상·조건 에셋을 제작할 수 있게 한다.

## 3. 해결하려던 문제

각 게임 시스템이 특정 업적 클래스를 직접 호출하면 업적 추가 때 전투 코드를 수정해야 한다. ScriptableObject 원본에 진행도를 저장하면 에디터 에셋이 오염되고 모든 인스턴스가 상태를 공유한다. 완료 처리 중 활성 목록을 수정하면 열거 예외도 발생할 수 있다.

## 4. 주요 클래스

| 클래스 | 책임 |
|---|---|
| [QuestManager](../../Assets/02.Scripts/Managers/Core/QuestManager.cs) | 등록, 보고 분배, 활성·완료 목록, 저장 |
| [Quest](../../Assets/02.Scripts/Quest/Quest.cs) | 카테고리, 조건, Task, 보상, 완료 생명주기 |
| [Achievement](../../Assets/02.Scripts/Quest/Achievement.cs) | 저장 가능한 Quest 특수화 |
| [QuestTaskData](../../Assets/02.Scripts/Quest/Task/QuestTaskData.cs) | 대상 필터, 성공 횟수, 완료 판정 |
| [TaskTarget](../../Assets/02.Scripts/Quest/Task/Target/TaskTarget.cs) | 보고 대상 비교 추상화 |
| [EnemyTarget](../../Assets/02.Scripts/Quest/Task/Target/EnemyTarget.cs) / [UIDTarget](../../Assets/02.Scripts/Quest/Task/Target/UIDTarget.cs) | 적 타입 또는 문자열 UID 비교 구현 |
| [QuestCondition](../../Assets/02.Scripts/Quest/Condition/QuestCondition.cs) | 퀘스트 활성 조건 추상화 |
| [QuestRewardData](../../Assets/02.Scripts/Quest/QuestRewardData.cs) | 완료 보상 지급 |
| [QuestEditorWindow](../../Assets/02.Scripts/Editor/QuestEditorWindow.cs) | 퀘스트·Target·Condition 제작 도구 |

## 5. 데이터 흐름

~~~mermaid
flowchart LR
    Asset[Quest ScriptableObject] --> Clone[Runtime Clone]
    Report[Category + Target + Count] --> Manager[QuestManager]
    Manager --> Clone
    Clone --> Condition[QuestCondition]
    Clone --> Task[QuestTaskData]
    Task --> Target[TaskTarget]
    Task --> Progress[Current Success]
    Progress --> Reward[QuestRewardData]
    Progress -->|Achievement 저장 대상| Save[QuestSaveData]
~~~

원본 Quest와 Task는 등록 시 복제되며, 진행 상태는 복제본에만 존재한다.

## 6. 이벤트 흐름

~~~mermaid
sequenceDiagram
    participant Game as Gameplay System
    participant QM as QuestManager
    participant Q as Quest
    participant T as QuestTaskData
    participant S as SaveDataManager

    Game->>QM: QuestRecieveReport(category, target, count)
    QM->>Q: QuestRecieveReport
    Q->>Q: Condition check
    Q->>T: TaskRecieveReport
    T->>T: Category / Target check
    T-->>Q: OnTaskCompleted
    Q->>Q: QuestRewardData.Give
    Q-->>QM: OnQuestComplete
    alt Achievement
        QM->>QM: activeAchievement에서 completeAchievement로 이동
        QM->>S: MarkQuestDirty
        QM->>S: SaveAchievementData
    else 일반 Quest
        QM->>QM: activeQuest에서 제거
    end
~~~

## 7. 핵심 구현 방식

### 범용 보고 필터

~~~csharp
if (TaskCategory != category) return;
if (!TaskContainsTarget(target)) return;

CurrentSuccess = TaskAction.Run(
    actionType, currentSuccess, successCount);

if (IsCompleted)
    OnTaskCompleted?.Invoke();
~~~

Category는 사건 종류, TaskTarget은 사건 대상을 판별한다. targets가 비어 있으면 해당 카테고리의 모든 대상을 허용한다.

### 런타임 복제

Quest.Clone은 ScriptableObject를 Instantiate하고 QuestTaskData를 새 객체로 복제한다. 원본 정의와 플레이 상태를 분리한다.

### 안전한 목록 변경

QuestManager는 활성 목록의 복사본을 순회한다. 보고 중 퀘스트가 완료되어 원본 목록에서 제거되어도 반복이 안전하다.

## 8. 설계 의도

- **Prototype**: ScriptableObject 정의를 런타임 복제
- **구성 기반 데이터**: Quest가 Condition, Task, Reward 참조를 조합
- **Strategy 역할**: TaskTarget.IsEqual과 QuestCondition.IsPass를 추상 메서드로 분리
- **Observer**: Task 완료 → Quest 완료 → QuestManager 처리의 이벤트 연쇄
- **연산 분리**: TaskAction.Run이 Set, Add, ContinuePositive 계산을 담당
- **Data-Driven Authoring**: 전용 EditorWindow와 에셋 데이터베이스

## 9. 문제 해결 과정

QuestManager는 QuestCategory, target, count를 받는 공통 보고 메서드를 제공한다. StageManager에서 명시적으로 확인되는 보고 호출은 ClearStage와 BuildTower다. QuestReporter는 직렬화된 category와 target을 사용해 보고할 수 있다. KillEnemy, CollectItem, UpgradeTower의 실제 발생 지점 연결은 현재 검색 범위에서 확인되지 않았다.

완료 이벤트가 목록을 변경하는 문제는 활성 목록 복사 순회와 Contains 재검사로 해결했다. 저장 복원 시 기존 업적을 UID로 찾아 복제하고, 새 버전에 추가된 업적은 저장에 없더라도 자동 등록한다.

## 10. 결과

- QuestCategory enum에 KillEnemy, ClearStage, CollectItem, UpgradeTower, BuildTower, Achievement가 정의되어 있다.
- ClearStage와 BuildTower는 StageManager에서 QuestRecieveReport 호출이 확인된다.
- TODO: 확인 필요 — KillEnemy, CollectItem, UpgradeTower의 실제 게임플레이 보고 연결은 코드 검색에서 확인되지 않았다.
- EASY, NORMAL, HARD, HELL 난이도 업적을 동일 데이터 모델로 관리한다.
- 진행 중·완료 업적을 분리하고 UI에서 정렬·필터링한다.
- 저장 버전에 없던 신규 업적도 로드 후 추가된다.
- 제작·검색·중복 UID 검사·디버그 완료 기능을 에디터 도구로 제공한다.

## 11. 개선 가능성

- active 목록을 QuestCategory별 Dictionary로 인덱싱해 매 보고의 전체 순회를 줄인다.
- 보고마다 생성하는 List 복사를 재사용 버퍼 또는 지연 완료 큐로 대체한다.
- QuestTaskData.Clone은 targets = this.targets로 대입하므로 TaskTarget 참조를 공유한다. 런타임에서 Target 에셋을 변경하지 않는다는 계약을 명시하거나 필요한 경우 복제 정책을 추가한다.
- QuestRewardType.Item 분기는 현재 플레이어 인벤토리에 아이템을 추가하지 않고 player dirty flag만 설정한다. 의도된 미구현인지 TODO: 확인 필요.
- 완료 시 fire-and-forget으로 호출하는 원격 저장을 저장 큐로 옮겨 실패·재시도를 관리한다.
- 일반 Quest 진행에서도 MarkQuestDirty가 호출되지만 QuestManager.GetSaveData는 Achievement만 수집한다. 일반 Quest를 저장하지 않는 것이 의도인지 TODO: 확인 필요.
- QuestStat.Comoplete 등 명명 오타를 마이그레이션 가능한 방식으로 정리한다.
- 업적 UID 변경을 지원하는 별칭 또는 마이그레이션 테이블을 둔다.
- TaskAction과 Condition 조합을 단위 테스트한다.

## 12. 포트폴리오에 강조할 점

- 게임 시스템과 업적 정의를 분리한 범용 보고 구조
- ScriptableObject Prototype으로 원본과 런타임 상태를 분리
- TaskTarget·QuestCondition 추상화와 완료 이벤트 연쇄
- 콘텐츠 추가를 지원하는 전용 에디터와 저장 호환 처리
