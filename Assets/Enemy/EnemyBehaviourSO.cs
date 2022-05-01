using UnityEngine;
public enum Behaviour
{
    Neutral,
    Aggresive,
    Agonal
}
public enum speedType
{
    Runner,
    Walker,
    Crawl
}
[CreateAssetMenu(fileName = "EnemyBehaviour", menuName = "ScriptableObjects/Enemy", order = 0)]
public class EnemyBehaviourSO: ScriptableObject
{
    [Range(1,50)]
    public int distanceToTrigger;

    public Behaviour enemyBehaviour;
    public int damage;
    public float speed;
    public speedType speedType;
    public float distanceToAttack = 2;
    public int baseHp;

}