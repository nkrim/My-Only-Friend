using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string sceneName;

    bool activated = false;

    private void OnTriggerEnter2D (Collider2D collision) {
        if(!activated) {
            activated = true;
            print(Camera.main.GetComponent<DialogueSystem>().StartScene(sceneName));
        }
    }
}
