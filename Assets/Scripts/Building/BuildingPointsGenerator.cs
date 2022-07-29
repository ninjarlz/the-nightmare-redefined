using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Building
{
    [ExecuteInEditMode]
    public class BuildingPointsGenerator : MonoBehaviour
    {
        private const string GridCanvasLabel = "GridCanvas";
        private const string RoomsLabel = "Rooms";
        private const string RoomLabel = "Room";
        private const string CrossLabel = "cross";
        private const string CaptureAreaLabel = "CaptureArea";
        private const string ChestLabel = "Chest";
        private const string RoomRenderLabel = "RoomRender";
        private const string PointsLabel = "Points";
        private const string PointLabel = "Point";
        private const string AreasLabel = "Areas";
        private const string AreaLabel = "Area";
        private const string EnemySpawnPointsLabel = "Enemy Spawn Points";
        private const string EnemySpawnPointLabel = "Spawn Point";
        
        private List<GameObject> _rooms = new List<GameObject>();
        private Canvas _gridCanvas;
        private GameObject _gridPointPrefab;
        private GameObject _areaPrefab;
        private GameObject _chestPrefab;

        private void SetUpGridCanvas()
        {
            _gridCanvas = GameObject.Find(GridCanvasLabel).GetComponent<Canvas>();
            var canvasChildren = _gridCanvas.transform
                .Cast<Transform>()
                .Select(t => t.gameObject)
                .ToList();
            canvasChildren.ForEach(DestroyImmediate);
        }

        private void SetUpRoomsObject()
        {
            var roomsGameObject = (GameObject) Resources.Load(RoomsLabel, typeof(GameObject));
            var roomsInstance = Instantiate(roomsGameObject, _gridCanvas.transform);
            roomsInstance.name = RoomsLabel;
            _gridPointPrefab = (GameObject) Resources.Load(CrossLabel, typeof(GameObject));
            _areaPrefab = (GameObject) Resources.Load(CaptureAreaLabel, typeof(GameObject));
            _chestPrefab = (GameObject) Resources.Load(ChestLabel, typeof(GameObject));
            _rooms = roomsInstance.transform
                .Cast<Transform>()
                .Select(t => t.gameObject)
                .ToList();
            var emptyObject = new GameObject();
            for (var roomIndex = 0; roomIndex < _rooms.Count; roomIndex++)
            {
                SetUpRoomObject(roomIndex, emptyObject);
            }
            DestroyImmediate(emptyObject);
        }

        private void SetUpRoomObject(int roomIndex, GameObject emptyObject)
        {
            var room = _rooms[roomIndex].GetComponent<Room>();
            SetUpGridPoints(room, emptyObject);
            SetUpAreas(room, emptyObject);
            SetUpEnemySpawnPoints(room, roomIndex, emptyObject);
        }

        private void SetUpGridPoints(Room room, GameObject emptyObject)
        {
            var roomTransform = room.transform; 
            var halfWidth = Mathf.FloorToInt(room.Width / 2f);
            var halfHeight = Mathf.FloorToInt(room.Height / 2f);
            var roomCanvas = Instantiate(emptyObject, roomTransform.position, roomTransform.rotation, roomTransform);
            if (room.HasChest)
            {
                Instantiate(_chestPrefab, roomTransform.position, roomTransform.rotation, roomTransform);
            }
            roomCanvas.name = RoomRenderLabel;
            var points = Instantiate(emptyObject, roomTransform.position, roomTransform.rotation, roomTransform);
            var pointsTransform = points.transform;
            points.name = PointsLabel;
            for (var x = roomTransform.position.x - halfWidth; x < roomTransform.position.x + halfWidth + 1; x += PlacementController.GridTileSize)
            for (var y = roomTransform.position.z - halfHeight; y < roomTransform.position.z + halfHeight + 1; y += PlacementController.GridTileSize)
            {
                SetUpGridPoint(x, y, room, roomCanvas, pointsTransform, emptyObject);
            }
        }

        private void SetUpGridPoint(float x, float y, Room room, GameObject roomCanvas, Transform pointsTransform, GameObject emptyObject)
        {
            var point = Instantiate(emptyObject, new Vector3(x, pointsTransform.position.y, y), 
                Quaternion.Euler(0f, 0f, 0f), pointsTransform);
            point.tag = room.tag;
            point.name = PointLabel + " " + x + " " + y;
            point.AddComponent<GridPoint>();
            var spriteRenderer = Instantiate(_gridPointPrefab, new Vector3(x, roomCanvas.transform.position.y, y), 
                Quaternion.Euler(90f, 0f, 0f), roomCanvas.transform);
            point.GetComponent<GridPoint>().SpriteRenderer = spriteRenderer.GetComponent<SpriteRenderer>();
        }

        private void SetUpAreas(Room room, GameObject emptyObject)
        {
            var areas = Instantiate(emptyObject, room.transform);
            areas.name = AreasLabel;
            for (var areaIndex = 0; areaIndex < room.Areas; areaIndex++)
            { 
                var area = Instantiate(_areaPrefab, areas.transform);
                area.transform.name = AreaLabel + " " + areaIndex;
                room.CaptureAreas.Add(area.GetComponent<CaptureArea>());
            }
        }

        private void SetUpEnemySpawnPoints(Room room, int roomIndex, GameObject emptyObject)
        {
            var spawnPoints = Instantiate(emptyObject, room.transform).transform;
            spawnPoints.name = EnemySpawnPointsLabel;
            for (int spawnPointIndex = 0; spawnPointIndex < room.EnemySpawnPointsCounter; spawnPointIndex++)
            {
                var spawnPoint = Instantiate(emptyObject, spawnPoints);
                spawnPoint.name = EnemySpawnPointLabel + " " + spawnPointIndex + " " + RoomLabel + roomIndex;
            }
        }

        private void Awake()
        {
            SetUpGridCanvas();
            SetUpRoomsObject();
        }       
  
    }
}
