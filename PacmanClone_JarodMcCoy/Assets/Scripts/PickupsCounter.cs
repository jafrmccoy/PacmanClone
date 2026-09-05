using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupsCounter : MonoBehaviour
{
    [SerializeField] private List<GameObject> pickups = new List<GameObject>();
    [SerializeField] private List<GameObject> fruits = new List<GameObject>();

    [SerializeField] private float newFruitTime = 10f;
    private float startTime;

    public static event Action<PickupsCounter> OnPickupsGone;

    private void OnEnable()
    {
        ChomperController.OnPickup += DeletePickup;
    }

    private void OnDisable()
    {
        ChomperController.OnPickup -= DeletePickup;
    }

    private void Start()
    {
        foreach (GameObject fruit in fruits)
        {
            fruit.SetActive(false);
        }
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= startTime + newFruitTime && fruits.Count > 0 && !fruits[0].activeInHierarchy)
        {
            fruits[0].SetActive(true);
        }
    }

    private void DeletePickup(ChomperController chomperController, GameObject pickup)
    {
        if (pickup.tag == "Fruit")
        {
            startTime = Time.time;
            fruits.Remove(pickup);
        }
        else
        {
            pickups.Remove(pickup);
        }

        Destroy(pickup);

        if (pickups.Count <= 0)
        {
            OnPickupsGone?.Invoke(this);
        }
    }
}
