﻿using Building;
using UnityEngine;

namespace Weapons
{
    public class ChestAlwaysFull : MonoBehaviour
    {
        public bool active = true;
        public bool alreadyPicked = false;
        public int grenades;
        public int snares;
        public int teddyBears;
        public int barrels;
        private Room _room;
        private Transform _lidClosed;
        private Transform _lidOpen;
        private GameObject _lid;
        private bool _isSet = false;

        private void Start()
        {
            _lid = transform.GetChild(0).GetChild(2).gameObject;
            _lidClosed = transform.GetChild(3);
            _lidOpen = transform.GetChild(2);
            _lid.transform.localPosition = _lidClosed.transform.localPosition;
            _lid.transform.localRotation = _lidClosed.transform.localRotation;
        }

        private void Update() {
            if (active && !_isSet)
            {
                _lid.transform.position = _lidOpen.transform.position;
                _lid.transform.rotation = _lidOpen.transform.rotation;
                _isSet = true;
            }
            alreadyPicked = false;
        }
    }
}
