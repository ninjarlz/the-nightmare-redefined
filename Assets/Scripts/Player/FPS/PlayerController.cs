﻿using GameLogic;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Player.FPS
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : NetworkBehaviour {
        [SerializeField] public float _speed = 2f;
        [SerializeField] private float _lookSensitivity = 2.5f;
        [SerializeField] private Joystick _move;
        [SerializeField] private Joystick _look;
        public float SensitivityScale { get; set; }
        [SerializeField] private float _nonZoomSensivity = 0.7f;
        Quaternion _prevRot;

        public float NonZoomSensitivity {
            get { return _nonZoomSensivity; }
            set { _nonZoomSensivity = value; }
        }

        [SerializeField] private float _zoomSensivity = 0.1f;

        public float ZoomSensitivity {
            get { return _zoomSensivity; }
            set { _zoomSensivity = value; }
        }

        private float _speedSlow = 2.5f;
        private float _speedFast = 7f;

        private PlayerMotor _motor;
        private PlayerManager _playerManager;
        private static readonly int IsSprinting = Animator.StringToHash("isSprinting");

        void Start() {
            _motor = GetComponent<PlayerMotor>();
            _playerManager = GetComponent<PlayerManager>();
            SensitivityScale = 0.7f;
#if UNITY_ANDROID
        move = GameObject.Find("Move").GetComponent<Joystick>();
        look = GameObject.Find("Look").GetComponent<Joystick>();
#endif
        }

   

        private void Update() {
            if (ButtonsControll.screensOver && !WinLoseScreens.winLoseActive)
            {
                if (transform.GetComponent<PlayerEquipment>().getActiveWeapon().GetComponent<Animator>().GetBool(IsSprinting))
                    _speed = _speedFast;
                else
                    _speed = _speedSlow;


//        Debug.Log(_speed);
                if (PauseGame.menuActive) {
            

                    _motor.Move(Vector3.zero);
                    _motor.Rotate(Vector3.zero);
                    _motor.RotateCamera(0f);
                
                }
                else {
                    float xMov = 0;
                    float zMov = 0;

#if UNITY_ANDROID
            if(Mathf.Abs(move.Horizontal) >= 0.2)
                xMov = move.Horizontal;
            if(Mathf.Abs(move.Vertical) >= 0.2)
                zMov = move.Vertical;
#endif

#if UNITY_STANDALONE
                    xMov = Input.GetAxisRaw("Horizontal");
                    zMov = Input.GetAxisRaw("Vertical");
#endif

                    Vector3 moveHorizontal = transform.right * xMov;
                    Vector3 moveVertical = transform.forward * zMov;

                    Vector3 velocity = (moveHorizontal + moveVertical).normalized * _speed;

                    _motor.Move(velocity);

                    float yRot = 0;
                    float xRot = 0;

#if UNITY_ANDROID
            if(Mathf.Abs(look.Horizontal) >= 0.2)
                yRot = look.Horizontal;
            if(Mathf.Abs(look.Horizontal) >= 0.2)
                xRot = look.Vertical;
#endif

#if UNITY_STANDALONE
                    yRot = Input.GetAxisRaw("Mouse X") * SensitivityScale;
                    xRot = Input.GetAxis("Mouse Y") * SensitivityScale;
#endif

                    Vector3 rotation = new Vector3(0f, yRot, 0f) * _lookSensitivity;
                    _motor.Rotate(rotation);

                    float cameraRotationX = xRot * _lookSensitivity;
                    _motor.RotateCamera(cameraRotationX);


                
        
            

                    _prevRot = transform.rotation;
                }
            }
        
        }

        void OnCollisionEnter(Collision col) {
            if (col.gameObject.CompareTag("SlowDownWall")) {
                _speedFast = 2.5f;
            }
        }    
    
        void OnCollisionExit(Collision col) {
            if (col.gameObject.CompareTag("SlowDownWall")) {
                _speedFast = 6f;
            }
        }
    }
}