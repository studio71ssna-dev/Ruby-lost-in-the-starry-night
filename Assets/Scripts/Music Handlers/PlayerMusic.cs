using SingletonManagers;
using UnityEngine;

public class PlayerMusic : MonoBehaviour
{
    [SerializeField] private InputJudge judge;
    [SerializeField] private NoteManager noteManager;
    [SerializeField] private ScoreManager scoreManager;

    private int activeFret = 0;
    private bool[] frets = new bool[3];

    private void OnEnable()
    {
        InputHandler.Instance.OnLane1 += () => HandleLane(0);
        InputHandler.Instance.OnLane2 += () => HandleLane(1);
        InputHandler.Instance.OnLane3 += () => HandleLane(2);

        InputHandler.Instance.OnFret1 += (p) => HandleFret(1, p);
        InputHandler.Instance.OnFret2 += (p) => HandleFret(2, p);
        InputHandler.Instance.OnFret3 += (p) => HandleFret(3, p);
    }

    private void Start()
    {
        judge.OnHit += HandleHit;
        noteManager.OnMiss += HandleMiss;
    }

    private void HandleLane(int lane)
    {
        judge.Judge(lane, activeFret);
    }

    private void HandleFret(int fret, bool pressed)
    {
        frets[fret - 1] = pressed;

        for (int i = 0; i < frets.Length; i++)
        {
            if (frets[i])
            {
                activeFret = i + 1;
                return;
            }
        }

        activeFret = 0;
    }

    private void HandleHit(HitResult result, Vector3 pos)
    {
        switch (result)
        {
            case HitResult.Perfect:
                scoreManager.Perfect();
                ParticleManager.Instance.PlayParticle("NoteHit", pos);
                AudioManager.Instance.Play("NoteHit", pos);
                break;

            case HitResult.Good:
                scoreManager.Good();
                ParticleManager.Instance.PlayParticle("NoteHit", pos);
                AudioManager.Instance.Play("NoteHit", pos);
                break;
        }
    }

    private void HandleMiss()
    {
        Debug.Log("MISS");

        scoreManager.WolfPressureUp?.Invoke();
    }
}