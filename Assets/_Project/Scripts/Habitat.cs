using System.Collections.Generic;
using UnityEngine;

namespace ZooTycoon
{
    public class Habitat : MonoBehaviour
    {
        [SerializeField] private Area _wanderArea;

        private readonly List<Animal> _animals = new();

        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
            if (animal.TryGetComponent<Wanderable>(out var wanderable))
            {
                wanderable.StartWandering(_wanderArea);
            }
        }

        public void RemoveAnimal(Animal animal)
        {
            if (!_animals.Remove(animal)) { return; }
            if (animal.TryGetComponent<Wanderable>(out var wanderable))
            {
                wanderable.StopWandering();
            }
        }
    }
}
