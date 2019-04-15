﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] _toDisable;
    [SerializeField] private Camera _sceneCamera;
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _weaponObjectPrefab;
    private PlayerEquipment _equipment;
    private bool _initialConf = true;
     
    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon();

        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
            DisableWeaponCamera();
        }
        else
        {
            GameManager.LocalPlayer = GetComponent<PlayerManager>();
            _sceneCamera = Camera.main;
            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(false);
            GameManager.Instance.SetCameraForBillboards(_cam);
            
        }
        GetComponent<PlayerManager>().Setup();
    }


    void DisableWeaponCamera()
    {
        _cam.transform.GetChild(1).GetComponent<Camera>().enabled = false;

        GameManager.SetLayerRecursively(_cam.transform.GetChild(0).GetChild(0).gameObject, "LocalPlayer");
    }

    void EquipWeapon()
    {
        GameObject weaponObject = Instantiate(_weaponObjectPrefab, _cam.transform.GetChild(0));
        PlayerShoot shoot = GetComponent<PlayerShoot>();
        shoot.Cam = _cam;
        _equipment = GetComponent<PlayerEquipment>();
        _equipment.Weapon = weaponObject.GetComponent<Weapon>();
        _equipment.WeaponSound = weaponObject.GetComponent<AudioSource>();
        shoot.Equipment = _equipment;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.RegisterPlayer(GetComponent<NetworkIdentity>().netId.ToString(), GetComponent<PlayerManager>());
    }

    private void AssignRemoteLayer()
    {
        transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
    }

    private void DisableComponents()
    {
        for (int i = 0; i < _toDisable.Length; i++)
            _toDisable[i].enabled = false;
    }

    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(true);
        }
        GameManager.UnregisterPlayer(transform.name);
    }

    private void OnEnable()
    {
        if (isLocalPlayer)
        {
            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(false);
        }
    }
}


