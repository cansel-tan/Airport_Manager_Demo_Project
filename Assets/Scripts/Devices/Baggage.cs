using UnityEngine;

public class Baggage : MonoBehaviour
{
    public bool collected;

    public void Collect(PlayerBaggageStack stack)
    {
        if (collected || stack == null) return;

        collected = true;
        stack.Add(transform);
    }
}
