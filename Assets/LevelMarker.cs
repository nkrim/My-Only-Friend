using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class LevelMarker : MonoBehaviour
{
    public Vector3 respawnPoint = Vector3.zero;
    public bool initUpsideDown = false;
    public bool initFlipped = false;

    private GameObject light_list = null;
    private CinemachineVirtualCamera vc = null;
    private LevelChanger lc = null;
    private Character player = null;

    private void Awake () {
        light_list = transform.Find("Lights").gameObject;
        light_list.SetActive(false);
        vc = GetComponent<CinemachineVirtualCamera>();
        lc = Camera.main.GetComponent<LevelChanger>();
        player = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void Start () {

    }

    private void Update () {
        // Check if character has gotten too far away (by slipping through)
        if(lc.cur_level == this) {
            Vector3 chr_pos = player.transform.position;
            if (Vector2.Distance(chr_pos, transform.position) > 20) {
                Restart();
            }
        }

    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if(lc.cur_level == this)
            return;
        this.ActivateLevel();
        lc.cur_level.DeactivateLevel();
        lc.prev_level = lc.cur_level;
        lc.cur_level = this;
    }
    private void OnTriggerExit2D (Collider2D collision) {
        if(lc.cur_level == this) {
            Collider2D dog_cldr = GameObject.FindWithTag("Player").GetComponent<Character>().GetDog().GetComponent<Collider2D>();
            if(dog_cldr.IsTouching(lc.prev_level.GetComponent<Collider2D>())) {
                lc.prev_level.ActivateLevel();
                lc.cur_level.DeactivateLevel();
                lc.cur_level = lc.prev_level;
                lc.prev_level = this;
            }
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
        //Character player = GameObject.FindWithTag("Player").GetComponent<Character>();

        Vector3 new_pos = transform.TransformPoint(respawnPoint);
        new_pos.z = 0;
        player.transform.position = new_pos;
        if(player) {
            Parasite par = player.GetParasite();
            if (par)
                par.ResetTail();
        } 

        if (initUpsideDown) { 
            player.transform.localEulerAngles = new Vector3(0,0,180);
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        else
            player.transform.localEulerAngles = Vector3.zero;

        if(initFlipped != player.IsFlippedX())
            player.FlipX();
    }

    public IEnumerator _ProcessShake (float amplitude = 1f, float frequency = 5f, float shakeTiming = 0.5f) {
        Noise(amplitude, frequency);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }

    void Noise (float amplitudeGain, float frequencyGain) {
        CinemachineBasicMultiChannelPerlin noise = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }

}
