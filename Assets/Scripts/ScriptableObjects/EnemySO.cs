using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/Enemy")]
public class EnemySO : ScriptableObject
{
    public float maxMagSize;
    public float maxHealth;
    public float minRangeToShoot;
    public float maxRangeToShoot;
    public float rateOfFire;
}
