using UnityEngine;

public enum HitResult
{
    None,
    Perfect,
    Good,
    Miss
}

public class InputJudge : MonoBehaviour
{
    [SerializeField] private SongController songController;
    [SerializeField] private NoteManager noteManager;

    [Header("Timing Windows")]
    [SerializeField] private float perfectWindow = 0.08f;
    [SerializeField] private float goodWindow = 0.12f;

    public System.Action<HitResult, Vector3> OnHit;

    public HitResult Judge(int lane, int activeFret)
    {
        Note note = noteManager.PeekLane(lane);
        if (note == null) return HitResult.None;

        if (note.NoteFret != activeFret)
            return HitResult.None;

        double time = songController.SongTime;
        float diff = Mathf.Abs((float)(note.HitTime - time));

        if (diff <= perfectWindow)
        {
            Vector3 pos = note.transform.position;
            noteManager.RemoveNote(lane);
            OnHit?.Invoke(HitResult.Perfect, pos);
            return HitResult.Perfect;
        }
        else if (diff <= goodWindow)
        {
            Vector3 pos = note.transform.position;
            noteManager.RemoveNote(lane);
            OnHit?.Invoke(HitResult.Good, pos);
            return HitResult.Good;
        }

        return HitResult.Miss;
    }
}