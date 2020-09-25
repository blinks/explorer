using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PlayerInput))]
public class ExplorerController : MonoBehaviour {
  public Vector2 moveForce = new Vector2(20f, 0f);
  public Vector2 stopForce = new Vector2(10f, 0f);
  public float jumpForce = 50f;
  public GameObject target;
  public GameObject projectile;
  public float looseStrength = 50f;
  public float maxStrength = 10f;
  public float groundedLagTime = 0.1f;
  public float jumpingLagTime = 0.1f;
  public LayerMask groundMask;

  // Collider info.
  Rigidbody2D body;
  CapsuleCollider2D collider2d;
  readonly ContactPoint2D[] contacts = new ContactPoint2D[8];
  float groundTime = Mathf.NegativeInfinity;

  // Control info.
  Vector2 moveControl;
  Vector2 aimControl = Vector2.left;
  float jumpTime = Mathf.NegativeInfinity;

  enum BowState {
    READY = 0, DRAWING, LOOSING,
  }
  float drawStartTime = Mathf.Infinity;
  BowState bowState = BowState.READY;

  private void Start() {
    body = GetComponent<Rigidbody2D>();
    collider2d = GetComponent<CapsuleCollider2D>();
  }

  private void Update() {
    if (aimControl.x < 0) {
      GetComponent<SpriteRenderer>().flipX = true;
    } else if (aimControl.x > 0) {
      GetComponent<SpriteRenderer>().flipX = false;
    }

    GetComponent<Animator>().SetBool("Drawing", bowState == BowState.DRAWING);
  }

  private void FixedUpdate() {
    // Grab a reference to our animator.
    var animator = GetComponent<Animator>();

    // Use raycast instead of collider contacts because the latter doesn't work with platform effector.
    if (Physics2D.Raycast(body.position, Vector2.down, 1f, groundMask)) {
      groundTime = Time.time + groundedLagTime;
      animator.SetBool("Grounded", true);
    } else {
      animator.SetBool("Grounded", false);
    }

    // Calculate whether the player is moving or not only once.
    bool moveControlZero = Mathf.Approximately(0f, moveControl.magnitude);
    if (!moveControlZero) {
      // Whenever you move, reset aim control to match; always normalized.
      aimControl = moveControl.normalized;
    }

    // Rotate the aim target to match movement controls. 
    // Also draw it closer to the explorer (and brighter) as draw time increases.
    var power = drawPower();
    target.transform.localPosition = aimControl * 2f * (1.5f - power);
    target.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(.5f, .5f, .5f, .5f), Color.white, power);

    // Stop or reverse force if grounded && ready.
    if (groundTime > Time.time && bowState == BowState.READY
        && (moveControlZero || Vector2.Dot(moveControl, body.velocity) < 0)) {
      body.AddForce(-body.velocity * stopForce);
    }

    // Run, unless drawing the bow.
    animator.SetBool("Ducking", false);
    if (!moveControlZero && bowState == BowState.READY) {
      body.AddForce(moveControl * moveForce);
      if (moveControl.y < -0.5f) {
        animator.SetBool("Ducking", true);
      }
    }
    var v = body.velocity;
    v.x = Mathf.Clamp(v.x, -10f, 10f);
    animator.SetFloat("Walk Speed", Mathf.Abs(v.x));
    animator.SetFloat("VelocityY", v.y);
    body.velocity = v;

    // Jump
    if (groundTime > Time.time && jumpTime > Time.time) {
      body.AddForce(Vector2.up * jumpForce);
      jumpTime = Mathf.NegativeInfinity; // needs a new press for a second jump.
    }

    // Fire
    if (bowState == BowState.LOOSING) {
      // Use aimControl (already normalized) * drawPower.
      var b = Instantiate(projectile).GetComponent<Rigidbody2D>();
      b.position = body.position + aimControl / 2f;
      b.velocity = aimControl * looseStrength * Mathf.Pow(maxStrength, drawPower());
      b.SetRotation(Mathf.Atan2(aimControl.y, aimControl.x) * Mathf.Rad2Deg);

      bowState = BowState.READY;
      drawStartTime = Mathf.Infinity;
    }
  }

  public void OnMove(InputValue input) {
    moveControl = input.Get<Vector2>();
  }

  public void OnJump(InputValue input) {
    // Store the time "jump" was pressed to enable jumping a little early.
    jumpTime = input.Get<float>() > 0.5f ? Time.time + jumpingLagTime : Mathf.NegativeInfinity;
  }

  public void OnFire(InputValue input) {
    if (bowState == BowState.READY && input.Get<float>() > 0.5f) {
      drawStartTime = Time.time;
      bowState = BowState.DRAWING;
    } else if (bowState == BowState.DRAWING) {
      bowState = BowState.LOOSING;
    }
  }

  /** Draw power clamped to 0-1, increasing over time. */
  float drawPower() {
    if (bowState == BowState.READY) {
      return 0f; // skip heavier math if not drawing.
    }

    // Log to increase faster at start and slow later.
    // +1 to start at 0 instead of negative numbers.
    var t = Mathf.Log(Time.time - drawStartTime + 1f, 4f);
    return Mathf.Clamp01(t);
  }
}
