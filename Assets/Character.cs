using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour { 

    public float jumpForce = 4000f;
    public float walkSpeed = 10f;
    [Range(0f,1f)]
    public float walkRamp = 0.5f;
    [Range(0f, 1f)]
    public float walkRampAir = 0.3f;


    // Input vars
    bool jump_pressed = false;
    float input_x = 0;

    // Linking vars
    private Parasite parasite;
    private Rigidbody2D rb;
    private Collider2D jump_cldr;
    private SpriteRenderer dog_sprite;
    private ParasiteHead p_head;

    // Layer masks
    private LayerMask foreground_mask;

    // Start is called before the first frame update
    private void Awake () {
        // Link vars
        if(!(parasite = GetComponentInChildren<Parasite>())) Debug.LogWarning("CHARACTER COULD NOT FIND PARASITE");
        if(!(rb = GetComponent<Rigidbody2D>())) Debug.LogWarning("CHARACTER COULD NOT FIND RIGIDBODY2D");
        if(!(jump_cldr = transform.Find("JumpCollider").GetComponent<Collider2D>())) Debug.LogWarning("CHARACTER CAN'T FIND JUMP COLLIDER");
        if (!(dog_sprite = transform.Find("Dog").GetComponent<SpriteRenderer>())) Debug.LogWarning("CHARACTER CAN'T FIND DOG SPRITE");
        p_head = GetComponentInChildren<ParasiteHead>();
        // Init layer masks
        foreground_mask = LayerMask.GetMask(new string[] { "Foreground" });
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetButtonDown("Jump")) {
            jump_pressed = true;
        }
        // Can set up curves from here
        input_x = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate () {
        // Jump
        if(jump_pressed) {
            jump_pressed = false;
            if(IsGrounded()) {
                Jump();
            }
        }
        // Move
        if(!p_head.IsGrabbed())
            Move();
    }


    bool IsGrounded() {
        return jump_cldr.IsTouchingLayers(foreground_mask);
    }

    void Jump() {
        rb.AddForce(jumpForce*Vector3.up);
    }

    // Manipulate horizontal velocity by input_x
    void Move () {
        float cur_vel_x = rb.velocity.x;
        float goal_vel_x = input_x * walkSpeed;
        float final_vel_x = Mathf.Lerp(cur_vel_x, goal_vel_x, IsGrounded() ? walkRamp : walkRampAir);
        rb.velocity = new Vector3(final_vel_x, rb.velocity.y);
        // Flip sprite
        if(Mathf.Abs(input_x) > 0.01 && Mathf.Abs(final_vel_x) > 0.01 && dog_sprite.flipX == final_vel_x > 0) {
            dog_sprite.flipX = !dog_sprite.flipX;
            parasite.FlipX(dog_sprite.flipX);
        }


    }
}
