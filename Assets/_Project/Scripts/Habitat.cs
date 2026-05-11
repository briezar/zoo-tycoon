using System.Collections.Generic;
using UnityEngine;

namespace ZooTycoon
{
    public class Habitat : MonoBehaviour
    {
        [SerializeField] private Area _wanderArea;
        [SerializeField] private int _capacity = 5;

        private readonly List<Animal> _animals = new();

        public bool IsFull => _animals.Count >= _capacity;

        public bool AddAnimal(Animal animal)
        {
            if (IsFull) { return false; }
            _animals.Add(animal);

            if (!_wanderArea.Contains(animal.transform.position))
            {
                animal.Agent.Warp(_wanderArea.GetRandomPoint());
            }

            if (animal.TryGetComponent<Wanderable>(out var wanderable)) { wanderable.StartWandering(_wanderArea); }
            return true;
        }

        public bool RemoveAnimal(Animal animal)
        {
            if (!_animals.Remove(animal)) { return false; }
            if (animal.TryGetComponent<Wanderable>(out var wanderable)) { wanderable.StopWandering(); }
            return true;
        }
    }
}