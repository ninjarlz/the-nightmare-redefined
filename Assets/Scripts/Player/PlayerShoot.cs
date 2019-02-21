﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    
    
    public Camera Cam { get; set; }

    public PlayerEquipment Equipment { get; set; }


    [SerializeField] private LayerMask _mask;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Cam == null) enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shoot();
    }

    void Shoot()
    {
        Equipment.WeaponSound.Play();
        if (isServer) CmdPlayerShooting(transform.name, "-1");
        else CmdPlayerShooting(transform.name, connectionToServer.connectionId.ToString());
        RaycastHit hit;
        Debug.Log(Equipment.Weapon.Range);
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, Equipment.Weapon.Range, _mask))
        {
            Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == "Player")
                CmdPlayerShoot(hit.collider.name, Equipment.Weapon.Damage);
        }
    }


    [Command]
    void CmdPlayerShooting(string shootingPlayerId, string connectionId)
    {
        GameManager.GetPlayer(shootingPlayerId).GetComponent<PlayerEquipment>().RpcPlayerShooting(connectionId);
    }

    

    [Command]
    void CmdPlayerShoot(string shootPlayerId, int damage)
    {
        Debug.Log(shootPlayerId + " has been shoot");
        GameManager.GetPlayer(shootPlayerId).TakeDamage(damage);
    }
}
