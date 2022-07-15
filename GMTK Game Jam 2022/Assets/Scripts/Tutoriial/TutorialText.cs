using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialText : MonoBehaviour
{
    [SerializeField]
    private GameObject _targetGameObject;
    [SerializeField]
    private bool _isOneTimeShow = true;
    [SerializeField]
    private bool _isShow;
    [SerializeField]
    private float _showTime;
    
    [Header("Detection")]
    [SerializeField]
    private bool _haveDetectiion;

    [SerializeField] private float _detectionOffset = 2;
    [SerializeField]
    private Transform _playerDetect;

    private int _numberShow;
    private Transform _player;


    #region Events

    [Header("Events")]
    [Space]
    public UnityEvent OnShowUp;
    public UnityEvent OnClose;

    #endregion
    
    public void Close()
    {
        if (!_isShow)
            return;
        _isShow = false;
        OnClose?.Invoke();
        LeanTween.scale(_targetGameObject, Vector3.zero, _showTime).setOnComplete(() => { _targetGameObject.SetActive(false); } );
    }

    public void Show()
    {
        if(_isOneTimeShow)
        {
            if (_numberShow > 0)
                return;
        }
        _isShow = true;
        _numberShow++;
        _targetGameObject.SetActive(true);
        LeanTween.scale(_targetGameObject, Vector3.one, _showTime);
        OnShowUp?.Invoke();
    }
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (_targetGameObject == null)
            _targetGameObject = gameObject;
    }

    private void Update()
    {
        if (!_haveDetectiion)
            return;
        if (Vector2.Distance(_playerDetect.position, _player.position) < _detectionOffset)
            Show();
        if (Vector2.Distance(_playerDetect.position, _player.position) > _detectionOffset)
            Close();
    }
}
