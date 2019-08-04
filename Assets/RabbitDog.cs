using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitDog : Meat
{
    public Sprite killedSprite;
    public List<GameObject> setInactiveAfterEating;
    public bool firstRabbit = false;

    bool killed = false;
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

    public override void Eat () {
        if(!killed && !eaten) { 
            proximity_show.SetActive(false);
            //chr.target_meat = null;
            killed = true;
            transform.parent.GetComponent<SpriteRenderer>().sprite = killedSprite;
            foreach (GameObject g in setInactiveAfterEating)
                g.SetActive(false);
            if(firstRabbit) {
                Camera.main.GetComponent<DialogueSystem>().StartScene("KillFirstRabbit");
            }
        }
        else if(!eaten) {
            Camera.main.GetComponent<DialogueSystem>().StartScene("CantEat");
        }
    }

    public void EatenByParasite() {
        eaten = true;
        killed = true;
        proximity_show.SetActive(false);
        chr.target_meat = this;
    }
}
