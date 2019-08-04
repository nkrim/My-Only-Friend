using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour { 
    
    public float jumpForce = 4000f;
    public float maxJumpAngle = 20f;
    public float flipAfterTime = 1;
    public float walkSpeed = 10f;
    [Range(0f,1f)]
    public float walkRamp = 0.5f;
    [Range(0f, 1f)]
    public float walkRampAir = 0.3f;

    public float refreezeAngle = 5f;

    public bool removeControl = true;
    public bool removeControlOnInit = true;
    public bool hideParasiteOnStart = true;
    public float parasiteStartX = 0.68f;

    public GameObject setActiveAfterFlip;

    [HideInInspector]
    public Meat target_meat = null;

    // Input vars
    bool jump_pressed = false;
    float input_x = 0;

    // State vars
    float flip_jumping = -1f;
    float time_falesly_grounded = 0f;

    // Linking vars
    private Parasite parasite;
    private Rigidbody2D rb;
    private Transform dog;
    private Collider2D jump_cldr;
    private Collider2D top_cldr;
    private Collider2D front_cldr;
    private Collider2D back_cldr;
    private SpriteRenderer dog_sprite;
    private ParasiteHead p_head;
    private Animator anim;
    private LevelChanger lc;

    // Layer masks
    private LayerMask foreground_mask;

    // Start is called before the first frame update
    private void Awake () {
        // Link vars
        if(!(parasite = GetComponentInChildren<Parasite>())) Debug.LogWarning("CHARACTER COULD NOT FIND PARASITE");
        if(!(rb = GetComponent<Rigidbody2D>())) Debug.LogWarning("CHARACTER COULD NOT FIND RIGIDBODY2D");
        if(!(dog = transform.Find("Dog"))) Debug.LogWarning("CHARACTER COULD NOT FIND DOG");
        if (!(jump_cldr = transform.Find("JumpCollider").GetComponent<Collider2D>())) Debug.LogWarning("CHARACTER CAN'T FIND JUMP COLLIDER");
        if (!(top_cldr = transform.Find("TopCollider").GetComponent<Collider2D>())) Debug.LogWarning("CHARACTER CAN'T FIND TOP COLLIDER");
        if (!(front_cldr = transform.Find("FrontCollider").GetComponent<Collider2D>())) Debug.LogWarning("CHARACTER CAN'T FIND FRONT COLLIDER");
        if (!(back_cldr = transform.Find("BackCollider").GetComponent<Collider2D>())) Debug.LogWarning("CHARACTER CAN'T FIND BACK COLLIDER");
        if (!(dog_sprite = transform.Find("Dog").GetComponent<SpriteRenderer>())) Debug.LogWarning("CHARACTER CAN'T FIND DOG SPRITE");
        p_head = GetComponentInChildren<ParasiteHead>();
        anim = GetComponentInChildren<Animator>();
        lc = Camera.main.GetComponent<LevelChanger>();
        // Init layer masks
        foreground_mask = LayerMask.GetMask(new string[] { "Foreground" });
        // other
        if(removeControlOnInit)
            removeControl = true;
    }

    private void Start () {
        if(hideParasiteOnStart) {
            parasite.transform.localPosition = new Vector3(parasiteStartX, 0, 0);
            parasite.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        // Can set up curves from here
        input_x = Input.GetAxis("Horizontal");

        if (removeControl) {
            // FLip jumping
            if (flip_jumping > 0) {
                if (flip_jumping > 0.5) {
                    transform.position += (flip_jumping - 0.5f) * Vector3.up;
                }
                float cur_angle = transform.localEulerAngles.z;
                if (cur_angle > 180) cur_angle -= 360;
                float goal_angle = Mathf.Lerp(cur_angle, 0, 0.25f);
                transform.localEulerAngles = new Vector3(0, 0, goal_angle);
                flip_jumping = Mathf.Abs(goal_angle / 180);
                if (flip_jumping < 0.1) {
                    flip_jumping = -1;
                    removeControl = false;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    transform.localEulerAngles = Vector3.zero;
                    rb.freezeRotation = true;
                    // Reset dog
                    dog.gameObject.layer = LayerMask.NameToLayer("Dog");
                    // Set active after flip
                    if(setActiveAfterFlip && !setActiveAfterFlip.activeSelf) {
                        setActiveAfterFlip.SetActive(true);
                    }
                }
                // Reset tail
                parasite.ResetTail();
            }
            else if (removeControlOnInit) {
                if (jump_cldr.IsTouchingLayers(foreground_mask) || top_cldr.IsTouchingLayers(foreground_mask) || front_cldr.IsTouchingLayers(foreground_mask) || back_cldr.IsTouchingLayers(foreground_mask)) {
                    removeControl = false;
                    removeControlOnInit = false;
                }
            }
        }
        // !removeControl stuff
        else if (!p_head.IsGrabbed()) { 
            if (Input.GetButtonDown("Jump")) {
                jump_pressed = true;
            }

            // Count for flip_jumping
            if(IsGroundedForFlip())
                time_falesly_grounded += Time.deltaTime;
            else
                time_falesly_grounded = 0;

            // If unfrozen, grounded and near-flat, refreeze rotation
            if(!rb.freezeRotation && IsGrounded() && Mathf.Abs(transform.localEulerAngles.z) < refreezeAngle) {
                transform.localEulerAngles = Vector3.zero;
                rb.freezeRotation = true;
            }

            // THe chomp
            if (Input.GetButtonDown("Fire1")) {
                anim.SetTrigger("Bite");
                if (target_meat) {
                    lc.ScreenShake(1f, 4f, 0.2f);
                    target_meat.Eat();
                }
                else {
                    lc.ScreenShake(0.25f, 2f, 0.2f);
                }
            }
        }

        // Animation updating
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetBool("IsMoving", Mathf.Abs(input_x) > 0.1f || Mathf.Abs(rb.velocity.x) > 1f);
    }

    private void FixedUpdate () {
        if(removeControl)
            return;
        if(p_head.IsGrabbed())
            return;
        // Jump
        if(jump_pressed) {
            jump_pressed = false;
            if(IsGrounded()) {
                Jump();
            }
            else if(IsGroundedForFlip() && time_falesly_grounded >= flipAfterTime) {
                JumpForFlip();
            }
        }
        // Move
        Move();
    }


    public Transform GetDog() {
        return dog;
    }
    public Parasite GetParasite() {
        return parasite;
    }


    bool IsGrounded() {
        float angle = transform.localEulerAngles.z;
        if(angle >= 180)
            angle -= 360;
        return Mathf.Abs(angle) <= maxJumpAngle && jump_cldr.IsTouchingLayers(foreground_mask);
    }
    bool IsGroundedForFlip() {
        if(rb.freezeRotation)
            return false;
        return front_cldr.IsTouchingLayers(foreground_mask) || top_cldr.IsTouchingLayers(foreground_mask) || back_cldr.IsTouchingLayers(foreground_mask);
    }

    void Jump() {
        rb.AddForce(jumpForce*Vector3.up);
    }
    void JumpForFlip() {
        removeControl = true;
        flip_jumping = 1f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        dog.gameObject.layer = LayerMask.NameToLayer("DodgePhysics");
    }

    // Manipulate horizontal velocity by input_x
    void Move () {
        float cur_vel_x = rb.velocity.x;
        float goal_vel_x = input_x * walkSpeed;
        float final_vel_x = Mathf.Lerp(cur_vel_x, goal_vel_x, IsGrounded() ? walkRamp : walkRampAir);
        if(IsGroundedForFlip() && !IsGrounded())
            final_vel_x /= 10;
        // Apply speed
        if ((input_x > 0 && !front_cldr.IsTouchingLayers(foreground_mask)) || (input_x < 0 && !back_cldr.IsTouchingLayers(foreground_mask)))
            rb.velocity = new Vector3(final_vel_x, rb.velocity.y);
        // Flip sprite
        if (IsGrounded() && Mathf.Abs(input_x) > 0.01 && dog_sprite.flipX == final_vel_x > 0 /*&& Mathf.Abs(final_vel_x) > 0.01*/)  {
            FlipX();
        }
    }

    public void FlipX() {
        if(!dog_sprite)
            return;
        dog_sprite.flipX = !dog_sprite.flipX;
        parasite.FlipX(dog_sprite.flipX);
    }
    public bool IsFlippedX() {
        return dog_sprite && dog_sprite.flipX;
    }
}
