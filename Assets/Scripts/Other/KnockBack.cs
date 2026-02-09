using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float knockBackForce = 3f;
    [SerializeField] private float knockBackMovMaxTime = 0.3f;

    private float knockBackMovTime;

    private Rigidbody2D rb;

    public bool isGettingBack { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        knockBackMovTime -= Time.deltaTime;

        if(knockBackMovTime < 0 )
        {
            StopKnockBackMov();
        }
    }

    public void GetKnockBack(Transform damageSource)
    {
        isGettingBack = true;
        knockBackMovTime = knockBackMovMaxTime;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackForce / rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
    }



    public void StopKnockBackMov()
    {
        rb.linearVelocity = Vector2.zero;
        isGettingBack = false; 
    }
}
