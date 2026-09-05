using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChomperController : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector3 lookDirection;
    private Quaternion lookRotation;
    private bool moving;
    private bool sprinting;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float lookSpeed = 5f;

    [SerializeField] private float powerupTime = 10f;

    private bool hasPowerup;

    private float powerupStartTime;

    private CharacterController charController;

    public static event Action<ChomperController> OnGetPowerup;
    public static event Action<ChomperController> OnLosePowerup;
    public static event Action<ChomperController, int> OnPlayerDied;

    public static event Action<ChomperController, int> ScoreIncreased;

    public static event Action<ChomperController, int> OnLivesChanged;

    public static event Action<ChomperController> OnPlayerLost;

    public static event Action<ChomperController, GameObject> OnPickup;

    [SerializeField] private int lives = 3;

    [SerializeField] private float deathCooldown = 0.2f;
    private float deathTime;

    private Vector3 startingPos;

    private void OnEnable()
    {
        //ChomperController.OnGetPowerup += GetPowerup;
        //ChomperController.OnLosePowerup += LosePowerup;
    }

    private void OnDisable()
    {
        //ChomperController.OnGetPowerup -= GetPowerup;
        //ChomperController.OnLosePowerup -= LosePowerup;
    }

    private void Start()
    {
        moving = false;
        sprinting = false;
        charController = GetComponent<CharacterController>();
        hasPowerup = false;
        startingPos = transform.position;
        deathTime = Time.time;
        ChangeLives(0);
    }

    private void Update()
    {
        Vector3 movePos = new Vector3(transform.position.x + moveInput.x, 0f, transform.position.z + moveInput.y);
        if (moving && Time.time >= deathTime + deathCooldown)
        {
            if (sprinting)
            {
                charController.Move(lookDirection.normalized * sprintSpeed * Time.deltaTime);
            }
            else
            {
                charController.Move(lookDirection.normalized * moveSpeed * Time.deltaTime);
            }
        }

        if (lookDirection != Vector3.zero && moving)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, lookSpeed * Time.deltaTime);
        }

        if (Time.time >= powerupStartTime + powerupTime && hasPowerup)
        {
            LosePowerup();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            moving = true;
            moveInput = context.ReadValue<Vector2>();
            lookDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            lookRotation = Quaternion.LookRotation(lookDirection.normalized);
        }
        else
        {
            moving = false;
            moveInput = Vector2.zero;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }
    }

    private void GetPowerup()
    {
        hasPowerup = true;
        powerupStartTime = Time.time;

        OnGetPowerup?.Invoke(this);
    }

    private void LosePowerup()
    {
        hasPowerup = false;

        OnLosePowerup?.Invoke(this);
    }

    private void Die()
    {
        hasPowerup = false;
        OnLosePowerup?.Invoke(this);

        ChangeLives(-1);

        deathTime = Time.time;

        transform.position = startingPos;
        transform.rotation = Quaternion.identity;

        if (lives < 0)
        {
            //super die
            OnPlayerLost?.Invoke(this);
        }
        else
        {
            OnPlayerDied?.Invoke(this, lives);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Powerup")
        {
            GainScore(5);
            GetPowerup();
            OnPickup?.Invoke(this, other.gameObject);
        }

        if (other.tag == "Pellet")
        {
            GainScore(1);
            OnPickup?.Invoke(this, other.gameObject);
        }

        if (other.tag == "Fruit")
        {
            ChangeLives(1);
            GainScore(15);
            OnPickup?.Invoke(this, other.gameObject);

        }

        if (other.tag == "Ghost")
        {
            if (!hasPowerup)
            {
                Die();
            }
            else
            {
                GainScore(10);
            }
        }
    }

    private void GainScore(int scoreAdded)
    {
        ScoreIncreased?.Invoke(this, scoreAdded);
    }

    private void ChangeLives(int livesAdded)
    {
        lives += livesAdded;
        OnLivesChanged?.Invoke(this, lives);
    }
}
