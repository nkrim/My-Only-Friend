using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public AudioClip gameSound;

    static bool muted = false;

    private GameObject title;
    private GameObject title_mute;

    private void Awake () {
        Time.timeScale = 0;
        title = GameObject.FindWithTag("Title");
        title_mute = GameObject.FindWithTag("TitleMute");
        title_mute.SetActive(false);
    }

    public void CloseTitle() {
        title.SetActive(false);
        Time.timeScale = 1;
        if(gameSound) {
            AudioSource src = Camera.main.GetComponent<AudioSource>();
            src.clip = gameSound;
            src.Play();
        }
    }

    public void ToggleMuteSound() {
        muted = !muted;
        title_mute.SetActive(muted);
        AudioListener.volume = muted ? 0 : 1;
    }
}
