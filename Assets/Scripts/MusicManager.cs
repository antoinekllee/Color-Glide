using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmTracks = null;
    private int currentTrack = 0;

    [SerializeField] private AudioSource bgmSource = null;

    private void Start()
    {
        SetRandomBGM();
    }

    private void Update ()
    {
        if (!bgmSource.isPlaying)
            SetRandomBGM();
    }

    private void SetRandomBGM ()
    {
        int randomTrack = Random.Range(0, bgmTracks.Length);
        while (randomTrack == currentTrack)
            randomTrack = Random.Range(0, bgmTracks.Length);

        bgmSource.clip = bgmTracks[randomTrack];
        bgmSource.Play();

        currentTrack = randomTrack;
    }
}
