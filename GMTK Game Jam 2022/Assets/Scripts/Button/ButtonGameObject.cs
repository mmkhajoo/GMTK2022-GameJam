using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonGameObject : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private ButtonType _buttonType;
    [SerializeField]
    private List<GameObject> _gameObjectsDetectable;

    private GameObject _currentObject;
    private Animator _anim;

    #endregion

    #region Events
    [Header("Events")]
    [Space]
    public UnityEvent OnButtonActive;
    public UnityEvent OnButtonDeactive;

    #endregion


    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var gameObject in _gameObjectsDetectable)
        {
            if (gameObject == collision.gameObject)
            {
                _anim.SetBool("active", true);
                _currentObject = collision.gameObject;
                OnButtonActive?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_buttonType == ButtonType.Permanent)
            return;

        if (_currentObject == collision.gameObject)
        {
            _anim.SetBool("active", false);
            _currentObject = null;
            OnButtonDeactive?.Invoke();
        }
    }
}
