using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float lifeTime = 1f;

#pragma warning restore 0649

    private Timer lifeTimer;
    private Vector3 direction;

    private bool initDone;
    private bool isMoving;
    private Utils.Direction cardinalDir = Utils.Direction.Up;

    private bool UnlimitedLifeTime { get => lifeTime <= 0; }

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

    public void Shoot(Utils.Direction dir)
    {
        Shoot(transform.position, Utils.VectorFromDirection(dir));
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
            case Utils.Direction.Up:
                cardinalDir = Utils.Direction.Down;
                break;
            case Utils.Direction.Down:
                cardinalDir = Utils.Direction.Up;
                break;
            default:
                cardinalDir = Utils.Direction.Up;
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
