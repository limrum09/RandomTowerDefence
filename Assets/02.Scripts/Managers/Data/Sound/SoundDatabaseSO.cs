using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Game/Sound Database")]
public class SoundDatabaseSO : ScriptableObject
{
    [SerializeField]
    private List<SoundData> sounds = new List<SoundData>();

    public List<SoundData> Sounds => sounds;
}
