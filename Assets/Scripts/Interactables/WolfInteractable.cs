using UnityEngine;

public class WolfInteractable : MonoBehaviour,IInteractable
{
    [SerializeField]private SongController songController;
    [SerializeField]private SongData[] songs;
    [SerializeField]private GameObject MusicUI;

    private void Awake()
    {
        songController = Object.FindFirstObjectByType<SongController>();
        
    }

    public void Interact(PlayerController player)
    {
        MusicUI.SetActive(true);
        if (songController == null)
        {
            Debug.LogError("SongController NOT FOUND!");
            return;
        }

        if (songs == null || songs.Length == 0)
        {
            Debug.LogError("No songs assigned!");
            return;
        }

        Debug.Log("Wolf interacted with! Starting random song...");

        int index = Random.Range(0, songs.Length);
        songController.StartSong(songs[index]);
    }
}