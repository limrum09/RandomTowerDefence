using UnityEngine;

[CreateAssetMenu(fileName = "Achievement_", menuName = "Quest/Achievement")]
public class Achievement : Quest
{
    public override bool IsSaveable => true;
}
