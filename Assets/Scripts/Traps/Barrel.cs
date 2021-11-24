﻿using System.Collections;
using GameLogic;
using UnityEngine;
using UnityEngine.Networking;

namespace Traps
{
    public class Barrel : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _explosionPrefab;
        private MeshRenderer _renderer;
        private BoxCollider _collider;
        public float damage;
        public string InitialPosAndTag { get; set; }

        private void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _renderer = GetComponent<MeshRenderer>();
            if (isServer) transform.GetChild(0).GetComponent<BarrelCollider>().server = true;
        }

        public void Explode()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log("EEEEE");
            //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(GameManager.LocalPlayer.GetComponent<NetworkIdentity>().connectionToClient);
            RpcExlodeBarrel(InitialPosAndTag);
            StartCoroutine(Decay());
        }
    
        private IEnumerator Decay()
        {
            yield return new WaitForSeconds(0.2f);
            NetworkServer.Destroy(gameObject);
        
        }

        [Command]
        public void CmdExplodeBarrel()
        {
            Debug.Log("IIIII");
            RpcExlodeBarrel(InitialPosAndTag);
        }

        [ClientRpc]
        public void RpcExlodeBarrel(string posAndTag)
        {
            Debug.Log("AAAAA");
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(explosion, 3f);
       
            GameManager.Instance.BuildingPoints[posAndTag].Buildable = true;
        }

    
    }
}
