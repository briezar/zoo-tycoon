using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ZooTycoon.UI
{
    public class Loading : MonoBehaviour
    {
        // [SerializeField] private Transform _loadingSpinLeft, _loadingSpinRight;
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }

        // protected void OnEnable()
        // {
        //     _loadingSpinLeft.DOLocalRotate(new Vector3(0, 0, 360), 2, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear).SetId(GetInstanceID());
        //     _loadingSpinRight.DOLocalRotate(new Vector3(0, 0, -360), 2, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear).SetId(GetInstanceID());
        // }

        // private void OnDisable()
        // {
        //     DOTween.Kill(GetInstanceID());
        // }
    }
}