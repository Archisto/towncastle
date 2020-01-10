using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private int maxHitpoints = 10;

    [SerializeField]
    private float speed = 0.2f;

    [SerializeField]
    private Material defaultMaterial;

    [SerializeField]
    private Material hurtMaterial;

#pragma warning restore 0649

    private MeshRenderer rend;

    public int Hitpoints { get; private set; }

    public int MaxHitpoints { get { return maxHitpoints; } }

    public bool IsDead { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        Hitpoints = maxHitpoints;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void Move(Vector3 direction)
    {
        if (IsDead)
            return;

        Vector3 newPosition = transform.position + direction.normalized * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    public bool TakeDamage(int amount)
    {
        if (IsDead)
            return false;

        if (Hitpoints == MaxHitpoints)
        {
            rend.material = hurtMaterial;
        }
        
        Hitpoints -= amount;

        if (Hitpoints <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    public void Die()
    {
        Debug.Log("Player died!");
        Hitpoints = 0;
        IsDead = true;
        GameManager.Instance.EndGame();
    }

    public void ResetPlayer()
    {
        Hitpoints = MaxHitpoints;
        IsDead = false;
        rend.material = defaultMaterial;
    }

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Hitpoints == MaxHitpoints)
        {
            Gizmos.color = Color.green;
        }
        else if (!IsDead)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.5f);
    }
}
