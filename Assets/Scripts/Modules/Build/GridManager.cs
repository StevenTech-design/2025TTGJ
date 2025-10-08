using System;
using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Build
{
    public class GridManager : MonoBehaviour
    {

		private HashSet<GameObject> _cells = new HashSet<GameObject>();
		private Dictionary<Vector2,bool> _cellOccupied = new Dictionary<Vector2,bool>();
			
        private bool CheckCellOccupied(Vector2Int pos, Vector2Int size) { 
			for(int i = pos.x; i < pos.x + size.x; i++) { 
				for(int j = pos.y; j < pos.y + size.y; j++) { 
					if(_cellOccupied.ContainsKey(new Vector2(i,j))) { 
						return true;
					}
				}
			}
			return false;
		}
		private void SetCellOccupied(Vector2Int pos, Vector2Int size) { 
			for(int i = pos.x; i < pos.x + size.x; i++) { 
				for(int j = pos.y; j < pos.y + size.y; j++) { 
					_cellOccupied[new Vector2(i,j)] = true;
				}
			}
		}
		private void ClearCellOccupied(Vector2Int pos, Vector2Int size) { 
			for(int i = pos.x; i < pos.x + size.x; i++) { 
				for(int j = pos.y; j < pos.y + size.y; j++) { 
					_cellOccupied[new Vector2(i,j)] = false;
				}
			}
		}
    }
}


