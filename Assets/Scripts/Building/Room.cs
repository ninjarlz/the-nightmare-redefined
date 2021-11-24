using System.Collections.Generic;
using GameLogic;
using UnityEngine;

namespace Building
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private int _width;
        public int Width => _width;
        [SerializeField] private int _height;
        public int Height => _height;
        [SerializeField] private int _areas;
        public int Areas => _areas;
        [SerializeField] private int _enemySpawnPointsCounter;
        public int EnemySpawnPointsCounter => _enemySpawnPointsCounter;
        private List<GridPoint> _gridPoints = new List<GridPoint>();
        private List<CaptureArea> _captureAreas = new List<CaptureArea>();
        public List<CaptureArea> CaptureAreas => _captureAreas;
        private readonly List<GameObject> _enemySpawnPoints = new List<GameObject>();
        public List<GameObject> EnemySpawnPoint => _enemySpawnPoints;
        public bool roomCaptured;
        [SerializeField] private bool _hasChest;
        public bool HasChest => _hasChest;


        public enum RoomPhase { Prepare, Defend, Collect};
        public RoomPhase CurrentRoomPhase { get; set; } = RoomPhase.Prepare;


        public void Setup()
        {
            FillGridPointsData();
            FillEnemySpawnPointsData();
            FillCaptureAreasData();
        }

        private void FillGridPointsData()
        {
            Transform points = transform.GetChild(1);
            for (int i = 0; i < points.childCount; i++)
            {
                _gridPoints.Add(points.GetChild(i).GetComponent<GridPoint>());
            }
        }

        private void FillEnemySpawnPointsData()
        {
            Transform areas = transform.GetChild(3);
            for (int i = 0; i < areas.childCount; i++)
            {
                GameManager.Instance.EnemySpawnPoints.Add(areas.GetChild(i).name, areas.GetChild(i));
                _enemySpawnPoints.Add(areas.GetChild(i).gameObject);
            }
            GameManager.GridRenderes.Add(transform.GetChild(0).gameObject);
        }

        private void FillCaptureAreasData()
        {
            Transform captures = transform.GetChild(2);
            for (int i = 0; i < captures.childCount; i++)
            {
                _captureAreas.Add(captures.GetChild(i).GetComponent<CaptureArea>());
            }
        }
    }
}
