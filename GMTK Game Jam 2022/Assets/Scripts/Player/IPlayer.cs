using System;
using Objects;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IPlayer
    {
        Transform Transform { get; }

        public bool IsImmune{ get; }

        void Enable();

        void Disable();

        void Die();
    }
}