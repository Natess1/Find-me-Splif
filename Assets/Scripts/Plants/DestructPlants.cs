using System;
using UnityEngine;

public class DestructPlants : MonoBehaviour
{
    public event EventHandler OnDestructTakeDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            OnDestructTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);

            NavSurfManagment.Instance.RebakeNavMashSurface();
        }
    }
}
