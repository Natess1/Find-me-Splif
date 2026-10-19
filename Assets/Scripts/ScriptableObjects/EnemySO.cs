using UnityEngine;


[CreateAssetMenu()]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int enemyHealth;
    public int enemyDamageAmount;
    public int haveMoney;
    public int xpReward = 5;   // сколько опыта даёт за убийство

}
