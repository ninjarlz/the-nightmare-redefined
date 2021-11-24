﻿using System.Collections.Generic;
using Building;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GameLogic
{
    public class CapturedRooms : NetworkBehaviour
    {
        private List<Room> rooms = new List<Room>();
        private List<Image> heads = new List<Image>();
        void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                rooms.Add(transform.GetChild(i).GetComponent<Room>());
            }

            //Transform roomCounter = GameObject.Find("RoomCounters").transform;
            //for (int i = 0; i < roomCounter.childCount; i++)
            //{
            //    heads.Add(roomCounter.GetChild(i).GetComponent<Image>());
            //}
        }
    
        void Update()
        {
            bool check = true;
            int roomsToDisable = 0;
        
            foreach (Room room in rooms)
            {
                if (!room.roomCaptured)
                {
                    check = false;
                }
                else
                {
                    roomsToDisable++;
                }
            }

            //CmdTurnHeads(roomsToDisable);

            if (check)
            {
//            GameManager.Instance.CurrentMachState = GameManager.MatchState.Lose;
            }
        }
    }
}
