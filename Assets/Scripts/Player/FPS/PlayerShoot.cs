﻿using System;
using System.Collections;
using GameLogic;
using TMPro;
using Traps;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using Weapons;

namespace Player.FPS
{
    public class PlayerShoot : NetworkBehaviour {
        public Camera Cam { get; set; }

        public PlayerEquipment Equipment { get; set; }

        private PlayerController _playerController;
        public AudioSource source { get; set; }
        public AudioClip shot;
        public AudioClip hitMarker;
        public AudioClip reload;
        public AudioClip pistol;
        public AudioClip rifle;
        public AudioClip nono;
  

        public GameObject Cross;

        [SerializeField] private LayerMask _mask;
        private bool _shootingDone = false;
        private float crossAccuracy = 1;
        private float normalFOV;
        private float zoomFOV;
        [SerializeField] private GameObject _grenadePrefab;
        private float _grenadeTimer = 0f;
        public int _maxGrenades = 3;
        public int _grenades;

        public TextMeshProUGUI _grenadesTM;


        public bool IsBuildingOnFly { get; set; }
        public bool WasBuilt { get; set; }
        private Animator weaponAnimator;
        private static readonly int IsAiming = Animator.StringToHash("isAiming");
        private static readonly int IsReloading = Animator.StringToHash("isReloading");
        private static readonly int IsSprinting = Animator.StringToHash("isSprinting");
        private static readonly int IsHidden = Animator.StringToHash("isHidden");
        private float currentRecoil;
        public float changeWeaponCooldown = 0;
        private Weapon activeWeapon = null;
        private Animator _playerAnimator;
    


        // Start is called before the first frame update
        void Start() {
            _grenades = 0;
            source = GetComponent<AudioSource>();
            activeWeapon = Equipment.Weapon1;
            if (Cam == null) enabled = false;
            else {
                Cross = GameObject.Find("cross");
                IsBuildingOnFly = false;
            }

            if(transform.GetChild(0).GetChild(0).gameObject.activeSelf)
                _playerAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
            else
                _playerAnimator = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();

            if (isLocalPlayer)
                _grenadesTM = GameObject.Find("Grenades").GetComponent<TextMeshProUGUI>();
            normalFOV = Cam.fieldOfView;
            zoomFOV = normalFOV - 40;
            currentRecoil = activeWeapon.Recoil;

            if (!isLocalPlayer) activeWeapon.GetComponent<Animator>().enabled = false;
        }


