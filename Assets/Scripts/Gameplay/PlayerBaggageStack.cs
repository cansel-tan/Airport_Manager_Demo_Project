using System.Collections.Generic;
using UnityEngine;

public class PlayerBaggageStack : MonoBehaviour
{
    [Header("Stack Root")]
    public Transform stackRoot;          
    public float verticalOffset = 0.4f;  

    private readonly List<Transform> _stack = new List<Transform>();

    public int Count => _stack.Count;

    public void Add(Transform bag)
    {
        if (bag == null) return;
        if (_stack.Contains(bag)) return;

        _stack.Add(bag);

        // Oyuncunun arkasına parent'la
        bag.SetParent(stackRoot, true);

        
        Vector3 localPos = Vector3.up * verticalOffset * (_stack.Count - 1);
        bag.localPosition = localPos;

        Debug.Log($"[Stack] Add -> {_stack.Count} bag(s) now.");
    }

    
    public void DropAllStacked(Transform root, float heightStep)
    {
        Debug.Log($"[Stack] DropAllStacked called. Stack size = {_stack.Count}");

        if (root == null)
        {
            Debug.LogError("[Stack] DropAllStacked: root is NULL!");
            return;
        }

        if (_stack.Count == 0)
        {
            Debug.Log("[Stack] DropAllStacked: stack empty.");
            return;
        }

        for (int i = 0; i < _stack.Count; i++)
        {
            Transform bag = _stack[i];
            if (bag == null) continue;

           
            Vector3 worldPos = root.position + Vector3.up * heightStep * i;

            bag.SetParent(null, true);          
            bag.position = worldPos;            
            bag.SetParent(root, true);          

            Debug.Log($"[Stack] Bag {i} worldPos = {bag.position}");
        }

        _stack.Clear();
    }
}
