using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Rhythm Game/Song")]
public class SongData : ScriptableObject
{
    public AudioClip music;
    public List<NoteData> notes;
}
