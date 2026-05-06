using EditorAttributes;
using UnityEngine;

namespace ZooTycoon
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Vector2Int _gridSize = new(10, 10);
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private bool _generateOnStart = true;
        [SerializeField] private Transform _cellParent;

        private void Start()
        {
            if (_generateOnStart)
            {
                GenerateGrid();
            }
        }

        [Button]
        private void GenerateGrid()
        {
            for (int i = _cellParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_cellParent.GetChild(i).gameObject);
            }

            for (int row = 0; row < _gridSize.x; row++)
            {
                for (int col = 0; col < _gridSize.y; col++)
                {
                    var cellPosition = _grid.CellToWorld(new(row, 0, col));
                    var cellObj = Instantiate(_cellPrefab, cellPosition, Quaternion.identity, _cellParent);
                    cellObj.name = $"{_cellPrefab.name} ({row}, {col})";
                }
            }
        }
    }
}