using UnityEngine;
using UnityEngine.Rendering;

public class DestructPlantsVisual : MonoBehaviour
{
    //DestractPlants = DestructablePlants - Объект, который мы уничтожаем. 
    [SerializeField] private DestructPlants destructPlants;
    [SerializeField] private GameObject bushDeathVFX;

    private void Start()
    {
        destructPlants.OnDestructTakeDamage += DestructPlant_OnDestructablePlantsTakeDamage;
    }

    private void DestructPlant_OnDestructablePlantsTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(bushDeathVFX, destructPlants.transform.position, Quaternion.identity);

    }

    private void OnDestroy()
    {
        destructPlants.OnDestructTakeDamage -= DestructPlant_OnDestructablePlantsTakeDamage;
    }





}
