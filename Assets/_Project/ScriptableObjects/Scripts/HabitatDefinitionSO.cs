using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Habitat")]
    public class HabitatDefinitionSO : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public GameObject PreviewPrefab { get; private set; }
        [field: SerializeField] public Habitat HabitatPrefab { get; private set; }

        [field: SerializeField] public ResourceAmount[] BuildCosts { get; private set; }
    }
}
