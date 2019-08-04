using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitParasite : Meat
{
    public Sprite bones;
    public List<GameObject> setActiveAfterEating;
    public List<GameObject> setInactiveAfterEating;
    public bool firstRabbit = false;
    public bool lastRabbit = false;

    bool eaten;
    RabbitDog sibling;
    GameObject proximity_show;

    //Character chr;
    ParasiteHead p_head;

    private void Awake () {
        proximity_show = transform.GetChild(0).gameObject;
        sibling = transform.parent.GetComponentInChildren<RabbitDog>();
        //chr = GameObject.FindWithTag("Player").GetComponent<Character>();
        p_head = GameObject.FindWithTag("ParasiteHead").GetComponent<ParasiteHead>();
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (eaten)
            return;
        proximity_show.SetActive(true);
        p_head.target_meat = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        proximity_show.SetActive(false);
        p_head.target_meat = null;
    }

    public override void Eat() {
        if (eaten)
            return;
        sibling.EatenByParasite();
        proximity_show.SetActive(false);
        p_head.target_meat = null;
        eaten = true;
        transform.parent.GetComponent<SpriteRenderer>().sprite = bones;
        foreach (GameObject g in setInactiveAfterEating)
            g.SetActive(false);
        foreach (GameObject g in setActiveAfterEating)
            g.SetActive(true);
        if (firstRabbit) {
            Camera.main.GetComponent<DialogueSystem>().StartScene("EatFirstRabbit");
        }
        else if(lastRabbit) {
            Camera.main.GetComponent<DialogueSystem>().StartScene("EatLastRabbit");
            p_head.enabled = false;
        }
    }
}
