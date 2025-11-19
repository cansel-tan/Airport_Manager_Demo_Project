using System;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSpawnZone : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public Transform baseSpawnPoint;      
    public int passengerCount = 6;      
    public float spacing = 1.2f;          

    [Header("Zemin Referansı (opsiyonel)")]
    public Transform groundRef;

    [Header("Prefab ve Parent")]
    public GameObject passengerPrefab;    
    public Transform passengersParent;    

    public PassengerBoardingManager boardingManager;

    [Header("Opsiyonel: Kuyruk kontrolü")]
    public PassengersLineController lineController;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasSpawned) return;

        hasSpawned = true;
        Debug.Log("[PassengerSpawnZone] Player girdi, yolcular spawn edilecek.");

        SpawnPassengers();
    }

    private void SpawnPassengers()
    {
        if (passengerPrefab == null || baseSpawnPoint == null) return;

        Vector3 dir = baseSpawnPoint.forward;
        var spawnedList = new List<Transform>();

        for (int i = 0; i < passengerCount; i++)
        {
            Vector3 offset = dir.normalized * spacing * i;
            Vector3 spawnPos = baseSpawnPoint.position + offset;

            // Y eksenini zemine sabitle
            if (groundRef != null)
                spawnPos.y = groundRef.position.y - 4f;
            else
                spawnPos.y = baseSpawnPoint.position.y;

            GameObject clone = Instantiate(
                passengerPrefab,
                spawnPos,
                baseSpawnPoint.rotation
                
            );
            clone.transform.parent = passengersParent;
            var pComponent = clone.GetComponentInChildren<Passengers>();
            Transform registeredTransform = (pComponent != null) ? pComponent.transform : clone.transform;

            Debug.Log($"[PassengerSpawnZone] Spawned {clone.name} (index {i}) at {spawnPos} - registeredTransform = {registeredTransform.name}");

            if (registeredTransform != null && boardingManager != null)
            {
                boardingManager.RegisterPassenger(registeredTransform);
                Debug.Log($"[PassengerSpawnZone] Registered {registeredTransform.name} to BoardingManager");
            }
            else if (boardingManager == null)
            {
                Debug.LogWarning("[PassengerSpawnZone] boardingManager atanmadı! Register yapılmadı.");
            }

            spawnedList.Add(registeredTransform);
        }

        if (lineController != null)
        {
            lineController.SetPassengers(spawnedList);
        }

        Debug.Log($"[PassengerSpawnZone] {passengerCount} yolcu spawn edildi.");
    }
}