using UnityEngine;
using UnityEngine.AI;

public abstract class Ghost : MonoBehaviour
{
    protected float _moveSpeed;
    protected float _respawnCooldown;
    protected float _respawnTime;
    protected NavMeshAgent _navMeshAgent;
    protected bool _playerHasPowerup;
    protected float _timeSinceStart;

    protected enum GhostState
    {
        Chase, //chase chomper
        Freeze, //freeze, when chomper gets pickup
        Start, //starting, stuck in spawn box
        Respawn //respawn, when after getting eaten
    }

    protected GhostState _state;

    protected float _startCooldown;
    protected float _startTime;

    protected Vector3 _startingPos;

    protected Vector3 _target;
    protected ChomperController _chomper;

    public Ghost()
    {
        _moveSpeed = 9f;
        _respawnCooldown = 4f;
        _navMeshAgent = new NavMeshAgent();
        _navMeshAgent.speed = _moveSpeed;
        _playerHasPowerup = false;
        _timeSinceStart = 0f;
        //_startCooldown = 4f; -- Set this in babies
        _startTime = Time.time;
        _startingPos = transform.position;
    }

    private void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        _target = _chomper.transform.position;

        switch (_state)
        {
            default:
            case GhostState.Chase:
                //follow chomper
                _navMeshAgent.SetDestination(_target);
                break;
            case GhostState.Freeze:
                //freeze
                _navMeshAgent.SetDestination(transform.position);
                break;
            case GhostState.Start:
                //start
                if (Time.time >= _startTime + _startCooldown)
                {
                    _state = GhostState.Chase;
                }
                else
                {
                    _navMeshAgent.SetDestination(transform.position);
                }
                break;
            case GhostState.Respawn:
                if (Time.time >= _respawnTime + _respawnCooldown)
                {
                    RestartGhost();
                }
                break;
        }
    }

    private void OnEnable()
    {
        ChomperController.OnGetPowerup += PlayerGotPowerup;
        ChomperController.OnLosePowerup += PlayerLostPowerup;
        ChomperController.OnPlayerDied += PlayerDied;
        ChomperController.OnGameStarted += GetPlayer;
    }

    private void OnDisable()
    {
        ChomperController.OnGetPowerup -= PlayerGotPowerup;
        ChomperController.OnLosePowerup -= PlayerLostPowerup;
        ChomperController.OnPlayerDied -= PlayerDied;
        ChomperController.OnGameStarted -= GetPlayer;
    }

    private void GetPlayer(ChomperController chomperController)
    {
        _chomper = chomperController;
    }

    private void PlayerGotPowerup(ChomperController chomperController)
    {
        _playerHasPowerup = true;

        _timeSinceStart = Time.time - _startTime;
        _state = GhostState.Freeze;
    }

    private void PlayerLostPowerup(ChomperController chomperController)
    {
        _playerHasPowerup = false;

        //SetGhostColors();
        //foreach (GameObject obj in models)
        //{
        //    obj.SetActive(true);
        //}

        if (_timeSinceStart >= _startTime + _startCooldown)
        {
            _state = GhostState.Chase;
        }
        else
        {
            _state = GhostState.Start;
        }
    }

    private void PlayerDied(ChomperController chomperController, int lives)
    {
        StartGhost();
    }

    private void StartGhost()
    {
        //turn models on
        //foreach (GameObject obj in models)
        //{
        //    obj.SetActive(true);
        //}

        _navMeshAgent.Warp(_startingPos);
        transform.rotation = Quaternion.identity;
        _startTime = Time.time;
        _timeSinceStart = Time.time - _startTime;
        _state = GhostState.Start;
    }

    private void RespawnGhost()
    {
        //turn models off
        //foreach (GameObject obj in models)
        //{
        //    obj.SetActive(false);
        //}

        _respawnTime = Time.time;
        _navMeshAgent.Warp(_startingPos);
        transform.rotation = Quaternion.identity;
        _state = GhostState.Respawn;
    }

    private void RestartGhost()
    {
        //foreach (GameObject obj in models)
        //{
        //    obj.SetActive(true);
        //}
        //SetGhostColors();

        _navMeshAgent.Warp(_startingPos);
        transform.rotation = Quaternion.identity;
        if (_playerHasPowerup)
        {
            _state = GhostState.Freeze;
        }
        else
        {
            _state = GhostState.Start;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && _state == GhostState.Freeze)
        {
            RespawnGhost();
        }
    }
}
