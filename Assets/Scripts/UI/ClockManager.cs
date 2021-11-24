﻿using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ClockManager : MonoBehaviour
    {
        public static bool canCount;
        public static float time;
        private TextMeshProUGUI _minutesTM;
        private TextMeshProUGUI _secondsTM;

        private int _minutes;
        private int _seconds;
        private String _zero = "";
    
        void Start()
        {
            _minutesTM = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _secondsTM = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            canCount = false;
            time = 0f;
        }
    
        void Update()
        {
            _zero = "";
            _minutes = (int) time / 60;
            _seconds = (int) time % 60;
        

            if (_minutes < 10)
                _zero = "0";
            _minutesTM.text = _zero + _minutes;
            _zero = "";

            if (_seconds < 10)
                _zero = "0";
            _secondsTM.text = _zero + _seconds;
        
        
            if (canCount)
            {
                time -= Time.deltaTime;
            }
        }
    }
}
