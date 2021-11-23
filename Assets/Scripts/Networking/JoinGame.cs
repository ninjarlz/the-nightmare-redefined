﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;

public class JoinGame : MonoBehaviour
{
   [SerializeField] private GameObject _roomListInst;
   [FormerlySerializedAs("status")] [SerializeField] private TextMeshProUGUI _status;
   [SerializeField] private Transform _roomListParent;
   [SerializeField] private GameObject _panel;
   private List<GameObject> _roomList = new List<GameObject>();
   private NetworkManager _networkManager;

   void Start()
   {
      _networkManager = NetworkManager.singleton;
      if (_networkManager.matchMaker == null)
      {
         _networkManager.StartMatchMaker();
      }

      RefreshRoomList();
   }

   public void RefreshRoomList()
   {
      ClearRoomList();
      _networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
      _status.text = "Loading...";
   }

   public void OnMatchList(bool success, string extendedinfo, List<MatchInfoSnapshot> responsedata)
   {
      _status.text = "";

      if (responsedata == null)
      {
         _status.text = "Couldn't get room list.";
         return;
      }

      foreach (MatchInfoSnapshot response in responsedata)
      {
         GameObject roomListItemGO = Instantiate(_roomListInst, _roomListParent, false);
         roomListItemGO.transform.SetParent(_roomListParent);
         
//         
//         roomListItemGO.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
//         roomListItemGO.transform.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
//         roomListItemGO.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
         ServerListItem item = roomListItemGO.GetComponent<ServerListItem>();
         if (item != null)
         {
            item.Setup(response, JoinRoom);
         }
         
         
         _roomList.Add(roomListItemGO);
      }

      if (_roomList.Count == 0)
      {
         _status.text = "No servers found.";
      }
   }

   void ClearRoomList()
   {
      for (int i = 0; i < _roomList.Count; i++)
      {
         Destroy(_roomList[i]);
      }
      
      _roomList.Clear();
   }

   public void JoinRoom(MatchInfoSnapshot match)
   {
      _networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, _networkManager.OnMatchJoined);
      ClearRoomList();
      _panel.SetActive(true);
      _panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joining room...";
   }
}
