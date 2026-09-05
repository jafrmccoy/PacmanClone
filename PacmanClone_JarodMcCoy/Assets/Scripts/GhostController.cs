using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{
    [SerializeField] private ChomperController chomper;

    [SerializeField] private float moveSpeed;

    [SerializeField] private float respawnCooldown = 4f;
    private float respawnTime;

    private NavMeshAgent navmeshAgent;

    [SerializeField] private GameObject[] models; //the children with meshes and lights

    [SerializeField] private Light[] lights;
    private Color[] defaultLightColors;
    [SerializeField] private Color[] freezeLightColors;

    private Material defaultMaterial;
    [SerializeField] private Material freezeMaterial;
    private SkinnedMeshRenderer smr;

    private bool playerHasPowerup;

    private float timeSinceStart;

    private enum GhostState
    {
        Chase, //chase chomper
        Freeze, //freeze, when chomper gets pickup
        Start, //starting, stuck in spawn box
        Respawn //respawn, when after getting eaten
    }

    private GhostState state;

    [SerializeField] private float leaveStartTime = 3f; //time before leaves spawn box
    private float startTime;

    public Vector3 startingPos;

    private void OnEnable()
    {
        ChomperController.OnGetPowerup += PlayerGotPowerup;
        ChomperController.OnLosePowerup += PlayerLostPowerup;
        ChomperController.OnPlayerDied += PlayerDied;
    }

    private void OnDisable()
    {
        ChomperController.OnGetPowerup -= PlayerGotPowerup;
        ChomperController.OnLosePowerup -= PlayerLostPowerup;
        ChomperController.OnPlayerDied -= PlayerDied;
    }

    private void Start()
    {
        playerHasPowerup = false;

        navmeshAgent = GetComponent<NavMeshAgent>();
        navmeshAgent.speed = moveSpeed;
        startingPos = transform.position;

        defaultLightColors = new Color[lights.Length + 1];

        for (int i = 0;  i < lights.Length; i++)
        {
            defaultLightColors[i] = lights[i].color;
        }

        smr = models[0].GetComponent<SkinnedMeshRenderer>();
        defaultMaterial = smr.material;

        StartGhost();
    }

    private void Update()
    {
        SetGhostColors();

        switch (state)
        {
            default:
            case GhostState.Chase:
                //follow chomper
                navmeshAgent.SetDestination(chomper.transform.position);
                break;
            case GhostState.Freeze:
                //freeze
                navmeshAgent.SetDestination(transform.position);
                break;
            case GhostState.Start:
                //start
                if (Time.time >= startTime + leaveStartTime)
                {
                    state = GhostState.Chase;
                }
                else
                {
                    navmeshAgent.SetDestination(transform.position);
                }
                break;
            case GhostState.Respawn:
                if (Time.time >= respawnTime + respawnCooldown)
                {
                    RestartGhost();
                }
                break;
        }
    }

    private void PlayerGotPowerup(ChomperController chomperController)
    {
        playerHasPowerup = true;

        timeSinceStart = Time.time - startTime;
        state = GhostState.Freeze;
    }

    private void PlayerLostPowerup(ChomperController chomperController)
    {
        playerHasPowerup = false;

        SetGhostColors();
        foreach (GameObject obj in models)
        {
            obj.SetActive(true);
        }

        if (timeSinceStart >= startTime + leaveStartTime)
        {
            state = GhostState.Chase;
        }
        else
        {
            state = GhostState.Start;
        }
    }

    private void PlayerDied(ChomperController chomperController, int lives)
    {
        StartGhost();
    }

    private void StartGhost()
    {
        //turn models on
        foreach (GameObject obj in models)
        {
            obj.SetActive(true);
        }

        navmeshAgent.Warp(startingPos);
        transform.rotation = Quaternion.identity;
        startTime = Time.time;
        timeSinceStart = Time.time - startTime;
        state = GhostState.Start;
    }

    private void RespawnGhost()
    {
        //turn models off
        foreach (GameObject obj in models)
        {
            obj.SetActive(false);
        }

        respawnTime = Time.time;
        navmeshAgent.Warp(startingPos);
        transform.rotation = Quaternion.identity;
        state = GhostState.Respawn;
    }

    private void RestartGhost()
    {
        foreach (GameObject obj in models)
        {
            obj.SetActive(true);
        }
        SetGhostColors();

        navmeshAgent.Warp(startingPos);
        transform.rotation = Quaternion.identity;
        if (playerHasPowerup)
        {
            state = GhostState.Freeze;
        }
        else
        {
            state = GhostState.Start;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && state == GhostState.Freeze)
        {
            RespawnGhost();
        }
    }

    private void SetGhostColors()
    {
        if (state == GhostState.Freeze && smr.material == defaultMaterial)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = freezeLightColors[i];
            }

            smr.material = freezeMaterial;
        }
        else if (state != GhostState.Freeze && smr.material != defaultMaterial)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = defaultLightColors[i];
            }

            smr.material = defaultMaterial;
        }
    }
}
