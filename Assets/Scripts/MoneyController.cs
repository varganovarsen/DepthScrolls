using System;
using UnityEngine;

public static class MoneyController
{
    static int money = 0;

    public static int Money
    {
        get => money;
        set
        {
            money = value;
            OnMoneyChanged?.Invoke(money);
        }
    }

    public static event Action<int> OnMoneyChanged;
    
}
