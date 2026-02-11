using UnityEngine;

public class SelfDestroyVFX : MonoBehaviour
{
    private new ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particleSystem && !particleSystem.IsAlive())
        {
            SelfDestroy();
        }
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }


    
}
