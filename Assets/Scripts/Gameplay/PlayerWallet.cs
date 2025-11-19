using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    public int CurrentMoney { get; private set; }

    public event Action<int> OnMoneyChanged;

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        OnMoneyChanged?.Invoke(CurrentMoney);   
        Debug.Log("Money: " + CurrentMoney);
    }
}
