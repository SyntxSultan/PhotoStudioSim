using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startingBalance = 0;
    private int balance;

    public event Action<int> OnBalanceChanged;

    public int GetMoney() => balance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        balance = startingBalance;
    }
    
    public void AddCurrency(int amount)
    {
        balance += amount;
        OnBalanceChanged?.Invoke(balance);
        Debug.Log($"[Currency] +{amount} → {balance}");
    }
    
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (balance < amount)
        {
            Debug.Log($"[Currency] Yetersiz bakiye. Gerekli: {amount}, Mevcut: {balance}");
            return false;
        }

        balance -= amount;
        OnBalanceChanged?.Invoke(balance);
        Debug.Log($"[Currency] -{amount} → {balance}");
        return true;
    }

    public bool CanBuy(int amount) => balance >= amount;
}