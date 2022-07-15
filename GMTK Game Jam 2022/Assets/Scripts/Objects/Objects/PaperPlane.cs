using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlane : ObjectController
{
    [SerializeField]
    private float _rotateSpeed;
    [SerializeField]
    private float _velocityCheck;

    private void Update()
    {
        if(_rigidbody2D.velocity.magnitude > _velocityCheck)
        {
            float angle = Mathf.Atan2(_rigidbody2D.velocity.y, _rigidbody2D.velocity.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * _rotateSpeed);
        }
    }
}