        void Update() {
            if (ButtonsControll.screensOver && !WinLoseScreens.winLoseActive)
            { 
                currentRecoil = Equipment.getActiveWeapon().Recoil;
//        weaponAnimator = activeWeapon.GetComponent<Animator>();

                if (_grenades > _maxGrenades)
                    _grenades = _maxGrenades;
                if (isLocalPlayer)
                    _grenadesTM.text = _grenades.ToString();
                if (crossAccuracy > 1.02) crossAccuracy -= (crossAccuracy * 0.05f + 0.02f);
                else crossAccuracy = 1f;
                Cross.transform.GetChild(0).transform.localScale = new Vector3(crossAccuracy, crossAccuracy, crossAccuracy);

                //changing weapon
                if (changeWeaponCooldown > 0) changeWeaponCooldown -= Time.deltaTime;
                if (changeWeaponCooldown <= 0 && Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01 &&
                    Equipment.getActiveWeapon().State == Weapon.WeaponState.idle && !IsBuildingOnFly) {
                    changeWeaponCooldown = 2;
                    if (Equipment.Weapon2 != null) {
                        if (Equipment.Weapon1.gameObject.activeSelf) {
                            StartCoroutine(HideWeapon(Equipment.Weapon1.gameObject, Equipment.Weapon2.gameObject));    //show rifle
                        }
                        else {
                            StartCoroutine(HideWeapon(Equipment.Weapon2.gameObject, Equipment.Weapon1.gameObject));    //show pistol
                        }
                    }
                }


                //fire mode
                if (Input.GetKeyDown(KeyCode.B)) Equipment.getActiveWeapon().changeFireMode();

                //reloading
                if (Input.GetKeyDown(KeyCode.R) && Equipment.getActiveWeapon().State != Weapon.WeaponState.reloading &&
                    Equipment.getActiveWeapon().CurrentMagAmmo != Equipment.getActiveWeapon().MaxMagAmmo) {
                    StartCoroutine(Reload());
                }

                if (Input.GetKey(KeyCode.G)) {
                    if (_grenadeTimer <= 3f) {
                        _grenadeTimer += Time.deltaTime;
                    }
                }
                if (Input.GetKeyUp(KeyCode.G)) {
                    if (_grenades > 0)
                    {
                        _grenades--;
                        CmdSpawnGrenade(transform.position, transform.rotation, transform.forward,
                            (_grenadeTimer + 0.5f) / 3);
                        _playerAnimator.SetTrigger("throwing");
                        _grenadeTimer = 0f;
                    }
                    else
                    {

                        source.clip = nono;
                        source.PlayOneShot(source.clip);
                        Debug.Log("nono");
                    }
                }
            

                //fireing
                if (Input.GetButton("Fire1") && Equipment.getActiveWeapon().State == Weapon.WeaponState.idle &&
                    !PauseGame.menuActive && Equipment.getActiveWeapon().CurrentMagAmmo >= 1 && !IsBuildingOnFly) {
                    Shoot();
                }

                if (Input.GetButtonUp("Fire1")) {
                    _shootingDone = false;
                    if (WasBuilt) {
                        IsBuildingOnFly = false;
                        WasBuilt = false;
                    }
                }

                //aiming
                if (Input.GetButton("Fire2") &&
                    (Equipment.getActiveWeapon().State == Weapon.WeaponState.idle ||
                     Equipment.getActiveWeapon().State == Weapon.WeaponState.shooting) &&
                    !PauseGame.menuActive) {
                    currentRecoil = Equipment.getActiveWeapon().Recoil * 0.35f;
                    if (isLocalPlayer) {
                        Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsSprinting, false);
                        Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsAiming, true);
                    }

                    Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, zoomFOV, 0.6f);
                    Cross.gameObject.SetActive(false);
                    _playerController.SensitivityScale = _playerController.ZoomSensitivity;
                }
                else {
                    currentRecoil = Equipment.getActiveWeapon().Recoil;
                    if (isLocalPlayer) Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsAiming, false);
                    Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, normalFOV, 0.6f);
                    Cross.gameObject.SetActive(true);
                    _playerController.SensitivityScale = _playerController.NonZoomSensitivity;
                }
            }
        }

        public IEnumerator HideWeapon(GameObject toHide, GameObject toShow) {
            toHide.gameObject.SetActive(true);
            toShow.gameObject.SetActive(true);
        
            toHide.transform.GetChild(0).gameObject.SetActive(true);
            toShow.transform.GetChild(0).gameObject.SetActive(false);

            toHide.transform.GetComponent<Animator>().SetBool(IsHidden, true);
            toShow.transform.GetComponent<Animator>().SetBool(IsHidden, true);
            yield return new WaitForSeconds(0.5f);    
            toHide.transform.GetChild(0).gameObject.SetActive(false);
            toShow.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log(toShow.transform.GetComponent<Weapon>().Id);
            if(toShow.transform.GetComponent<Weapon>().Id == 1)
                playSound(rifle);
            else
                playSound(pistol);
            yield return new WaitForSeconds(0.5f); 
            if (toShow.transform.GetComponent<Weapon>().Id == 0) {
                toShow.transform.localPosition = new Vector3(0.02f, 0.03f, -0.22f);
                toShow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else {
                toShow.transform.localPosition = new Vector3(0.08f, -0.02f, -0.17f);
                toShow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        
            toShow.transform.GetComponent<Animator>().SetBool(IsHidden, false);
            toHide.transform.GetComponent<Animator>().SetBool(IsHidden, false);
            toHide.gameObject.SetActive(false);
            toShow.gameObject.SetActive(true);
        }
    
        IEnumerator Reload() {
            if (isLocalPlayer) Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsReloading, true);
            //Blackened = true;
            Equipment.getActiveWeapon().reload();
            playSound(reload);
            yield return new WaitForSeconds(Equipment.getActiveWeapon().ReloadTime);
            //Blackened = false;
            if (isLocalPlayer) Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsReloading, false);
        }

        IEnumerator TripleShot() {
            PerformWeaponFire();
            yield return new WaitForSeconds(Equipment.getActiveWeapon().FireRate * 0.8f);
            PerformWeaponFire();
            yield return new WaitForSeconds(Equipment.getActiveWeapon().FireRate * 0.8f);
            PerformWeaponFire();
            yield return new WaitForSeconds(Equipment.getActiveWeapon().FireRate * 0.8f);
        }


        void Shoot() {
            crossAccuracy += 2f - crossAccuracy * 0.5f;
            Equipment.getActiveWeapon().GetComponent<Animator>().SetBool(IsSprinting, false);


            if (Equipment.getActiveWeapon().Mode == Weapon.FireMode.single && !_shootingDone) {
                PerformWeaponFire();
                _shootingDone = true;
            }
            else if (Equipment.getActiveWeapon().Mode == Weapon.FireMode.triple && !_shootingDone) {
                Equipment.getActiveWeapon().Recoil = Equipment.getActiveWeapon().Recoil / 2;
                StartCoroutine(TripleShot());
                Equipment.getActiveWeapon().Recoil = Equipment.getActiveWeapon().Recoil * 2;
                _shootingDone = true;
            }
            else if (Equipment.getActiveWeapon().Mode == Weapon.FireMode.continous) {
                PerformWeaponFire();
            }
        }


        void PerformWeaponFire() {
            if (Equipment.getActiveWeapon().CurrentMagAmmo >= 1) {
                Equipment.PlayerShooting();
                playSound(shot);
                Equipment.getActiveWeapon().shoot();
                gameObject.GetComponent<PlayerMotor>().IncreaseRecoil(currentRecoil);
                CmdPlayerShooting();
                RaycastHit hit;
                if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit,
                    Equipment.getActiveWeapon().Range,
                    _mask, QueryTriggerInteraction.Ignore)) {
                    Debug.Log("We hit " + hit.collider.name);
                    if (hit.collider.tag == "Player") { //wylaczamy friendly fire??? NIE XD
                        StartCoroutine(ShowHitmarker());
                        CmdPlayerShoot(hit.collider.GetComponentInParent<PlayerManager>().transform.name,
                            Equipment.getActiveWeapon().Damage);
                    }
                    else if (hit.collider.tag == "EnemyHead") {
                        StartCoroutine(ShowHitmarker());
                        CmdEnemyShoot(hit.collider.GetComponentInParent<NavMeshAgent>().transform.name,
                            3 * Equipment.getActiveWeapon().Damage);
                    }
                    else if (hit.collider.tag == "EnemyBody") {
                        StartCoroutine(ShowHitmarker());
                        CmdEnemyShoot(hit.collider.GetComponentInParent<NavMeshAgent>().transform.name,
                            2 * Equipment.getActiveWeapon().Damage);
                    }
                    else if (hit.collider.tag == "EnemyLegs") {
                        StartCoroutine(ShowHitmarker());
                        CmdEnemyShoot(hit.collider.GetComponentInParent<NavMeshAgent>().transform.name,
                            Equipment.getActiveWeapon().Damage);
                    }
                    else if (hit.collider.tag == "Barrel") {
                        Barrel barrel = hit.collider.GetComponent<Barrel>();
                        //CmdSetAuthAndExplode(barrel.netId, GetComponent<NetworkIdentity>());
                        CmdExplodeBarrel(barrel.netId);
                        //barrel.Explode();
                        //barrel.CmdExplodeBarrel(barrel.InitialPosAndTag);

                    }


                    Equipment.DoHitEffect(hit.point, hit.normal);
                    CmdOnHit(hit.point, hit.normal);
                }

                if (Equipment.getActiveWeapon().CurrentMagAmmo == 0 && Equipment.getActiveWeapon().CurrentAmmo >= 1)
                    StartCoroutine(Reload());
            }
        }

        [Command]
        public void CmdExplodeBarrel(NetworkInstanceId objectId)
        {
            var Barrel = NetworkServer.FindLocalObject(objectId).GetComponent<Barrel>();
            Barrel.Explode();
        }


        [Command]
        public void CmdSetAuthAndExplode(NetworkInstanceId objectId, NetworkIdentity player)
        {
            var iObject = NetworkServer.FindLocalObject(objectId);
            Debug.Log(iObject.name);
            var networkIdentity = iObject.GetComponent<NetworkIdentity>();
            var otherOwner = networkIdentity.clientAuthorityOwner;
            Debug.Log(otherOwner.connectionId);
            if (otherOwner == player.connectionToClient)
            {
                Debug.Log("ZAJEBSICIE");
                return;
            }
            else
            {
                if (otherOwner != null)
                {
                    networkIdentity.RemoveClientAuthority(otherOwner);
                }
                networkIdentity.AssignClientAuthority(player.connectionToClient);
                Debug.Log("DOWOD ZODNOSCI: ");
                Debug.Log(networkIdentity.clientAuthorityOwner.ToString());

                Debug.Log(player.connectionToClient.ToString());
            }
        }

        private void playSound(AudioClip clip)
        {
//        if (clip == hitMarker)
//            source.volume = 1.0f;
//        else
//            source.volume = 0.7f;
            source.clip = clip;
            source.PlayOneShot(source.clip);
        }

        public void playSootSound()
        {
            playSound(shot);
        }

        IEnumerator ShowHitmarker() {
            playSound(hitMarker);
            Cross.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            Cross.transform.GetChild(1).gameObject.SetActive(false);
        }

        [Command]
        void CmdPlayerShooting() {
            Equipment.RpcPlayerShooting();
        }

        [Command]
        void CmdOnHit(Vector3 hitPoint, Vector3 normal) {
            Equipment.RpcDoHitEffect(hitPoint, normal);
        }

        [Command]
        void CmdEnemyShoot(string shootEnemyId, float damage) {
            Debug.Log(shootEnemyId + " has been shoot");
            GameManager.GetEnemy(shootEnemyId).CmdTakeDamage(damage);
        }


        public void InvokeCmdPlayerShoot(string shootPlayerId, float damage) {
            CmdPlayerShoot(shootPlayerId, damage);
        }

        [Command]
        void CmdPlayerShoot(string shootPlayerId, float damage) {
            Debug.Log(shootPlayerId + " has been shoot");
            GameManager.GetPlayer(shootPlayerId).RpcTakeDamage(damage);
        }

        [Command]
        void CmdSpawnGrenade(Vector3 playerPos, Quaternion playerRot, Vector3 forwardVector, float force) {
            GameObject grenade = Instantiate(_grenadePrefab, playerPos + forwardVector + Vector3.up / 2, playerRot);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.velocity = force * (15 * forwardVector + 5 * Vector3.up);
            NetworkServer.Spawn(grenade);
        }
    }
}