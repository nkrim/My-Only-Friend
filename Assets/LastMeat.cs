using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastMeat : Meat
{
    public AudioClip finalSong;

    bool eaten = false;

    Character chr;
    private GameObject proximity_show;

    private void Awake () {
        chr = GameObject.FindWithTag("Player").GetComponent<Character>();
        proximity_show = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (eaten)
            return;
        proximity_show.SetActive(true);
        chr.target_meat = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        proximity_show.SetActive(false);
        chr.target_meat = null;
    }

    public override void Eat() {
        Camera.main.cullingMask = LayerMask.GetMask(new string[] { "UI" });
        Camera.main.GetComponent<DialogueSystem>().StartScene("Finale");
        if(finalSong) {
            AudioSource src = Camera.main.GetComponent<AudioSource>();
            src.clip = finalSong;
            src.Play();
        }
    }
}
