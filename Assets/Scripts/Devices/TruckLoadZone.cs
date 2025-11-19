using UnityEngine;

public class TruckLoadZone : MonoBehaviour
{
    public BaggageTruckLoader loader;

    private bool _used;

    private void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;
        loader.StartLoading();
    }
}
