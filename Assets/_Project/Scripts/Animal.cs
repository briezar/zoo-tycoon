using UnityEngine;
using UnityEngine.AI;

namespace ZooTycoon
{
    public class Animal : MonoBehaviour
    {
        [field: SerializeField] public AnimalDefinitionSO Definition { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }

        public void SetDefinition(AnimalDefinitionSO definition) => Definition = definition;
    }
}