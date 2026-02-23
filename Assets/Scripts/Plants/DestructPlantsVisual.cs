using UnityEngine;

public class DestructPlantsVisual : MonoBehaviour
{
    [SerializeField] private DestructPlants destructablePlants;
    [SerializeField] private GameObject bushDeathVFX;

    private void Start()
    {
        destructablePlants.OnDestructTakeDamage += DestructPlant_OnDestructablePlantsTakeDamage;
    }

    private void DestructPlant_OnDestructablePlantsTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(bushDeathVFX, destructablePlants.transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        destructablePlants.OnDestructTakeDamage -= DestructPlant_OnDestructablePlantsTakeDamage;
    }





}
