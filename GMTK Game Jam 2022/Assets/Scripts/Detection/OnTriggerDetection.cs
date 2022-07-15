using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerDetection : MonoBehaviour
{
    [SerializeField]
    private string _tagDetection;

    [SerializeField]
    public UnityEvent OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_tagDetection))
        {
            OnTriggerEnter?.Invoke();
            gameObject.SetActive(false);
        }

    }
}
