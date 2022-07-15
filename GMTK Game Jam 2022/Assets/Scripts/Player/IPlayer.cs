using System;
using Objects;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IPlayer
    {
        Transform Transform { get; }
        void Enable();

        void Disable();

        void Transition(ObjectController gameObject);

        void GetOutFromItem();

        void Die();
    }
}