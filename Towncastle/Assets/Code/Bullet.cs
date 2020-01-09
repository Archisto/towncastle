using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float lifeTime = 1f;

    private Timer lifeTimer;
    private Vector3 direction;

    private bool initDone;
    private bool isMoving;
    private HelperMethods.Direction cardinalDir = HelperMethods.Direction.Up;

    private bool UnlimitedLifeTime { get { return lifeTime <= 0; } }

    private void InitObject()
    {
        if (initDone)
            return;

        if (!UnlimitedLifeTime)
            lifeTimer = new Timer(lifeTime);

        initDone = true;
    }

    public void Shoot(Vector3 startPosition, Vector3 direction)
    {
        if (!initDone)
            InitObject();

        transform.position = startPosition;
        this.direction = direction;
        isMoving = true;
        gameObject.SetActive(true);

        if (!UnlimitedLifeTime && !lifeTimer.IsActive)
            lifeTimer.Start();
    }

    public void Shoot(HelperMethods.Direction dir)
    {
        Shoot(transform.position, HelperMethods.VectorFromDirection(dir));
    }

    public void Shoot()
    {
        Shoot(cardinalDir);
    }

    public void Stop()
    {
        isMoving = false;
    }

    public void Turn()
    {
        switch (cardinalDir)
        {
            case HelperMethods.Direction.Up:
                cardinalDir = HelperMethods.Direction.Down;
                break;
            case HelperMethods.Direction.Down:
                cardinalDir = HelperMethods.Direction.Up;
                break;
            default:
                cardinalDir = HelperMethods.Direction.Up;
                break;
        }

        Shoot(cardinalDir);
    }

    private void Update()
    {
        if (!initDone)
            return;

        if (isMoving)
            Move(direction);

        if (!UnlimitedLifeTime && lifeTimer.Check())
        {
            lifeTimer.ResetTimer();
            DestroyObject();
        }
    }

    public void Move(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction.normalized * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null && !player.IsDead)
        {
            player.TakeDamage(damage);
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        isMoving = false;
        lifeTimer.ResetTimer();
        gameObject.SetActive(false);
    }
}
