using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Es un manager de FightScenarios?
        if (other.transform.parent.TryGetComponent<FightingScenarioManager>(out FightingScenarioManager manager))
        {
            manager.StartScenario();
        }
    }
}
