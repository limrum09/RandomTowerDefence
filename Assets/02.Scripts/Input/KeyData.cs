using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용하는 입력 액션 종류
/// 실제 KeyCode를 직접 사용하지 않고 의미있는 액션 이름으로 입력을 관리하기 위한 Enum
/// </summary>
public enum InputAction
{
    MoveTower,
    MakeTower,
    TowerMoveToQueueSlot,
    ShowGradeUpgradeTowerView,
    ShowStatUpgradeTowerView,
    TowerGradeNormalUpgrade,
    TowerGradePremiunUpgrade,
    TowerStatDamgeUpgrade,
    TowerStatAttackSpeedUpgrade,
    Buy,
    MakeObstacle,
    RemoveObstacle,
    Options
}

/// <summary>
/// 입력 액션과 실제 KeyCode를 매핑해서 관리
/// 기본 키 성정 초기화, 액션별 KeyCode 조회
/// 키 변경, 중복 키 사용 여부 확인
/// </summary>
public class KeyData
{
    // InputAction변로 열결된 실제 KeyCode 저장
    private Dictionary<InputAction, KeyCode> keys = new Dictionary<InputAction, KeyCode>();

    /// <summary>
    /// 모든 입력 샌셩의 키를 기본값으로 초기화
    /// </summary>
    public void ResetKeyCodes()
    {
        keys[InputAction.MoveTower] = KeyCode.M;
        keys[InputAction.MakeTower] = KeyCode.N;
        keys[InputAction.TowerMoveToQueueSlot] = KeyCode.Q;
        keys[InputAction.ShowGradeUpgradeTowerView] = KeyCode.Z;
        keys[InputAction.ShowStatUpgradeTowerView] = KeyCode.X;
        keys[InputAction.TowerGradeNormalUpgrade] = KeyCode.E;
        keys[InputAction.TowerGradePremiunUpgrade] = KeyCode.D;
        keys[InputAction.TowerStatDamgeUpgrade] = KeyCode.W;
        keys[InputAction.TowerStatAttackSpeedUpgrade] = KeyCode.S;
        keys[InputAction.Buy] = KeyCode.B;
        keys[InputAction.MakeObstacle] = KeyCode.R;
        keys[InputAction.RemoveObstacle] = KeyCode.T;
        keys[InputAction.Options] = KeyCode.Escape;
    }

    /// <summary>
    /// 지정한 입력 액션에 들옥된 KeyCode가져오기
    /// </summary>
    /// <param name="key">조회할 입력 액션</param>
    /// <param name="keyCode">조회된 KeyCode</param>
    /// <returns>등록된 키가 있으면 true</returns>
    public bool TryGetKeyCode(InputAction key, out KeyCode keyCode)
    {
        return keys.TryGetValue(key, out keyCode);
    }

    /// <summary>
    /// 지정된 입력 액션에 등록된 keyCode를 반환
    /// 등록된 코드가 없으면 KeyCode.None반환
    /// </summary>
    /// <param name="key">조회할 입력 액션</param>
    /// <returns>들록된 KeyCode 또는 KeyCode.None</returns>
    public KeyCode GetKeyCode(InputAction key)
    {
        if (keys.TryGetValue(key, out KeyCode keyCode))
            return keyCode;

        return KeyCode.None;
    }

    /// <summary>
    /// 지정한 입력 액션의 keyCode를 변경
    /// </summary>
    /// <param name="key">변경할 입력 액션</param>
    /// <param name="keyCode">새로 들록할 KeyCode</param>
    public void SetKeyCode(InputAction key, KeyCode keyCode)
    {
        keys[key] = keyCode;
    }

    /// <summary>
    /// 특정 KeyCode가 다른 액션에서 이미 사용 중인지 확인
    /// 현재 변경하려는 액션 자신은 검사에서 제외
    /// </summary>
    /// <param name="key">현재 변경하려는 입력 액션</param>
    /// <param name="keyCode">중복 여부를 검사할 KeyCode</param>
    /// <returns>다른 액션에서 이미 사용중이면 true</returns>
    public bool ContainsValue(InputAction key, KeyCode keyCode)
    {
        foreach (var pair in keys)
        {
            if (pair.Key != key && pair.Value == keyCode)
                return true;
        }

        return false;
    }
}
