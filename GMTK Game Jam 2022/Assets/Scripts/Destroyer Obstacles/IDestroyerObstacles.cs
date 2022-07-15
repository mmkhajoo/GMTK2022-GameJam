using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyerObstacles 
{
    bool IsActive { get; }

    void Active();

    void Deactive();

    void Execute();
}
