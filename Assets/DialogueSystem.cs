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
    public float scrollSpeed = 0.1f;

    List<Dialogue> dialogueScenes = new List<Dialogue>();
    Dialogue cur_scene;
    int sentence_index = -1;
    Coroutine text_scroll = null;
    Transform cur_dialogue_box = null;

    private Transform dialogue_box_face_left = null;
    private Transform dialogue_box_face_right = null;

    private Character chr;
    private ParasiteHead p_head;




    private void Awake () {
        dialogue_box_face_left = transform.Find("Canvas/DialogueFaceLeft");
        dialogue_box_face_right = transform.Find("Canvas/DialogueFaceRight");

        chr = GameObject.FindWithTag("Player").GetComponent<Character>();
        p_head = GameObject.FindWithTag("ParasiteHead").GetComponent<ParasiteHead>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiateDialogueScenes();
        //StartScene(0);
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

    private void FixedUpdate () {
        if (sentence_index >= 0 && cur_scene.dialogue[sentence_index].spriteIndex > 0) {
            p_head.ForceMoveHead(chr.transform.position + 0.75f*new Vector3(chr.IsFlippedX() ? 1 : -1,1));
        }
    }

    public bool StartScene(int index) {
        if(index >= dialogueScenes.Count)
            return false;
        chr.removeControl = true;
        Dialogue d = dialogueScenes[index];
        cur_scene = d;
        sentence_index = 0;
        StartSentence(cur_scene.dialogue[0]);
        return true;
    }
    public bool StartScene(string name) {
        for(int i=0; i<dialogueScenes.Count; i++) {
            if(dialogueScenes[i].name.Equals(name))
                return StartScene(i);
        }
        return false;
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
            yield return new WaitForSeconds(scrollSpeed);
            tmp.text += text[i];
        }
        text_scroll = null;
    }



    void InstantiateDialogueScenes() {
        // SCENE - Intro
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

        // Scene - HowToParasite
        d = new Dialogue {
            name = "HowToParasite",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "YOUR PARASITE CAN\nCLING TO SURFACES"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "USE IT TO FLING\nONTO THAT CLIFF"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "YEAH COME ON FRIEND"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'M JUST HERE TO HELP"
        });
        dialogueScenes.Add(d);


        // Scene - SeeFirstRabbit
        d = new Dialogue {
            name = "SeeFirstRabbit",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "HEY, SEE THAT IDIOT"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "WHAT'RE THEY DOING THERE"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "YOU DON’T NEED THEM"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'M YOUR ONLY FRIEND"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "KILL IT! KILL IT FOR ME!"
        });
        dialogueScenes.Add(d);

        // Scene - KillFirstRabbit
        d = new Dialogue {
            name = "KillFirstRabbit",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "*CHOMP*!!!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "YES VERY GOOD\nVERY GOOD KILL YES"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "YES YES\nI LIKE THE WAY YOU KILL"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "NOW LET ME FEED!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'VE BEEN SUCH A\nGOOD FRIEND"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "SO I THINK I DESERVE\nTHE FIRST BITE!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        dialogueScenes.Add(d);

        // Scene - CantEat
        d = new Dialogue {
            name = "CantEat",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "*CHOMP CHO-*"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "HEY!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "GET YOUR DIRTY MOUTH\nOFF THAT!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'M THE ONE DOING\nALL THE WORK HERE"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "THAT FOOD IS FOR ME!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        dialogueScenes.Add(d);

        // Scene - EatFirstRabbit
        d = new Dialogue {
            name = "EatFirstRabbit",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "*SLLLLLLURRRRRPP*"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "SO DELICIOUS\nSO SCRUMPTIOUS MMM"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I LOVE IT\nI LOVE IT I LOVE IT!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "SUSTENANCE I NEED\nMORE!!!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I'M YOUR ONE\nAND ONLY FRIEND"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "SO YOU BETTER\nGET MORE FOR ME!!!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "....."
        });
        dialogueScenes.Add(d);

        // Scene - EatLastRabbit
        d = new Dialogue {
            name = "EatLastRabbit",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "MMM SO DELICIOUS MMM"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I’M SO FULL!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "TUMMY WUMMY\nFULL OF DEAD FRIENDS"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "NOT FRIENDS!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "I’M THE ONLY FRIEND\nYOU NEED"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "BEING FULL\nMAKES ME SLEEPY..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "VERY TIRED..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "ZZZZZZZZZZZZ"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = ".........."
        });
        dialogueScenes.Add(d);

        // Scene - Finale
        d = new Dialogue {
            name = "Finale",
            dialogue = new List<Sentence>()
        };
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "*PLOP*"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "HEY!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "HEY FRIEND!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "ZZZ..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "WHAT'S THIS?\nANOTHER FRIEND?"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "NO!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "ONLY ONE FRIEND!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 0,
            text = "..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 1,
            text = "AGHHHHHHHHHHH!!!!!"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = 2,
            text = "I’M YOU’RE\nONLY FRIEND NOW..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "THE END..."
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "CREDITS:"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "PROGRAMMING:\nNOAH KRIM"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "DESIGN:\nNOAH KRIM"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "ART:\nMATT ZEHNER"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "MUSIC:\nSOMETHING SOFT"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "CONCEPT:\nNOAH & MATT"
        });
        d.dialogue.Add(new Sentence {
            spriteIndex = -1,
            text = "THANKS FOR PLAYING!!!\n:)"
        });
        dialogueScenes.Add(d);
    }
}
