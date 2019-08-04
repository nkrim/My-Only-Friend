using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastMeat : MonoBehaviour
{
    private void OnTriggerExit2D (Collider2D collision) {
        Camera.main.cullingMask = LayerMask.GetMask(new string[] { "UI" });
        Camera.main.GetComponent<DialogueSystem>().StartScene("Finale");
    }
}
