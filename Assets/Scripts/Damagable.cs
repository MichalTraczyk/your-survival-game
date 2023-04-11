using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public UnityEvent onDieEvent;
    public UnityEvent<int> onDamageEvent;

    public Slider hpSlider;
    public int startHp;
    public int currHp;

    EnemyBehaviour behaviour;
    // Start is called before the first frame update
    void Start()
    {
        behaviour = GetComponent<EnemyBehaviour>();
        if (onDieEvent == null)
            onDieEvent = new UnityEvent();
        if (startHp != 0)
            Setup(startHp);
    }

    public void Setup(int start)
    {
        if (start != 0)
            currHp = start;
        startHp = start;
        if (hpSlider != null)
        {
            hpSlider.maxValue = start;
            hpSlider.value = currHp;
        }
    }

    public void Damage(int dmg,WeaponType weaponType = WeaponType.Rifle)
    {
        if(weaponType == WeaponType.Melee&& behaviour != null)
        {
            behaviour.OnSwordHit();
        }

        currHp -= dmg;
        onDamageEvent.Invoke(dmg);

        if (currHp <= 0)
        {
            onDieEvent.Invoke();
        }
        if(hpSlider != null)
            hpSlider.value = currHp;
    }

    public void Test(int xd)
    {
        Debug.Log("damage: " + xd);
    }
}
