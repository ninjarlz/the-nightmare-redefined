﻿using System.Collections;
using System.Collections.Generic;
using Building;
using Constants;
using Enemy;
using Player.FPS;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        private List<GameObject> _rooms = new List<GameObject>();
        public List<GameObject> Rooms => _rooms;
        private float _prepareTimer = 45f;
        private float[] _timers = { 120f, 180f, 300f };
        private Room _currentRoom;
        [SerializeField] private GameObject[] _chests = new GameObject[3];
        public GameObject currentChest;

        public Room CurrentRoom {get => _currentRoom;
            set
            {
                _currentRoom = value;
                _enemySpawnPoints.Clear();
                _enemySpawnMarkers.Clear();
                _currentCaptureAreas.Clear();
                _currentRoom.EnemySpawnPoint.ForEach(spawnPoint =>
                {
                    _enemySpawnPoints.Add(spawnPoint.transform.name, spawnPoint.transform);
                    if (spawnPoint.transform.childCount <= 0)
                    {
                        return;
                    }
                    CameraFacing cameraFacing = spawnPoint.transform.GetChild(0).GetComponent<CameraFacing>();
                    _enemySpawnMarkers.Add(spawnPoint.transform.GetChild(0).GetComponent<ExPointBlink>());
                    cameraFacing.cameraToLookAt = LocalPlayer.GetComponent<PlayerSetup>().ActionCamera;
                });
                foreach (CaptureArea captureArea in _currentRoom.CaptureAreas)
                {
                    Vector3 v = captureArea.transform.position;
                    _currentCaptureAreas.Add(v.x + "_" + v.y + "_" + v.z, captureArea);
                }
            
            }
        }
        public static bool IsListeningForReady { get; set; }
        private Dictionary<string,CaptureArea> _currentCaptureAreas = new Dictionary<string, CaptureArea>();
        public Dictionary<string, CaptureArea> CurrentCaptureAreas => _currentCaptureAreas;
        private MusicManager _musicManager;
        private Dictionary<string, Transform> _enemySpawnPoints = new Dictionary<string, Transform>();
        private string _nameOfPreviousSpawn;
        public Dictionary<string, Transform> EnemySpawnPoints => _enemySpawnPoints;
        private List<CaptureArea> _captureAreas = new List<CaptureArea>();
        private List<ExPointBlink> _enemySpawnMarkers = new List<ExPointBlink>();
        private static List<GameObject> _gridRenderes = new List<GameObject>();
        [SerializeField] GameObject[] _doors;
        private CapturePointsUI _cpUI;

        public static List<GameObject> GridRenderes { get { return _gridRenderes; } }
        private static List<SpriteRenderer> _spriteRenderes = new List<SpriteRenderer>();
        public static List<SpriteRenderer> SpriteRenderer { get { return _spriteRenderes; } }
        private GameObject _enemyPrefab;
        [SerializeField] private GameObject[] _enemiesPrefabs;
        [SerializeField] private MatchSettings _matchSettings;
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private int _waves;
        [SerializeField] private int _enemiesAmount;
        [SyncVar] private int _enemiesCounter = 0;
        [SyncVar] private int _spawnedEnemiesCounter = 0;
        private Coroutine _spawnEnemy; 
        [SerializeField] private List<CameraFacing> _billboards = new List<CameraFacing>();
        public List<CameraFacing> Billboards { get { return _billboards; } set { _billboards = value; } }
        public enum MatchState { None, Lobby, Room1Prepare, Room1Fight, Room2Prepare, Room2Fight, Room3Prepare, Room3Fight, Win, Lose}
        private MatchState _currentMatchState = MatchState.None;
        private WinLoseScreens _screens;
        private TextMeshProUGUI _pickUp;

   

        public MatchState CurrentMachState
        {
            get { return _currentMatchState; }
            set
            {
                _currentMatchState = value;
                switch (value)
                {
                    case MatchState.Lobby:
                        _arrow.gameObject.SetActive(false);
                        break;

                    case MatchState.Room1Prepare:
                        currentChest = _chests[0];
                        Instance.CurrentRoom = Instance.Rooms[1].GetComponent<Room>();
                    
                        _cpUI.setRoom();
                        _arrow.gameObject.SetActive(true);
                        _arrow.setTarget();
                        ClockManager.time = 60f;
                        ClockManager.canCount = true;
                        StartCoroutine(PickUpPrepareFirst());
                        break;
                    case MatchState.Room1Fight:
                        _enemyPrefab = _enemiesPrefabs[0];
                        _arrow.gameObject.SetActive(false);
                        StartHordeAttack();
                        ClockManager.time = _timers[0];
                        ClockManager.canCount = true;
                        StartCoroutine(PickUpFight());
                        break;
                    case MatchState.Room2Prepare:
                        currentChest = _chests[1];
                        Instance.CurrentRoom = Instance.Rooms[2].GetComponent<Room>();
                        _cpUI.setRoom();

                        _arrow.gameObject.SetActive(true);
                        _arrow.setTarget();

                        ClockManager.time = 60f;
                        ClockManager.canCount = true;
                        _doors[0].SetActive(false);
                        StartCoroutine(PickUpPrepare());
                        break;

                    case MatchState.Room2Fight:
                        _nameOfPreviousSpawn = null;
                        _arrow.gameObject.SetActive(false);
                        _enemyPrefab = _enemiesPrefabs[1];
                        StartHordeAttack();
                        ClockManager.time = _timers[1];
                        ClockManager.canCount = true;
                        StartCoroutine(PickUpFight());
                        break;

                    case MatchState.Room3Prepare:
                        currentChest = _chests[2];
                        Instance.CurrentRoom = Instance.Rooms[0].GetComponent<Room>();
                        _cpUI.setRoom();
                        _arrow.gameObject.SetActive(true);
                        _arrow.setTarget();
                        _doors[1].SetActive(false);
                        ClockManager.time = 90f;
                        ClockManager.canCount = true;
                        StartCoroutine(PickUpPrepare());
                        break;

                    case MatchState.Room3Fight:
                        _nameOfPreviousSpawn = null;
                        _arrow.gameObject.SetActive(false);
                        _enemyPrefab = _enemiesPrefabs[2];
                        StartHordeAttack();
                        ClockManager.time = _timers[2];
                        ClockManager.canCount = true;
                        StartCoroutine(PickUpFight());
                        break;

                    case MatchState.Win:
                        //StopHordeAttack();
                        ClockManager.time = 0f;
                        ClockManager.canCount = false;
                        _screens.ActivateScreen(true);
                        break;

                    case MatchState.Lose:
                        //StopHordeAttack();
                        ClockManager.time = 0f;
                        ClockManager.canCount = false;
                        _screens.ActivateScreen(false);
                        break;
                }
            }
        }
        public enum GameState { Building, Fighting }
        //private static GameState _currentState = GameState.Building;
        private static GameState _currentState = GameState.Fighting;
        public static GameState CurrentState
        {
            get => _currentState;
            set
            {
                if (value == GameState.Building)
                {
                    foreach (PlayerManager player in _players.Values)
                        player.SetBuildingMode();
                    if (Instance.isServer) Instance.StopCoroutine(Instance.SpawnEnemy());
                    TurnOnGridRenders(true);
                }
                else if (value == GameState.Fighting)
                {
                    foreach (PlayerManager player in _players.Values)
                        player.SetActionMode();
                    TurnOnGridRenders(false);
                    //if (Instance.isServer) Instance.StartCoroutine(Instance.SpawnEnemy());
                }
                _currentState = value;
            }
        }
        [SyncVar] public int ReadyPlayersCnt = 0;            
        public GameObject[] Weapons => _weapons;
        [SerializeField] private GameObject[] _floorsToDisable;
        public GameObject[] FloorsToDisable => _floorsToDisable;
        private TextMeshProUGUI _readyPlayers;

        private void SetUpInstance()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one GameManager in scene!");
            }
            else
            {
                Instance = this;
            }
        }

        private void Awake()
        {
            SetUpInstance();
            Transform rooms = GameObject.Find(ObjectsLabels.ROOMS).transform;
            for (int i = 0; i < rooms.childCount; i++)
            {
                _rooms.Add(rooms.GetChild(i).gameObject);
                Transform areas = rooms.GetChild(i).GetChild(2);
            
                for(int k = 0; k < areas.childCount; k++)
                {
                    Transform area = areas.GetChild(k);
                    _spriteRenderes.Add(area.GetComponent<SpriteRenderer>());
                    Transform candles = area.GetChild(0);
                    for (int j = 0; j < candles.childCount; j++)
                    {
                        _billboards.Add(candles.GetChild(j).GetChild(0).GetComponent<CameraFacing>());
                    }
                }
            }
            foreach (GameObject room in _rooms)
            {
                Transform points = room.transform.GetChild(1);
                for (int i = 0; i < points.childCount; i++)
                {
                    Transform point = points.GetChild(i);
                    string posAndTag = point.transform.position.x + "_" + point.transform.position.z + "_" +  point.tag;
                    _buildingPoints.Add(posAndTag, point.GetComponent<GridPoint>());
                }
                Transform captureAreas = room.transform.GetChild(2);
                for (int i = 0; i < captureAreas.childCount; i++)
                {
                    Transform captureArea = captureAreas.GetChild(i);
                    _captureAreas.Add(captureArea.GetComponent<CaptureArea>());
                }
            }
            _cpUI = GameObject.Find(ObjectsLabels.CAPTURE_POINTS).GetComponent<CapturePointsUI>();
            foreach (GameObject room in Rooms)
            {
                room.GetComponent<Room>().Setup();
            }
            _musicManager = GetComponent<MusicManager>();
            _screens = GameObject.Find(ObjectsLabels.WIN_LOOSE_SCREEN).GetComponent<WinLoseScreens>();
            _pickUp = GameObject.Find(ObjectsLabels.INSTRUCTIONS_SCREEN).GetComponent<TextMeshProUGUI>();
            _readyPlayers = GameObject.Find(ObjectsLabels.READY_NUMBER_LABEL).GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            TurnOnGridRenders(false);
        
            //if (Instance.isServer) Instance.StartCoroutine(Instance.SpawnEnemy());
        }

        private void Update()
        {
            if (_currentMatchState == MatchState.Lobby)
            {
                _readyPlayers.enabled = true;
                _readyPlayers.text = ReadyPlayersCnt + "/" + _players.Count + " Players ready";
            }
            else
            {
                _readyPlayers.enabled = false;
            }
            foreach (PlayerManager player in _players.Values)
            {
                if (player._currentHealth > 0f)
                {
                    break;
                }
                CurrentMachState = MatchState.Lose;
            }
        
            if (ClockManager.canCount)
            {
                if (ClockManager.time <= 0)
                {
                    switch (_currentMatchState)
                    {
                        case MatchState.Room1Prepare:
                            _musicManager.ChangeClip(false);
                            CurrentMachState = MatchState.Room1Fight;
                            break;
                        case MatchState.Room1Fight:
                            _musicManager.ChangeClip(true);
                            CurrentMachState = MatchState.Room2Prepare;
                            break;
                        case MatchState.Room2Prepare:
                            _musicManager.ChangeClip(false);
                            CurrentMachState = MatchState.Room2Fight;
                            break;
                        case MatchState.Room2Fight:
                            _musicManager.ChangeClip(true);
                            CurrentMachState = MatchState.Room3Prepare;
                            break;
                        case MatchState.Room3Prepare:
                            _musicManager.ChangeClip(false);
                            CurrentMachState = MatchState.Room3Fight;
                            break;
                        case MatchState.Room3Fight:
                            _musicManager.ChangeClip(false);
                            CurrentMachState = MatchState.Win;
                            break;
                    }
                }
                else if (ClockManager.time <= 10f)
                {
                    switch (_currentMatchState)
                    {
                        case MatchState.Room1Fight:
                            StopHordeAttack();
                            break;

                        case MatchState.Room2Fight:
                            StopHordeAttack();
                            break;

                        case MatchState.Room3Fight:
                            StopHordeAttack();
                            break;
                    }
                }
            }
        
        }
    
        private IEnumerator PickUpPrepareFirst()
        {
            _pickUp.enabled = true;
            _pickUp.text = "Pick up equipment from the chests!";
            yield return new WaitForSeconds(7);
            _pickUp.text = "Fortify the area!";
        }
    
        private IEnumerator PickUpPrepare()
        {
            Debug.Log("Second prepare");
            _pickUp.enabled = true;
            _pickUp.text = "Follow the arrow to the next room!";
            yield return new WaitForSeconds(7);
            _pickUp.text = "Pick up equipment from the chests!";
            yield return new WaitForSeconds(7);
            _pickUp.text = "Fortify the area!";
        }

        private IEnumerator PickUpFight()
        {
            _pickUp.text = "Defend the area!";
            yield return new WaitForSeconds(5);
            _pickUp.enabled = false;
        }

        #region Building
        private Dictionary<string, GridPoint> _buildingPoints = new Dictionary<string, GridPoint>();
        public Dictionary<string, GridPoint> BuildingPoints => _buildingPoints;

        public static void TurnOnGridRenders(bool isOn)
        {
            foreach (GameObject renderer in GridRenderes)
                renderer.SetActive(isOn);
            foreach (SpriteRenderer spriteRenderer in SpriteRenderer)
                spriteRenderer.enabled = isOn;
        }

        [ClientRpc]
        void RpcPauseGame()
        {
        }
        #endregion

        #region EnemySpawning


        public IEnumerator SpawnEnemy()
        {
            float upperTimeBound;
            switch (_activePlayers.Count)
            {
                case 1:
                    upperTimeBound = 8f;
                    break;
                case 2:
                    upperTimeBound = 6f;
                    break;
                case 3:
                    upperTimeBound = 4f;
                    break;
                default:
                    upperTimeBound = 3f;
                    break;
            }

            float randTime = Random.Range(1f, upperTimeBound);
            yield return new WaitForSeconds(randTime);
            Transform spawnPoint = null;
            Transform previousSpawn = null;
            if (_nameOfPreviousSpawn != null)
            {
                previousSpawn = EnemySpawnPoints[_nameOfPreviousSpawn];
                EnemySpawnPoints.Remove(_nameOfPreviousSpawn);
            }
            int randIndex = Random.Range(0, EnemySpawnPoints.Keys.Count);
            int counter = 0;
            foreach (KeyValuePair<string, Transform> entry in _enemySpawnPoints)
            {
                if (counter == randIndex)
                {
                    //Debug.Log(counter);
                    spawnPoint = entry.Value;
                    if (_nameOfPreviousSpawn != null) _enemySpawnPoints.Add(_nameOfPreviousSpawn, previousSpawn);
                    _nameOfPreviousSpawn = spawnPoint.name;
                    break;
                }
                counter++;
            }
            GameObject enemy = Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(enemy);
            _enemiesCounter++;
            _spawnedEnemiesCounter++;
            _spawnEnemy = StartCoroutine(SpawnEnemy());
        }


        public static void SetLayerRecursively(GameObject obj, string layerName)
        {
            if (obj == null)
            {
                return;
            }

            obj.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in obj.transform)
            {
                if (child == null) continue;
                SetLayerRecursively(child.gameObject, layerName);
            }
        }

        public void StartHordeAttack()
        {
            foreach (ExPointBlink exPointBlink in _enemySpawnMarkers)
            {
                exPointBlink.StartBlink();
            }
            if (isServer)
            {
                _spawnEnemy = StartCoroutine(SpawnEnemy());
            }
        }

        public void StopHordeAttack()
        {
            foreach (ExPointBlink exPointBlink in _enemySpawnMarkers)
            {
                exPointBlink.StopBlink();
            }
            if (isServer)
            {
                StopCoroutine(_spawnEnemy);
            }
        }

        #endregion


        #region PlayerAndEnemies

        private const string PLAYER_ID_PREFIX = "Player ";

        private static Arrow _arrow;
        private static PlayerManager _localPlayer;
        public static PlayerManager LocalPlayer {
            get { return _localPlayer; }
            set
            {
                _localPlayer = value;
                _arrow = _localPlayer.GetComponentInChildren<Arrow>();
                if (_arrow == null) Debug.Log("honk");
            }
        }
        private static Dictionary<string, PlayerManager> _players = new Dictionary<string, PlayerManager>();
        private static Dictionary<string, PlayerManager> _activePlayers = new Dictionary<string, PlayerManager>();
        public static Dictionary<string, PlayerManager> Players { get { return _players; } }
        public static Dictionary<string, PlayerManager> ActivePlayers { get { return _activePlayers; } }
        public static Dictionary<string, EnemyControllerServer> _enemies = new Dictionary<string, EnemyControllerServer>();
        public static Dictionary<string, EnemyControllerServer> Enemies { get { return _enemies; } }
        private static int _enemyIdCounter = 0;
        public static int EnemyIdCounter { get { return _enemyIdCounter; } set { _enemyIdCounter = value; } }
    
        private static List<int> arrowIndexList = new List<int> {0, 1, 2, 3};
        public static void RegisterPlayer(string netId, PlayerManager player)
        {
            string playerId = PLAYER_ID_PREFIX + netId;
            _players.Add(playerId, player);
            _activePlayers.Add(playerId, player);
            player.transform.name = playerId;
        
            int tempIndex = Random.Range(0, arrowIndexList.Count);
            player.transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
            if (!player.isLocalPlayer)
            {
                Outline outline = player.GetComponentInChildren<Outline>();
                outline.enabled = true;
                switch (arrowIndexList[tempIndex])
                {
                    case 0:
                        outline.OutlineColor = Color.green;
                        break;
                    case 1:
                        outline.OutlineColor = Color.yellow;
                        break;
                    case 2:
                        outline.OutlineColor = Color.blue;
                        break;
                    case 3:
                        outline.OutlineColor = Color.red;
                        break;
                }
            }
            player.transform.GetChild(4).GetChild(arrowIndexList[tempIndex]).gameObject.SetActive(true);
            arrowIndexList.Remove(tempIndex);
        }

        public static void UnregisterPlayer(string playerId) {
            int index = -1;
            if (_players[playerId].transform.GetChild(4).GetChild(0).gameObject.activeSelf) 
                index = 0;
            else if (_players[playerId].transform.GetChild(4).GetChild(1).gameObject.activeSelf)
                index = 1;
            else if (_players[playerId].transform.GetChild(4).GetChild(1).gameObject.activeSelf)
                index = 2;
            else
                index = 3;
        
            arrowIndexList.Add(index);
            arrowIndexList.Sort();
            
            _players.Remove(playerId);
            if (_activePlayers.ContainsKey(playerId)) _activePlayers.Remove(playerId);
        }

        public static PlayerManager GetPlayer(string playerId)
        {
            if (!_players.ContainsKey(playerId)) return null;
            return _players[playerId];
        }

        public static EnemyControllerServer GetEnemy(string enemyId)
        {
            if (!_enemies.ContainsKey(enemyId)) return null;
            return _enemies[enemyId];
        }

        public static void DeactivatePlayer(string playerId)
        {
            _activePlayers.Remove(playerId);
        }

        public static void ActivatePlayer(string playerId, PlayerManager player)
        {
            _activePlayers.Add(playerId, player);
        }

        public void SetCameraForBillboards(Camera cam)
        {
            foreach (CameraFacing cameraFacing in _billboards)
                cameraFacing.cameraToLookAt = cam;
        }

        public void TeleportToRoom1()
        {
            CurrentMachState = MatchState.Room1Prepare;
            Vector3 pos = CurrentRoom.CaptureAreas[0].transform.position;
            float randX = Random.Range(-2f, 2f);
            float randZ = Random.Range(-2f, 2f);
            Rigidbody rigidbody = LocalPlayer.GetComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.isKinematic = true;
            LocalPlayer.transform.position = new Vector3(pos.x + randX, pos.y + 1f, pos.z + randZ);
            rigidbody.isKinematic = false;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            TurnOnGridRenders(false);
            PlayerEquipment playerEquipment = LocalPlayer.GetComponentInChildren<PlayerEquipment>();
            Destroy(playerEquipment.Weapon1.gameObject);
            if (playerEquipment.Weapon2 != null) Destroy(playerEquipment.Weapon2.gameObject);
            LocalPlayer.gameObject.GetComponent<PlayerSetup>().EquipWeapon();
            _localPlayer.GetComponent<PlacementController>().placeableCount[0] = 0;
            _localPlayer.GetComponent<PlacementController>().placeableCount[1] = 0;
            _localPlayer.GetComponent<PlacementController>().placeableCount[2] = 0;
            _localPlayer.GetComponent<PlayerShoot>()._grenades = 0;
        }


        #endregion
    }
}
