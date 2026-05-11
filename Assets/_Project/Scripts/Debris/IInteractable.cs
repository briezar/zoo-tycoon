using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZooTycoon
{
    public interface IInteractable
    {
        UniTask Interact(object source, CancellationToken ct = default);
    }

    public interface IInteractionUI
    {
        public static readonly HashSet<IInteractionUI> ActiveUIs = new();

        UniTask Show();
        UniTask Hide();
    }


    /// <summary>
    /// When this UI is shown, all other interaction UIs are hidden.
    /// </summary>
    public interface IModalInteractionUI : IInteractionUI
    {
        public static void HideAll()
        {
            foreach (var activeUI in ActiveUIs)
            {
                if (activeUI is IModalInteractionUI)
                {
                    activeUI.Hide();
                }
            }
        }
    }
}