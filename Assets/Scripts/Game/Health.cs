using System;
using UnityEngine.Events;

public class Health
{
    public static UnityEvent DepletedHealthEvent = new UnityEvent();
    public static UnityEvent LowHealthEvent = new UnityEvent();
    public static UnityEvent RecoveredHealthEvent = new UnityEvent();
    public static bool NoFail;

    static int _hp;
    public static int HP {
        get => _hp;
        private set => _hp = Math.Clamp(value, 0, 100);       
    }

    public static void UpdateHP(int amount)
    {
        HP += amount;
        if (HP == 0) DepletedHealthEvent.Invoke();
        if (amount < 0 && HP < 20) LowHealthEvent.Invoke();
        if (amount > 0 && HP > 20) RecoveredHealthEvent.Invoke();
    }

    public static void ResetHP()
    {
        HP = 50;
    }
}