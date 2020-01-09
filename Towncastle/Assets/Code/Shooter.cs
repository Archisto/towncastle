using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    public List<Bullet> bullets;

    [SerializeField]
    private Vector3 areaCorner1;

    [SerializeField]
    private Vector3 areaCorner2;

    [SerializeField]
    private float shootPosBackOffset;

    [SerializeField]
    private float shootInterval_easiest = 1f;

    [SerializeField]
    private float shootInterval_hardest = 0.3f;

    [SerializeField]
    private float timeTillMaxDifficulty = 20f;

#pragma warning restore 0649

    private Timer difficultyTimer;

    private float elapsedTime;

    public float shootInterval;

    // Start is called before the first frame update
    private void Start()
    {
        difficultyTimer = new Timer(timeTillMaxDifficulty);
        difficultyTimer.Start();
        UpdateShootInterval();
    }

    // Update is called once per frame
    private void Update()
    {
        difficultyTimer.Check();

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shootInterval)
        {
            Shoot();
            elapsedTime = 0f;
            UpdateShootInterval();
        }
    }

    private void UpdateShootInterval()
    {
        shootInterval = shootInterval_hardest + (1 - difficultyTimer.Ratio) * (shootInterval_easiest - shootInterval_hardest);
    }

    private void Shoot()
    {
        foreach (Bullet b in bullets)
        {
            if (!b.gameObject.activeSelf)
            {
                float x = areaCorner1.x;
                float z = areaCorner1.z;
                Vector3 direction = Vector3.zero;
                int side = Random.Range(0, 4);
                switch (side)
                {
                    case 0:
                        x = areaCorner1.x - shootPosBackOffset;
                        z = Random.Range(areaCorner1.z, areaCorner2.z);
                        direction = Vector3.right;
                        break;
                    case 1:
                        x = areaCorner2.x + shootPosBackOffset;
                        z = Random.Range(areaCorner1.z, areaCorner2.z);
                        direction = Vector3.left;
                        break;
                    case 2:
                        x = Random.Range(areaCorner1.x, areaCorner2.x);
                        z = areaCorner1.z - shootPosBackOffset;
                        direction = Vector3.forward;
                        break;
                    case 3:
                        x = Random.Range(areaCorner1.x, areaCorner2.x);
                        z = areaCorner2.z + shootPosBackOffset;
                        direction = Vector3.back;
                        break;
                }
                
                b.Shoot(new Vector3(x, 1, z), direction);
                return;
            }
        }
    }

    public void ResetShooter()
    {
        DestroyAllBullets();
        difficultyTimer.Start();
        elapsedTime = 0f;
    }

    public void DestroyAllBullets()
    {
        foreach (Bullet b in bullets)
        {
            if (b.gameObject.activeSelf)
                b.DestroyObject();
        }
    }

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (areaCorner1 != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(areaCorner1, areaCorner1 + Vector3.up * 2f);
        }

        if (areaCorner2 != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(areaCorner2, areaCorner2 + Vector3.up * 2f);
        }
    }
}
