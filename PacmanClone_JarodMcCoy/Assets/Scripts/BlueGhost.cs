using UnityEngine;

public class BlueGhost : Ghost
{
    public BlueGhost() : base()
    {
        _startCooldown = 4f;
    }

    protected override void Move()
    {
        _target = transform.position;

        _navMeshAgent.SetDestination(_target);
    }
}
