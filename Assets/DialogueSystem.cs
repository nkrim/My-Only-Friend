using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct Sentence {
    public int spriteIndex;
    public string text;
}
public struct Dialogue {
    public string name;
    public List<Sentence> dialogue;
}

public class DialogueSystem : MonoBehaviour { 

    public List<Sprite> characters;

    List<Dialogue> dialogueScenes = new List<Dialogue>();
    Dialogue cur_scene;
    int sentence_index = -1;
    Coroutine text_scroll = null;
    Transform cur_dialogue_box = null;

    private Transform dialogue_box_face_left = null;
    private Transform dialogue_box_face_right = null;

    private Character chr;




    private void Awake () {
        dialogue_box_face_left = transform.Find("Canvas/DialogueFaceLeft");
        dialogue_box_face_right = transform.Find("Canvas/DialogueFaceRight");

        chr = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiateDialogueScenes();
        StartScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(sentence_index >= 0) {
            // ensure no control is given to other objects
            chr.removeControl = true;
            if (Input.anyKeyDown) {
                // Scrip text scrolling
                if (text_scroll != null) {
                    StopCoroutine(text_scroll);
                    text_scroll = null;
                    TextMeshProUGUI tmp = cur_dialogue_box.GetComponentInChildren<TextMeshProUGUI>();
                    tmp.text = cur_scene.dialogue[sentence_index].text;
                }
                // Next sentence
                else {
                    NextSentence();
                }
            }
        }
    }

    public void StartScene(int index) {
        Dialogue d = dialogueScenes[index];
        cur_scene = d;
        sentence_index = 0;
        StartSentence(cur_scene.dialogue[0]);
    }
    public void StartScene(string name) {
        for(int i=0; i<dialogueScenes.Count; i++) {
            if(dialogueScenes[i].name.Equals(name))
                StartScene(i);
        }
    }
    void EndScene() {
        sentence_index = -1;
        chr.removeControl = false;
    }

    void NextSentence () {
        cur_dialogue_box.gameObject.SetActive(false);
        sentence_index++;
        if(sentence_index < cur_scene.dialogue.Count) {
            StartSentence(cur_scene.dialogue[sentence_index]);
        }
        else {
            EndScene();
        }
    }

    void StartSentence(Sentence s) {
        Sprite face = s.spriteIndex >= 0 ? characters[s.spriteIndex] : null;
        cur_dialogue_box = s.spriteIndex > 0 ? dialogue_box_face_left : dialogue_box_face_right;
        Transform face_box = cur_dialogue_box.transform.Find("FaceBox");
        if (!face)
            face_box.gameObject.SetActive(false);
        else {
            face_box.gameObject.SetActive(true);
            face_box.GetChild(0).GetComponent<SpriteRenderer>().sprite = face;
        }
        TextMeshProUGUI tmp = cur_dialogue_box.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "";
        cur_dialogue_box.gameObject.SetActive(true);
        text_scroll = StartCoroutine(_ProcessText(tmp, s.text));
    }
    IEnumerator _ProcessText(TextMeshProUGUI tmp, string text) {
        for(int i=0; i<text.Length; i++) {
            yield return new WaitForSeconds(0.15f);
            tmp.text += text[i];
        }
        text_scroll = null;
    }



    void InstantiateDialogueScenes() {
        // SCENE 1 - Intro
        Dialogue d = new Dialogue {
            name = "Intro",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "HEY!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "HEY FRIEND!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "YOU'RE MY NEW\nFLIPPIN FRIEND NOW!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "AND I'M NOW\nYOUR ONLY FRIEND..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'LL HELP YOU GET\nTHE HELL OUT OF HERE..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "...MY FRIEND."
        });
        dialogueScenes.Add(d);


        // Scene 2
    }
}
