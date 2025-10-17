using System.Collections.Generic;
using UnityEngine;
using TTGJ.Common;
using TTGJ.Framework;

namespace TTGJ.Plant
{
    public class FieldSystem : MonoSingleton<FieldSystem>
    {
        private Field[][] fields;
        private Dictionary<PlantBase, Vector2Int> plantDic = new Dictionary<PlantBase, Vector2Int>();

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (transform.childCount < 1)
            {
                Debug.Log("Not had field");
                return;
            }
            fields = new Field[transform.childCount][];
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform row = transform.GetChild(i);
                fields[i] = new Field[row.childCount];
                for (int j = 0; j < row.childCount; ++j)
                {
                    fields[i][j] = row.GetChild(j).gameObject.TryAddComponent<Field>();
                    Cell cell = fields[i][j].gameObject.TryAddComponent<Cell>();
                    cell.cellPos = new Vector2Int(i, j);
                }
            }
        }

        private bool CheckCanPlant(Vector2Int cellPos, int currentSize, int preSize)
        {
            if (cellPos.x - preSize < 0 || cellPos.x + currentSize > fields.Length
            || cellPos.y - preSize < 0 || cellPos.y + currentSize > fields[0].Length)
            {
                return false;
            }

            int leftColumn = cellPos.x - preSize;
            int rightColumn = cellPos.x + currentSize;
            int topRow = cellPos.y - preSize;
            int bottomRow = cellPos.y + currentSize;

            for (int i = topRow; i < bottomRow; ++i)
            {
                if (fields[leftColumn][i].IsPlanted())
                {
                    return false;
                }
            }

            for (int i = topRow; i < bottomRow; ++i)
            {
                if (fields[rightColumn][i].IsPlanted())
                {
                    return false;
                }
            }

            for (int i = leftColumn; i < rightColumn; ++i)
            {
                if (fields[i][topRow].IsPlanted())
                {
                    return false;
                }
            }

            for (int i = leftColumn; i < rightColumn; ++i)
            {
                if (fields[i][bottomRow].IsPlanted())
                {
                    return false;
                }
            }
            return true;
        }
        public bool ToOccupied(Vector2Int cellPos, int currentSize, int preSize)
        {
            if (!CheckCanPlant(cellPos, currentSize, preSize))
            {
                return false;
            }
            int leftColumn = cellPos.x - preSize;
            int rightColumn = cellPos.x + currentSize;
            int topRow = cellPos.y - preSize;
            int bottomRow = cellPos.y + currentSize;

            for (int i = topRow; i < bottomRow; ++i)
            {
                fields[leftColumn][i].ToOccupied();
            }

            for (int i = topRow; i < bottomRow; ++i)
            {
                fields[rightColumn][i].ToOccupied();
            }


            for (int i = leftColumn; i < rightColumn; ++i)
            {
                fields[i][topRow].ToOccupied();
            }

            for (int i = leftColumn; i < rightColumn; ++i)
            {
                fields[i][bottomRow].ToOccupied();
            }
            return true;
        }
        public List<PlantBase> GetSurroundPlants(Vector2Int cellPos, int currentSize)
        {
            List<PlantBase> plants = new List<PlantBase>();

            int halfSize = currentSize / 2;
            int leftBound = cellPos.x - halfSize - 1;
            int rightBound = cellPos.x + halfSize + 1;
            int topBound = cellPos.y - halfSize - 1;
            int bottomBound = cellPos.y + halfSize + 1;

            for (int x = leftBound; x <= rightBound; x++)
            {
                for (int y = topBound; y <= bottomBound; y++)
                {
                    bool isInnerArea = (x > leftBound && x < rightBound && y > topBound && y < bottomBound);
                    if (isInnerArea)
                        continue;

                    if (x < 0 || x >= fields.Length || y < 0 || y >= fields[0].Length)
                        continue;

                    PlantBase plant = fields[x][y].GetOccupiedPlant();
                    if (plant != null && !plants.Contains(plant))
                    {
                        plants.Add(plant);
                    }
                }
            }

            return plants;
        }
        public void AddPlant(PlantBase plant, Vector2Int cellPos)
        {
            plantDic.Add(plant, cellPos);
        }
        public void RemovePlant(PlantBase plant)
        {
            plantDic.Remove(plant);
        }
    }    
}