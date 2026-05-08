using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ZooTycoon
{
    public class Habitat : MonoBehaviour
    {
        [SerializeField] private Collider _wanderArea;

        private readonly List<Animal> _animals = new();

        public void AddAnimal(Animal animal)
        {
            
        }

    }
}
