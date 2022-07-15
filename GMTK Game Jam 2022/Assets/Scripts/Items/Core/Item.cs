using DefaultNamespace;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Items
{
    public abstract class Item : DragItem , IItem
    {
        #region Fields

        [SerializeField]
        private ItemType _type;
        private bool _isActive;
        private bool _isEnable;
        
        #endregion

        #region Properties

        public ItemType Type => _type;
        public bool IsActive => _isActive;
        public bool IsEnable => _isEnable;

        #endregion

        #region Events
        [Header("Events")]
        [Space]
        public UnityEvent OnActive;
        public UnityEvent OnDeactive;

        #endregion

        #region Public Methods

        public abstract void Execute();

        public virtual void Enable()
        {
            _isEnable = true;
            gameObject.SetActive(true);
            
            OnActive.AddListener(() => _itemSound.OnActivate(_type));
            OnDeactive.AddListener(() => _itemSound.OnDeActivate());
        }

        public virtual void Disable()
        {
            _isEnable = false;
            gameObject.SetActive(false);
        }

        public virtual void Active()
        {
            _isActive = true;
            OnActive?.Invoke();
        }

        public virtual void DeActive()
        {
            _isActive = false;
            OnDeactive?.Invoke();
        }

        #endregion
        
    }
}