using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private PlayerWallet wallet;

    private void Start()
    {
        wallet = FindObjectOfType<PlayerWallet>();
        UpdateMoney(wallet.CurrentMoney);

     
        wallet.OnMoneyChanged += UpdateMoney;
    }

    private void OnDestroy()
    {
        if (wallet != null)
            wallet.OnMoneyChanged -= UpdateMoney;
    }

    private void UpdateMoney(int amount)
    {
        moneyText.text = amount.ToString();
    }
}
