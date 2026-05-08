using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.Events;
using EditorAttributes;

namespace ZooTycoon.AI
{
    public enum OffMeshLinkMoveMode
    {
        Teleport,
        NormalSpeed,
        Parabola,
        Curve
    }

    public class AgentLinkMover : MonoBehaviour
    {
        [Serializable]
        public class LinkMoveConfig
        {
            public OffMeshLinkMoveMode MoveMode;
            public int AreaType;
            public AnimationCurve Curve = new();
            public float ParabolaHeight = 8f;
        }

        [SerializeField] private NavMeshAgent _agent;

        public OffMeshLinkMoveMode DefaultMoveMode = OffMeshLinkMoveMode.NormalSpeed;


        public List<LinkMoveConfig> LinkMoveConfigs = new();

        public UnityEvent<OffMeshLinkMoveMode> OnLinkStart = new();
        public UnityEvent<OffMeshLinkMoveMode> OnLinkEnd = new();

        private IEnumerator Start()
        {
            _agent.autoTraverseOffMeshLink = false;
            while (true)
            {
                if (_agent.isOnOffMeshLink)
                {
                    var offMeshLinkData = _agent.currentOffMeshLinkData;
                    if (Vector3.Distance(offMeshLinkData.endPos, _agent.destination) < Vector3.Distance(offMeshLinkData.startPos, _agent.destination))
                    {
                        var link = (NavMeshLink)_agent.navMeshOwner;
                        var moveConfig = LinkMoveConfigs?.Find(c => c.AreaType == link.area);
                        var moveMode = moveConfig?.MoveMode ?? DefaultMoveMode;

                        OnLinkStart?.Invoke(moveMode);

                        switch (moveMode)
                        {
                            case OffMeshLinkMoveMode.Teleport:
                                break;
                            case OffMeshLinkMoveMode.NormalSpeed:
                                yield return StartCoroutine(MoveAtNormalSpeed());
                                break;
                            case OffMeshLinkMoveMode.Parabola:
                                yield return StartCoroutine(MoveParabola(moveConfig));
                                break;
                            case OffMeshLinkMoveMode.Curve:
                                yield return StartCoroutine(MoveCurve(moveConfig));
                                break;
                        }

                        _agent.CompleteOffMeshLink();
                        OnLinkEnd?.Invoke(moveMode);
                    }

                }

                yield return null;
            }
        }

        private IEnumerator MoveAtNormalSpeed()
        {
            var data = _agent.currentOffMeshLinkData;
            var endPos = data.endPos + Vector3.up * _agent.baseOffset;
            var targetRotation = Quaternion.LookRotation(endPos - _agent.transform.position);
            while (Vector3.Distance(_agent.transform.position, endPos) > 0.1f)
            {
                var pos = Vector3.MoveTowards(_agent.transform.position, endPos, _agent.speed * Time.deltaTime);
                var rotation = Quaternion.RotateTowards(_agent.transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
                _agent.transform.SetPositionAndRotation(pos, rotation);
                yield return null;
            }
        }

        private IEnumerator MoveParabola(LinkMoveConfig config)
        {
            var data = _agent.currentOffMeshLinkData;
            var duration = Vector3.Distance(data.startPos, data.endPos) / _agent.speed;
            var startPos = _agent.transform.position;
            var endPos = data.endPos + Vector3.up * _agent.baseOffset;
            var targetRotation = Quaternion.LookRotation(endPos - _agent.transform.position);
            var normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                var yOffset = config.ParabolaHeight * (normalizedTime - normalizedTime * normalizedTime);
                var pos = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                var rotation = Quaternion.RotateTowards(_agent.transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
                _agent.transform.SetPositionAndRotation(pos, rotation);
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        private IEnumerator MoveCurve(LinkMoveConfig config)
        {
            var data = _agent.currentOffMeshLinkData;
            var duration = Vector3.Distance(data.startPos, data.endPos) / _agent.speed;
            var startPos = _agent.transform.position;
            var endPos = data.endPos + Vector3.up * _agent.baseOffset;
            var targetRotation = Quaternion.LookRotation(endPos - _agent.transform.position);
            var normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                var yOffset = config.Curve.Evaluate(normalizedTime);
                var pos = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                var rotation = Quaternion.RotateTowards(_agent.transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
                _agent.transform.SetPositionAndRotation(pos, rotation);
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

    }
}