using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIcon : MonoBehaviour
{
    private float _playerIconRotation = 0f;
    private Quaternion _rotation;

    private void Start()
    {
        _rotation = transform.rotation;
    }

    private void Update()
    {
        if (GameManager.LocalPlayer != null)
        {
            _playerIconRotation = GameManager.LocalPlayer.transform.rotation.eulerAngles.y;
        }
        transform.rotation = Quaternion.Euler(90, _rotation.y, -_playerIconRotation);
    }
}
