using System.Collections.Generic;
using System.Linq;
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
        private List<CaptureArea> _captureAreas = new List<CaptureArea>();
        public List<CaptureArea> CaptureAreas => _captureAreas;
        private readonly List<GameObject> _enemySpawnPoints = new List<GameObject>();
        public List<GameObject> EnemySpawnPoint => _enemySpawnPoints;
        public bool roomCaptured;
        [SerializeField] private bool _hasChest;
        public bool HasChest => _hasChest;
        
        public void Setup()
        {
            FillEnemySpawnPointsData();
            FillCaptureAreasData();
        }
       
        private void FillEnemySpawnPointsData()
        {
            var areas = transform.GetChild(3)
                .Cast<Transform>()
                .ToList();
            areas.ForEach(area =>
            {
                GameManager.Instance.EnemySpawnPoints.Add(area.name, area);
                _enemySpawnPoints.Add(area.gameObject);
            });
            GameManager.GridRenderes.Add(transform.GetChild(0).gameObject);
        }

        private void FillCaptureAreasData()
        {
            _captureAreas = transform.GetChild(2)
                .Cast<Transform>()
                .Select(t => t.GetComponent<CaptureArea>())
                .ToList();
        }
    }
}
