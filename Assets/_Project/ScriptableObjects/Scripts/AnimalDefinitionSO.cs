using UnityEngine;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/AnimalDefinition")]
    public class AnimalDefinitionSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [SerializeField] private Animal _prefab;

        public Animal Spawn(Transform parent = null)
        {
            var animal = Instantiate(_prefab, parent);
            animal.SetDefinition(this);
            return animal;
        }
    }
}
