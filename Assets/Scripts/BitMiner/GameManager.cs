using System;
using UnityEngine;

namespace BitMiner
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Currencies")]
        [SerializeField] private double coins = 0;
        [SerializeField] private double upgradeCurrency = 0;

        public double Coins => coins;
        public double UpgradeCurrency => upgradeCurrency;

        // Events for UI separation
        public event Action<double> OnCoinsChanged;
        public event Action<double> OnUpgradeCurrencyChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddCoins(double amount)
        {
            if (amount <= 0) return;
            coins += amount;
            OnCoinsChanged?.Invoke(coins);
        }

        public bool SpendCoins(double amount)
        {
            if (amount <= 0) return false;
            if (coins >= amount)
            {
                coins -= amount;
                OnCoinsChanged?.Invoke(coins);
                return true;
            }
            return false;
        }

        public void AddUpgradeCurrency(double amount)
        {
            if (amount <= 0) return;
            upgradeCurrency += amount;
            OnUpgradeCurrencyChanged?.Invoke(upgradeCurrency);
        }

        public bool SpendUpgradeCurrency(double amount)
        {
            if (amount <= 0) return false;
            if (upgradeCurrency >= amount)
            {
                upgradeCurrency -= amount;
                OnUpgradeCurrencyChanged?.Invoke(upgradeCurrency);
                return true;
            }
            return false;
        }
    }
}
