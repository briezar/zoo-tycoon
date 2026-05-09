using GameDevKit;
using UnityEngine;

namespace ZooTycoon
{
    [ScriptableObjectResourcesPath("_Project/Resources/ScriptableObjectReferences/ResourceRef")]
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/ScriptableObjectReferences/ResourceSO_Ref", order = 0)]
    public class ResourceSO_Ref : SingletonScriptableObject<ResourceSO_Ref>
    {
        [SerializeField] private ResourceSO _energy, _gold, _gem;

        public static ResourceSO Energy => instance._energy;
        public static ResourceSO Gold => instance._gold;
        public static ResourceSO Gem => instance._gem;
    }
}