using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteHead : MonoBehaviour
{
    public Sprite closedMouth = null;
    public float moveForce = 0.5f;
    public float grabbedForce = 10f;
    public float maxSpeed = 10f;
    public float carryOverVelocityCoeff = 5f;
    public float grabbedDogMass = 1f;

    [HideInInspector]
    public Meat target_meat = null;

    Sprite openMouth = null;
    bool is_grabbed = false;
    float original_dog_mass;

    // Linking vars
    private SpriteRenderer sr = null;
    private Collider2D cldr;
    private Rigidbody2D rb;
    private Character chr;
    private Rigidbody2D chr_rb;

    // Layermasks
    private LayerMask foreground_mask;

    private void Awake () {
        sr = GetComponent<SpriteRenderer>();
        openMouth = sr.sprite;
        cldr = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        chr = GetComponentInParent<Character>();
        chr_rb = chr.GetComponent<Rigidbody2D>();
        original_dog_mass = chr_rb.mass;
        foreground_mask = LayerMask.GetMask(new string[] { "Foreground" });
    }



    private void Update () {
        if (chr.removeControl)
            return;
        if (is_grabbed & Input.GetMouseButtonUp(0))
            Ungrab();
        else if(!is_grabbed & Input.GetMouseButtonDown(0)) {
            if(target_meat != null) {
                target_meat.Eat();
            }
            else
                Grab();
        }
        sr.sprite = Input.GetMouseButton(0) ? closedMouth : openMouth;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(chr.removeControl)
            return;
        // Get mouse position
        Vector3 m_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_pos.z = 0;
        //rb.AddForce(moveForce*forceDir);
        if(!is_grabbed) {
            // Push rigidbody towards m_pos
            Vector3 force = m_pos - transform.position;
            Vector3 forceDir = force.normalized;
            rb.AddForce(moveForce * forceDir);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
        else {
            // Push rigidbody towards m_pos
            Vector3 force = m_pos - transform.parent.GetChild(0).position; // Relative to base
            Vector3 forceDir = force.normalized;
            chr_rb.AddForce(grabbedForce * -forceDir, ForceMode2D.Impulse);
            chr_rb.velocity = Vector2.ClampMagnitude(chr_rb.velocity, maxSpeed);
        }

    }

    public void ForceMoveHead(Vector3 target) {
        Vector3 force = target - transform.position;
        Vector3 forceDir = force.normalized;
        rb.AddForce(moveForce * forceDir);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }



    void Grab() {
        if(!cldr.IsTouchingLayers(foreground_mask))
            return;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        chr_rb.mass = grabbedDogMass;
        chr_rb.freezeRotation = false;
        is_grabbed = true;
    }
    void Ungrab () {
        rb.constraints = RigidbodyConstraints2D.None;
        chr_rb.mass = original_dog_mass;
        //chr_rb.transform.rotation = Quaternion.identity;
        //chr_rb.freezeRotation = true;
        is_grabbed = false;
        // Propel chr_rb
        chr_rb.velocity *= carryOverVelocityCoeff;
    }


    public bool IsGrabbed () {
        return is_grabbed;
    }
}
