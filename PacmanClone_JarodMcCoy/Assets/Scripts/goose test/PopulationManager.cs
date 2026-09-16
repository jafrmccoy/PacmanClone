using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static List<GameObject> geese = new List<GameObject>();

    public static int population;

    [SerializeField] private int birthRate; //1 in every birthRate seconds

    [SerializeField] private Vector2 spawnXRange;
    [SerializeField] private float spawnX;
    [SerializeField] private Vector2 spawnYRange;
    [SerializeField] private float spawnY;


    [SerializeField] private GameObject goose;

    [SerializeField] private TextMeshProUGUI popCountText;

    private void Update()
    {
        population = geese.Count;

        popCountText.text = "Population: " + population;
    }

    private IEnumerator GooseSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(birthRate);

            SpawnGoose();
        }
    }

    private void SpawnGoose()
    {
        int direction = Random.Range(1, 4);
        //top, right, down, bottom

        float posX = 0f;
        float posY = 0f;
        Vector2 spawnPos = Vector2.zero;

        switch (direction)
        {
            default:
            case 1:
                posX = Random.Range(spawnXRange.x, spawnXRange.y);
                posY = spawnY;
                break;
            case 2:
                posX = spawnX;
                posY = Random.Range(spawnYRange.x, spawnYRange.y);
                break;
            case 3:
                posX = -1f * posX;
                posY = Random.Range(spawnYRange.x, spawnYRange.y);
                break;
            case 4:
                posX = Random.Range(spawnXRange.x, spawnXRange.y);
                posY = -1f * spawnY;
                break;
        }
    }
}
