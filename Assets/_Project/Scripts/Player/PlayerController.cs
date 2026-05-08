using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using EditorAttributes;
using GameDevKit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using ZooTycoon.AI;
using ZooTycoon.Input;

namespace ZooTycoon
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovement Movement { get; private set; }

    }
}