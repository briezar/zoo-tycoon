using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon
{
    public class Debris : AdvancedBehaviour
    {
        [field: SerializeField] public DebrisConfig Config { get; private set; }

        [field: SerializeField] public DebrisInteractionUI InteractionUI { get; private set; }

        [SerializeField] private GameObject _clearFx;

        public async UniTask Interact(CancellationToken ct = default)
        {
            InteractionUI.Show();
            InteractionUI.ClearDebrisBtn.text = $"Clear?\n{Config.clearCosts.Select(c => c.GetIconAmountText()).JoinToString("\n")}";
            var isCanceled = await InteractionUI.ClearDebrisBtn.OnClickAsync(ct).SuppressCancellationThrow();
            if (isCanceled)
            {
                InteractionUI.Hide();
            }
        }

        public void Clear()
        {
            Instantiate(_clearFx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    [Serializable]
    public class DebrisConfig
    {
        public ResourceAmount[] clearCosts;
        public SerializableTimeSpan clearTime;
        public ResourceAmount[] clearRewards;
    }
}