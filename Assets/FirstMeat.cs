using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMeat : Meat
{
    public float poopSpeed = 0.2f;
    public List<GameObject> setActiveAfterEating;
    public Sprite bones;

    bool eaten = false;
    bool pooping = false;

    Character chr;
    Parasite p;

    private GameObject proximity_show;

    private void Awake () {
        chr = GameObject.FindWithTag("Player").GetComponent<Character>();
        p = chr.GetParasite();
        proximity_show = transform.GetChild(0).gameObject;
    }

    private void Update () {
        if(pooping) {
            float cur_x = p.transform.localPosition.x;
            cur_x -= poopSpeed;
            if(cur_x <= 0) {
                p.transform.localPosition = Vector3.zero;
                pooping = false;
                Camera.main.GetComponent<DialogueSystem>().StartScene("Intro");
                Rigidbody2D[] rbs = p.GetComponentsInChildren<Rigidbody2D>();
                foreach(Rigidbody2D rb in rbs)
                    rb.constraints = RigidbodyConstraints2D.None;
            }
            else {
                p.transform.localPosition = Vector3.right*cur_x;
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if(eaten)
            return;
        proximity_show.SetActive(true);
        chr.target_meat = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        proximity_show.SetActive(false);
        chr.target_meat = null;
    }

    public override void Eat() {
        proximity_show.SetActive(false);
        chr.target_meat = null;
        eaten = true;
        transform.parent.GetComponent<SpriteRenderer>().sprite = bones;
        chr.removeControl = true;
        pooping = true;
        if(chr.IsFlippedX())
            chr.FlipX();
        p.gameObject.SetActive(true);
        foreach(GameObject g in setActiveAfterEating)
            g.SetActive(true);
    }
}
