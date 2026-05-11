using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using ZooTycoon.Input;

namespace ZooTycoon.UI
{
    public class DialogueOverlay : OverlayUI
    {
        [SerializeField] private TMP_Text _dialogueText;

        public string DialogueText
        {
            get => _dialogueText.text;
            set => _dialogueText.text = value;
        }

        protected override async UniTask OnShow()
        {
            InputManager.Enable_PlayerMovement(false);
            Anim.FadeIn();
        }

        protected override async UniTask OnHide()
        {
            await Anim.FadeOut();
            InputManager.Enable_PlayerMovement(true);
        }

        public async UniTask TypeText(string text)
        {
            _dialogueText.text = text;
            _dialogueText.ForceMeshUpdate();
            var typeTween = Tween.TextMaxVisibleCharacters(_dialogueText, 0, text.Length, 1f, Ease.Linear);

            await UniTask.WaitForSeconds(0.1f);
            while (true)
            {
                if (!typeTween.isAlive) { break; }
                if (Pointer.current.press.wasPressedThisFrame)
                {
                    typeTween.Complete();
                    break;
                }
                await UniTask.Yield();
            }
        }
    }
}
