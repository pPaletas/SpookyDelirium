using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct EnemyType
{
    public GameObject enemyType;
    [Range(0, 100)] public int spawnChance;
}

public class FightingScenarioManager : MonoBehaviour
{
    [SerializeField] private EnemyType[] _enemyTypes;
    [SerializeField] private GameObject _presentationEnemy;

    private List<Transform> _enemySpawners = new List<Transform>();
    private GameObject _walls;
    private GameObject _trigger;
    private Transform _enemiesParent;

    private bool _fightingStarted = false;

    public void StartScenario()
    {
        _fightingStarted = true;
        _trigger.SetActive(false);
        _walls.SetActive(true);
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (Transform spawner in _enemySpawners)
        {
            GameObject enemyClone = Instantiate(GetNextEnemyToSpawn(), spawner.position, spawner.rotation);
            enemyClone.transform.SetParent(_enemiesParent);
            enemyClone.GetComponentInChildren<SpiderStateMachine>().TriggerEnemy();
        }
    }

    private GameObject GetNextEnemyToSpawn()
    {
        // Revisa todas las opciones menos la última
        for (int i = 0; i < _enemyTypes.Length - 1; i++)
        {
            int randomValue = Random.Range(0, 100);
            bool isNext = randomValue <= _enemyTypes[i].spawnChance;

            if (isNext) return _enemyTypes[i].enemyType;
        }

        // Si no cae ninguno, entonces devuelve el último
        return _enemyTypes[_enemyTypes.Length - 1].enemyType;
    }

    private void CheckIfAllEnemiesHaveBeenDefeated()
    {
        if (_fightingStarted && _enemiesParent.childCount <= 0f)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        _enemySpawners.AddRange(transform.Find("EnemySpawners").GetComponentsInChildren<Transform>());
        // Remueve el padre
        _enemySpawners.Remove(transform.Find("EnemySpawners"));

        _walls = transform.Find("Walls").gameObject;
        _trigger = transform.Find("Trigger").gameObject;
        _enemiesParent = transform.Find("Enemies");
    }

    private void Update()
    {
        CheckIfAllEnemiesHaveBeenDefeated();
    }
}