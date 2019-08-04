using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDog : MonoBehaviour
{
    private GameObject proximity_show;

    private void Awake () {
        proximity_show = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        proximity_show.SetActive(true);
    }
    private void OnTriggerExit2D (Collider2D collision) {
        proximity_show.SetActive(false);
    }
}
