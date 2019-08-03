using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parasite : MonoBehaviour
{
    public float injectionSpeed = 2f;
    public float ejectionSpeed = 5f;
    public float ejectionRamp = 0.7f;

    // Private vars
    private bool flippedX = false;
    private bool injecting = false;
    private float ejection = -1f; // less than 0: all the way in, 1: all the way out
    private float injectionIncrement = 0.2f;

    // Linking vars
    private Transform offset;
    private List<Rigidbody2D> tail_links;
    private ParasiteHead head;


    // Lifecycle
    private void Awake () {
        // Linking
        if(!(offset = transform.Find("Parasite Tail Offset"))) Debug.LogWarning("PARASITE COULD NOT FIND OFFSET");
        tail_links = new List<Rigidbody2D>();
        for(int i=1; i<offset.childCount; i++) {
            tail_links.Add(offset.GetChild(i).GetComponent<Rigidbody2D>());
        }
        head = GetComponentInChildren<ParasiteHead>();
    }

    private void Start () {
        //ForceInject();
    }

    private void Update () {
        // Grab and ungrab

    }

    private void FixedUpdate () {
        //if(injecting) {
        //    Rigidbody2D base_link = tail_links[0];
        //    base_link.velocity = transform.right * injectionSpeed * (flippedX ? -1 : 1);
        //    if(!base_link.freezeRotation) {
        //        float adjusted_angle = base_link.transform.localEulerAngles.z;
        //        if(adjusted_angle >= 180)
        //            adjusted_angle -= 360;

        //        if (Mathf.Abs(base_link.transform.localEulerAngles.z) < 0.1)
        //            base_link.freezeRotation = true;
        //        else
        //            base_link.angularVelocity = 4 * -base_link.transform.localEulerAngles.z * Time.fixedDeltaTime;
        //    }
        //}
    }


    // Public funcs
    public void FlipX(bool value) {
        //if(ejection < 0)
            //return;
        if(this.flippedX == value)
            return;
        this.flippedX = value;

        Rigidbody2D base_link = tail_links[0];
        Transform t = base_link.transform;
        t.localPosition = new Vector3(-t.localPosition.x, 0, 0);
        t.localScale = new Vector3(-t.localScale.x, 1, 1);
        t.localEulerAngles = new Vector3(0, 0, -t.localEulerAngles.z);
        //SliderJoint2D joint = t.GetComponent<SliderJoint2D>();
        //joint.connectedAnchor = new Vector2(-joint.connectedAnchor.x, joint.connectedAnchor.y);
        Transform prev_transform = t;
        for(int i=1; i<tail_links.Count; i++) {
            Rigidbody2D rb = tail_links[i];
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
            t = rb.transform;
                Vector3 pos_adjust = injectionIncrement * prev_transform.right * (flippedX ? 1 : -1);
                t.position = prev_transform.position + pos_adjust;
                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
                t.localEulerAngles = new Vector3(0, 0, -prev_transform.localEulerAngles.z); // Divide by 2 to counteract springing of joint;
            prev_transform = t;
        }
    }

    // Protected funcs
    void Eject(Vector3 target) {
        // Get head
        Rigidbody2D head = offset.GetChild(offset.childCount-1).GetComponent<Rigidbody2D>();
        // Get target velocity
        //Vector3 final_pos = 

    }
    void Inject() {

    }
    void ForceInject() {
        for(int i=0; i<offset.childCount; i++) {
            Transform child = offset.GetChild(i);
            // Turn off simulation and shove into body
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            rb.simulated = false;
            child.localPosition = Vector3.zero;
            child.gameObject.SetActive(false);
        }
    }
    void Grab() {

    }
    void Ungrab() {

    }
}
