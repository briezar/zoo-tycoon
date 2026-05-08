using EditorAttributes;
using UnityEngine;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Currency")]
    public class CurrencySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: AssetPreview(24, 24)]
        [field: SerializeField] public Sprite Icon { get; private set; }

    }
}
