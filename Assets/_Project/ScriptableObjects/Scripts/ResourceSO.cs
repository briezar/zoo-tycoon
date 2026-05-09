using EditorAttributes;
using UnityEngine;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }

        // [field: AssetPreview(24, 24)]
        [field: SerializeField] public Sprite Icon { get; private set; }

    }
}
