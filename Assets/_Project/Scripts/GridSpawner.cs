using EditorAttributes;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZooTycoon
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private Grid _grid;

        public Vector2Int GridSize = new(10, 10);
        public bool CenterGrid = true;

        public GameObject[] CellPrefabs;
        public Vector3 CellScale = Vector3.one;
        public Transform CellParent;

        [EnableField(nameof(UseCustomSeed))]
        public int CellSeed;
        public bool UseCustomSeed = false;

        public bool UseNoise = true;

        [ShowField(nameof(UseNoise))]
        [Tooltip("Higher values result in more variation, lower values result in more uniform distribution")]
        [Min(0.01f)]
        public float NoiseScale = 0.1f;

        [ShowField(nameof(UseNoise))]
        [Tooltip("Only cells with noise value above this threshold will be spawned. Higher values result in fewer cells, lower values result in more cells.")]
        [Range(0, 1)]
        public float NoiseThreshold = 0.6f;

        public Grid Grid => _grid;

        private readonly Dictionary<Vector3Int, GameObject> _spawnedCells = new();

        private static readonly List<GridSpawner> _instances = new();

        private void OnEnable() => _instances.Add(this);
        private void OnDisable() => _instances.Remove(this);

        // private void Start()
        // {
        //     _spawnedCells.Clear();
        //     for (int i = 0; i < CellParent.childCount; i++)
        //     {
        //         var child = CellParent.GetChild(i).gameObject;
        //         var cellPos = _grid.WorldToCell(child.transform.position);
        //         _spawnedCells[cellPos] = child;
        //     }
        // }

        // public static IEnumerable<GameObject> EnumerateCellsAtWorldPos(Vector3 worldPos)
        // {
        //     foreach (var instance in _instances)
        //     {
        //         var cellPos = instance.Grid.WorldToCell(worldPos);
        //         if (instance._spawnedCells.TryGetValue(cellPos, out var cellObj))
        //         {
        //             yield return cellObj;
        //         }
        //     }
        // }

        [Button]
        public void GenerateCells()
        {
            for (int i = CellParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(CellParent.GetChild(i).gameObject);
            }

            if (!UseCustomSeed)
            {
                CellSeed = (int)(GridSize.x * 73856093 ^ GridSize.y * 19349663 ^ (UseNoise ? 83492791 : 0) ^ (CenterGrid ? 2971215073 : 0));
            }

            _spawnedCells.Clear();

            var random = new System.Random(CellSeed);
            var gridWorldSize = new Vector3(GridSize.x * _grid.cellSize.x, 0, GridSize.y * _grid.cellSize.z);

            for (int row = 0; row < GridSize.x; row++)
            {
                for (int col = 0; col < GridSize.y; col++)
                {
                    if (UseNoise)
                    {
                        var noise = Mathf.PerlinNoise(row * NoiseScale, col * NoiseScale);
                        if (noise < NoiseThreshold) { continue; }
                    }

                    var cellGridPos = new Vector3Int(row, 0, col);
                    var cellWorldPos = _grid.CellToWorld(cellGridPos);
                    if (CenterGrid)
                    {
                        cellWorldPos -= gridWorldSize / 2f;
                        cellWorldPos += new Vector3(_grid.cellSize.x, 0, _grid.cellSize.z) / 2f;
                    }
                    var cellPrefab = CellPrefabs[random.Next(CellPrefabs.Length)];
                    var cellObj =
// #if UNITY_EDITOR
//                     (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, CellParent); // Linking prefabs should reduce scene file size but for some reason, it increases dramatically instead. Gonna use regular Instantiate for now.
// #else
                    Instantiate(cellPrefab, CellParent);
// #endif
                    cellObj.transform.position = cellWorldPos;
                    cellObj.name = $"{cellPrefab.name} ({row}, {col})";
                    cellObj.transform.localScale = Vector3.Scale(cellObj.transform.localScale, CellScale);

                    _spawnedCells.Add(cellGridPos, cellObj);
                }
            }

        }
    }
}