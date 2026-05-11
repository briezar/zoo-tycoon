using System;
using Cysharp.Threading.Tasks;
using GameDevKit;
using UnityEngine;
using UnityEngine.AI;
using ZooTycoon.Input;

namespace ZooTycoon.QuestSystem
{
    public class ObtainAnimalStep : MonoBehaviour
    {
        [SerializeField] private QuestDefinitionSO _obtainAnimalQuest;
        [SerializeField] private Animal _animal;
        [SerializeField] private Collider _area;
        [SerializeField] private PlayerController _player;

        private bool _completedEndQuestCinematic = false;

        private void Start()
        {
            StoryDirector.OnStepStarted[this] += (info) => HandleStepStarted(info);
            QuestRegistrySO.Instance.OnQuestCompleted[this] += q => HandleQuestCompleted(q);
        }

        private void OnDestroy()
        {
            StoryDirector.OnStepStarted.RemoveSource(this);
            QuestRegistrySO.Instance.OnQuestCompleted.RemoveSource(this);
        }

        private async UniTask HandleStepStarted(ValueChangeInfo<QuestInstance> info)
        {
            if (info.current.Definition != _obtainAnimalQuest) { return; }
            StoryDirector.OnStepStarted.RemoveSource(this);

            StoryDirector.WaitBeforeQuestCompleteTask += WaitForCinematic;

            InputManager.Enable_PlayerMovement(false);

            _animal.transform.SetParent(null);

            // Place animal at farthest valid NavMesh position within the area before activating
            _animal.transform.position = GetFarthestNavMeshPosition();
            _animal.gameObject.SetActive(true);

            await CinemachineUtils.LookAt(_animal.transform, 1f);

            InputManager.Enable_PlayerMovement(true);
        }

        private async UniTask WaitForCinematic(QuestDefinitionSO questDef)
        {
            if (questDef != _obtainAnimalQuest) { return; }
            await UniTask.WaitUntil(() => _completedEndQuestCinematic);
        }

        private async UniTask HandleQuestCompleted(QuestInstance quest)
        {
            if (quest.Definition != _obtainAnimalQuest) { return; }
            QuestRegistrySO.Instance.OnQuestCompleted.RemoveSource(this);

            await CinemachineUtils.LookAt(_animal.transform, 1f);

            _completedEndQuestCinematic = true;
        }

        private Vector3 GetFarthestNavMeshPosition()
        {
            var playerPos = _player.transform.position;
            var bounds = _area.bounds;

            // Find the farthest point on the collider bounds from the player
            var boundsCenter = bounds.center;
            var directionFromPlayer = (boundsCenter - playerPos).normalized;

            // Extend to the far side of the bounds using ClosestPointOnBounds from the opposite side
            var farSamplePoint = boundsCenter + directionFromPlayer * bounds.extents.magnitude * 2f;
            var farthestOnBounds = _area.ClosestPointOnBounds(farSamplePoint);

            // Offset inward so the animal isn't placed right on the boundary
            const float inwardOffset = 1.5f;
            var inwardDir = (boundsCenter - farthestOnBounds).normalized;
            var candidate = farthestOnBounds + inwardDir * inwardOffset;

            // Snap to NavMesh
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            // Fallback: sample progressively closer toward the center
            for (int i = 1; i <= 5; i++)
            {
                var fallback = Vector3.Lerp(farthestOnBounds, boundsCenter, i * 0.15f);
                if (NavMesh.SamplePosition(fallback, out NavMeshHit fallbackHit, 5f, NavMesh.AllAreas))
                {
                    return fallbackHit.position;
                }
            }

            return candidate;
        }

    }
}