using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class LevelMarker : MonoBehaviour
{
    public Vector3 respawnPoint = Vector3.zero;
    public bool initUpsideDown = false;

    private GameObject light_list = null;
    private CinemachineVirtualCamera vc = null;

    private void Awake () {
        light_list = transform.Find("Lights").gameObject;
        light_list.SetActive(false);
        vc = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start () {

    }

    private void OnTriggerEnter2D (Collider2D collision) {
        LevelChanger lc = Camera.main.GetComponent<LevelChanger>();
        if(lc.cur_level == this)
            return;
        this.ActivateLevel();
        lc.cur_level.DeactivateLevel();
        lc.prev_level = lc.cur_level;
        lc.cur_level = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        LevelChanger lc = Camera.main.GetComponent<LevelChanger>();
        if(lc.cur_level == this) {
            Collider2D dog_cldr = GameObject.FindWithTag("Player").GetComponent<Character>().GetDog().GetComponent<Collider2D>();
            lc.prev_level.ActivateLevel();
            lc.cur_level.DeactivateLevel();
            lc.cur_level = lc.prev_level;
            lc.prev_level = this;
        }
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireCube(transform.TransformPoint(respawnPoint), 0.25f*Vector3.one);
    }

    public void ActivateLevel() {
        light_list.SetActive(true);
        vc.Priority = 100;
    }

    public void DeactivateLevel() {
        vc.Priority = 10;
        light_list.SetActive(false);
    }

    public void Restart() {
        Character player = GameObject.FindWithTag("Player").GetComponent<Character>();

        Vector3 new_pos = transform.TransformPoint(respawnPoint);
        new_pos.z = 0;
        player.transform.position = new_pos;
        if(player) {
            Parasite par = player.GetParasite();
            if (par)
                par.ResetTail();
        } 

        if (initUpsideDown)
            player.transform.localEulerAngles = new Vector3(0,0,180);
        else
            player.transform.localEulerAngles = Vector3.zero;
    }

}
