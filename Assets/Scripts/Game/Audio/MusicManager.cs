using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource actionTrack;
    public AudioSource calmTrack;

    public void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        actionTrack.Pause();

        WaveEvents.OnPlayerKilled += OnPlayerKilled;
        WaveEvents.OnGameStart += OnGameStart;
    }

    public void OnPlayerKilled() {
        StartCoroutine(setVolume(actionTrack, 0, false));
    }
    public void OnGameStart() {
        calmTrack.volume = 1;
        calmTrack.UnPause();
    }

    public void StartAction() {
        StopAllCoroutines();
        StartCoroutine(setVolume(calmTrack, 0, true));
    }
    public void StartCalm() {
        StopAllCoroutines();
        StartCoroutine(setVolume(actionTrack, 0, true));
    }

    public IEnumerator setVolume(AudioSource src, float volume, bool switchToOtherSong) {
        if (volume == 1) {
            src.UnPause();
        }
        while(src.volume != volume) {
            src.volume = Mathf.MoveTowards(src.volume, volume, Time.deltaTime * 2);
            yield return null;
        }
        if(volume == 0) {
            src.Pause();
        }
        if(switchToOtherSong) {
            AudioSource thisSrc = (src == actionTrack ? calmTrack : actionTrack);
            StartCoroutine(setVolume(thisSrc, 1, false));
        }
    }

    
}
