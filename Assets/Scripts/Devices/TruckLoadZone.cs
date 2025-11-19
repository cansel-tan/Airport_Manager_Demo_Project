using UnityEngine;
using UnityEngine.UI;

public class TruckLoadZone : MonoBehaviour
{
    public BaggageTruckLoader loader;
    [Header("References")]
    public PlayerBaggageStack playerStack;
    public GameObject esBlock;

    private bool _used;

    private void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;
        if(playerStack != null && playerStack.isDropped == false)
        {
            Debug.Log("[TruckLoadZone] There is no baggage, cannot load truck.");
            return;
        }
        if(esBlock != null)
        {
            esBlock.SetActive(false);
        }
        _used = true;
        loader.StartLoading();
    }
}
