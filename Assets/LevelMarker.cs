using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class LevelMarker : MonoBehaviour
{
    public Vector3 respawnPoint = Vector3.zero;
    public bool initUpsideDown = false;

    private void OnTriggerEnter2D (Collider2D collision) {
        LevelChanger lc = Camera.main.GetComponent<LevelChanger>();
        this.GetComponent<CinemachineVirtualCamera>().Priority = 100;
        lc.cur_level.GetComponent<CinemachineVirtualCamera>().Priority = 10;
        lc.prev_level = lc.cur_level;
        lc.cur_level = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        LevelChanger lc = Camera.main.GetComponent<LevelChanger>();
        if(lc.cur_level == this) {
            Collider2D dog_cldr = GameObject.FindWithTag("Player").GetComponent<Character>().GetDog().GetComponent<Collider2D>();
            lc.prev_level.GetComponent<CinemachineVirtualCamera>().Priority = 100;
            lc.cur_level.GetComponent<CinemachineVirtualCamera>().Priority = 10;
            lc.cur_level = lc.prev_level;
            lc.prev_level = this;
        }
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireCube(transform.TransformPoint(respawnPoint), 0.25f*Vector3.one);
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
