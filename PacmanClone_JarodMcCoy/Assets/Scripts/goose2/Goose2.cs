using System.Collections;
using UnityEngine;

public class Goose2 : MonoBehaviour
{
    private Vector2 moveTarget;
    [SerializeField] private Vector2 moveCooldown;
    private float realCooldown;
    [SerializeField] private Vector2 minTarget;
    [SerializeField] private Vector2 maxTarget;
    [SerializeField] private float moveSpeed;
    private float lastMovedTime;
    public float birthday {get; private set;}

    [SerializeField] private GameObject nametagObj;
    public string prefix;
    public string suffix;
    private Nametag nametag;

    private bool move;

    [SerializeField] private bool firstGoose;
    [SerializeField] private int chanceToDie; //1 in chanceToDie chance to die


    private void Start()
    {
        move = true;
        lastMovedTime = Time.time;
        moveTarget = (Vector2)transform.position;
        ChangeMoveTarget();

        prefix = NameBank.nameBank.GetPrefix();
        suffix = NameBank.nameBank.GetSuffix();

        Canvas canvas = FindAnyObjectByType<Canvas>();
        GameObject tagObj = Object.Instantiate(nametagObj, canvas.transform);
        nametag = tagObj.AddComponent<Nametag>().Init(this.gameObject, prefix, suffix, tagObj);
        nametag.gameObject.SetActive(true);

        PopulationManager.geese.Add(this.gameObject);
        birthday = Time.time;

        if (!firstGoose)
        {
            StartCoroutine(CheckForDeath());
        }
    }

    private void Update()
    {
        if (move && ((Vector2)transform.position - moveTarget).magnitude < 0.1f)
        {
            move = false;
            lastMovedTime = Time.time;
        }

        if (!move && Time.time >= lastMovedTime + realCooldown)
        {
            ChangeMoveTarget();
            move = true;
        }

        Move();
    }

    private void Move()
    {
        if (move)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
        }
    }

    private void ChangeMoveTarget()
    {
        realCooldown = Random.Range(moveCooldown.x, moveCooldown.y);

        Vector2 newTarget = Vector2.zero;
        newTarget.x = Random.Range(minTarget.x, maxTarget.x);
        newTarget.y = Random.Range(minTarget.y, maxTarget.y);
        moveTarget = newTarget;
    }

    private IEnumerator CheckForDeath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            RollDeath();
        }
    }

    private void RollDeath()
    {
        int roll = Random.Range(1, chanceToDie + 1);
        if (roll == chanceToDie)
        {
            Die();
        }
    }

    private void Die()
    {
        PopulationManager.geese.Remove(this.gameObject);
        Destroy(gameObject);
    }
}
