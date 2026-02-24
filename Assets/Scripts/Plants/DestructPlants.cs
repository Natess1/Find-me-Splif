using System;
using Unity.VisualScripting;
using UnityEngine;
public class DestructPlants : MonoBehaviour
{
    [SerializeField] private int haveMoney;
    public event EventHandler OnDestructTakeDamage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            OnDestructTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
            GameInput.Instance.AddMoney(haveMoney);
            NavSurfManagment.Instance.RebakeNavMashSurface();
        }

    }


}
