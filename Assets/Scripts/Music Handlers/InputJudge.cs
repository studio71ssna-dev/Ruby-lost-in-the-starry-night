using UnityEngine;

public enum HitResult { None, Perfect, Good, Miss }

public class InputJudge : MonoBehaviour
{
    [SerializeField] private SongController songController;
    [SerializeField] private NoteManager noteManager;

    [Header("Timing Windows")]
    [SerializeField] private float perfectWindow = 0.08f;
    [SerializeField] private float goodWindow = 0.15f;

    public System.Action<HitResult, Vector3> OnHit;

    public HitResult Judge(int lane, int activeColor)
    {
        Note note = noteManager.PeekLane(lane);
        if (note == null) return HitResult.None;

        // noteFret == 0 means white/neutral — no arrow key needed
        // noteFret 1/2/3 means brown/green/purple — must hold matching arrow
        if (note.NoteFret != activeColor)
        {
            Debug.Log($"Color mismatch — note is {note.NoteFret}, you are holding {activeColor}");
            return HitResult.None;
        }

        double time = songController.SongTime;
        float diff = Mathf.Abs((float)(note.HitTime - time));

        if (diff <= perfectWindow)
        {
            Vector3 pos = note.transform.position;
            noteManager.RemoveNote(lane);
            OnHit?.Invoke(HitResult.Perfect, pos);
            return HitResult.Perfect;
        }

        if (diff <= goodWindow)
        {
            Vector3 pos = note.transform.position;
            noteManager.RemoveNote(lane);
            OnHit?.Invoke(HitResult.Good, pos);
            return HitResult.Good;
        }

        return HitResult.Miss;
    }
}