using UnityEngine;

public class Goose : MonoBehaviour
{
    private Vector2 moveTarget;
    [SerializeField] private float moveCooldown;

    [SerializeField] private Vector2 minTarget;
    [SerializeField] private Vector2 maxTarget;
    [SerializeField] private float moveSpeed;

    private float lastMovedTime;

    [SerializeField] private float lifespan = 10f;
    private float spawnTime;

    private bool move;

    private GameObject childObj;

    public string prefix;
    public string suffix;
    [SerializeField] GameObject nametagObj;
    private Nametag nametag;

    public enum BreedState
    {
        CanBreed,
        Cooldown,
        Female,
        Male,
        Baby
    }
    public BreedState state;

    [SerializeField] private float breedCooldown;
    private float lastBredTime;
    private SpriteRenderer sr;
    [SerializeField] private float breedRange;
    public Goose partner;
    [SerializeField] private float breedTime;
    private float breedStartTime;

    public Vector2 partnerLastPos;

    [SerializeField] private float matureTime = 2f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        state = BreedState.Baby;
        move = true;
        moveTarget = (Vector2)transform.position;
        partnerLastPos = Vector2.zero;
        ChangeMoveTarget();
        spawnTime = Time.time;
        childObj = gameObject;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        GameObject tagObj = Object.Instantiate(nametagObj, canvas.transform);
        nametag = tagObj.AddComponent<Nametag>().Init(this, prefix, suffix, tagObj);
        nametag.gameObject.SetActive(true);

        PopulationManager.geese.Add(this.gameObject);
    }

    private void Update()
    {
        if (((Vector2)transform.position - moveTarget).magnitude < 0.1f && Time.time > lastMovedTime + moveCooldown)
        {
            lastMovedTime = Time.time;
            ChangeMoveTarget();
        }

        if (Time.time >= spawnTime + lifespan)
        {
            Die();
        }

        switch (state)
        {
            default:
            case BreedState.Baby:
                Move();
                if (Time.time >= spawnTime + matureTime)
                {
                    state = BreedState.CanBreed;
                }
                break;
            case BreedState.CanBreed:
                if (partner != null)
                {
                    partner = null;
                }
                Move();
                FindPartner();
                break;
            case BreedState.Cooldown:
                Move();
                if (Time.time >= lastBredTime + breedCooldown)
                {
                    state = BreedState.CanBreed;
                }
                break;
            case BreedState.Female:
                if (Time.time >= breedStartTime + breedTime)
                {
                    SpawnChild();
                }
                break;
            case BreedState.Male:
                if (Time.time >= breedStartTime + breedTime)
                {
                    lastBredTime = Time.time;
                    state = BreedState.Cooldown;
                }
                break;
        }

        BreedColors();
    }

    private void FindPartner()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, breedRange);

        foreach (Collider2D collider in colliders)
        {
            Goose potential = collider.GetComponent<Goose>();

            if (potential != null && potential != this && potential.state == BreedState.CanBreed && potential.partner == null && partner == null)
            {
                partner = collider.GetComponent<Goose>();
                partner.partner = this;
                state = BreedState.Female;
                partner.state = BreedState.Male;
                breedStartTime = Time.time;
                partner.breedStartTime = breedStartTime;
                break;
            }
        }
    }

    private void SpawnChild()
    {
        Vector2 spawnPos = Vector2.zero;
        if (partner != null)
        {
            spawnPos = (Vector2)(transform.position + partner.transform.position) / 2f;
        }
        else
        {
            Debug.Log("Fatherless child");
            spawnPos = ((Vector2)transform.position + partnerLastPos) / 2f;
        }

        GameObject child = Object.Instantiate(childObj, spawnPos, Quaternion.identity);
        Goose childGoose = child.GetComponent<Goose>();


        int coinflip = Random.Range(0, 2);
        if (coinflip < 1)
        {
            childGoose.prefix = prefix;
            childGoose.suffix = partner.suffix;
        }
        else
        {
            childGoose.prefix = partner.prefix;
            childGoose.suffix = suffix;
        }

        state = BreedState.Cooldown;
        lastBredTime = Time.time;
    }

    private void BreedColors()
    {
        switch (state)
        {
            default:
            case BreedState.Baby:
                sr.color = Color.pink;
                break;
            case BreedState.CanBreed:
                sr.color = Color.green;
                break;
            case BreedState.Cooldown:
                sr.color = Color.yellow;
                break;
            case BreedState.Male:
                sr.color = Color.blue;
                break;
            case BreedState.Female:
                sr.color = Color.red;
                break;
        }
    }

    private void Die()
    {
        if (partner != null)
        {
            partner.partnerLastPos = transform.position;
        }
        PopulationManager.geese.Remove(this.gameObject);
        Destroy(gameObject);
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
        Vector2 newTarget = Vector2.zero;
        newTarget.x = Random.Range(minTarget.x, maxTarget.x);
        newTarget.y = Random.Range(minTarget.y, maxTarget.y);
        moveTarget = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, breedRange);
    }
}
