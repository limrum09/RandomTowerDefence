using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 타워 대기열을 관리하는 클래스
/// 대기열 슬롯 초기화, 대기열에 타워 UID 추가
/// 대기열에서 타워 제거, 슬롯 클릭 시 TowerController에 설치 요청 전달
/// </summary>
public class QueueController : MonoBehaviour
{
    // 대기열 슬롯 UI 목록
    [SerializeField]
    private List<QueueSlotUI> slots = new List<QueueSlotUI>();
    private string[] towerUID;                  // 슬롯별 저장된 타워 UID
    private int length;                         // 대기열 슬롯 개수

    public event Action<string, int, QueueController> OnRequestBuildTower;
    public event Action OnRemoveTowerFromQueue;

    private void Awake()
    {
        // 슬롯 개수 저장
        length = slots.Count;

        // 슬롯 개수만큰 UID 배열 생성
        towerUID = new string[length];

        // 빈 슬롯으로 초기화
        for(int i = 0; i < length; i++)
        {
            towerUID[i] = string.Empty;
            slots[i].Init(this, i);
            slots[i].RemoveSlotUI();
        }
    }

    public void MoveFieldTowerToQueue(string uid)
    {
        if (!AddTower(uid))
            return;

        OnRemoveTowerFromQueue?.Invoke();
    }

    /// <summary>
    /// 대기열에 타워를 추가한다.
    /// 빈 슬롯을 앞에서 부터 찾아 UID를 저장하고 UI를 갱신
    /// </summary>
    /// <param name="uid">추가할 타워 UID</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddTower(string uid)
    {
        for(int i = 0; i < length; i++)
        {
            if (slots[i].IsQueueEmpty)
            {
                towerUID[i] = uid;
                slots[i].SetSlotUI(uid);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 지정한 인덱스의 타워를 대시열에서 제거
    /// </summary>
    /// <param name="index">제거할 타워의 인덱스</param>
    public void RemoveTower(int index)
    {
        // 잘못된 인덱스 방어
        if (index < 0 || index >= length)
            return;

        // 초기화 및 UI슬롯 비우기
        towerUID[index] = string.Empty;
        slots[index].RemoveSlotUI();
    }

    /// <summary>
    /// 지정한 슬롯의 UID 반환
    /// </summary>
    /// <param name="index">조회할 슬롯 인덱스</param>
    /// <returns>타워 UID, 없으면 string.Empty</returns>
    public string GeTowerUID(int index)
    {
        if (index < 0 || index >= length)
            return string.Empty;

        return towerUID[index];
    }

    /// <summary>
    /// 자식의 QueueSlotUI 슬롯 클릭 시, 호출
    /// </summary>
    /// <param name="index">해당 슬롯의 위치</param>
    /// <param name="uid">클릭한 타워의 UID</param>
    public void OnClickQueueSlot(int index, string uid)
    {
        if (index < 0 || index >= length)
            return;

        if (uid == string.Empty)
            return;

        if (!towerUID.Contains(uid))
            return;

        OnRequestBuildTower?.Invoke(uid, index, this);
    }
}
