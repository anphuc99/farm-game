using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Views
{
    public struct Node
    {
        public int gridX;
        public int gridY;        

        public bool walkable;

        public Vector3 worldPoint;        

        public Node(bool walkable,Vector3 worldPoint, int gridX, int gridY)
        {
            this.gridX = gridX;
            this.gridY = gridY;
            this.walkable = walkable;
            this.worldPoint = worldPoint;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return gridX == ((Node)obj).gridX && gridY == ((Node)obj).gridY;
            }
            return false;
        }
    }

    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance;

        [Header("Thiết Lập Lưới")]
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize; 
        public float nodeUnit;

        private Node[,] _grid;

        private int _gridSizeX;
        private int _gridSizeY;
        private Vector3 _rootPosition;


        private void Awake()
        {
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeUnit);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeUnit);
            _rootPosition = transform.position;
            Instance = this;
        }
        [Button]
        public void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Vector3 worldPoint = GetWorldPoint(x, y);
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeUnit, unwalkableMask));
                    _grid[x, y] = new Node(walkable, worldPoint, x, y);
                }
            }
        }

        [Button]
        public void UpdateGrid()
        {
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    if(_grid[x, y].walkable)
                    {
                        Vector3 worldPoint = GetWorldPoint(x, y);
                        _grid[x, y].walkable = !(Physics2D.OverlapCircle(worldPoint, nodeUnit, unwalkableMask));
                    }
                }
            }
        }

        public List<Vector3> FindPathAsVector3(Vector3 startPos, Vector3 targetPos)
        {
            List<Node> nodes = FindPath(startPos, targetPos);
            List<Vector3> result = new List<Vector3>();
            foreach (Node node in nodes)
            {
                result.Add(node.worldPoint);
            }
            return result;
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = NodeFromWorldPoint(startPos);
            Node targetNode = NodeFromWorldPoint(targetPos);

            if (!startNode.walkable)
            {
                startNode = GetNeighborsWalkable(startNode);
            }

            if(!targetNode.walkable)
            {
                targetNode = GetNeighborsWalkable(targetNode);
            }

            List<Node> path = new List<Node>() {startNode};                    
            while(!startNode.Equals(targetNode))
            {
                List<Node> neighbors = GetNeighbors(startNode);

                Node nodeMin = default;
                float distanceMin = float.MaxValue;

                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (!neighbors[i].walkable) continue;
                    float distance = Vector3.Distance(neighbors[i].worldPoint, targetNode.worldPoint);
                    if (distance < distanceMin)
                    {
                        if (path.Contains(neighbors[i])) continue;
                        nodeMin = neighbors[i];
                        distanceMin = distance;
                    }
                }
                startNode = nodeMin;
                path.Add(startNode);
            }

            return path;
        }

        public Node GetNeighborsWalkable(Node node)
        {
            int index = 1;            
            while (true)
            {
                int top = node.gridY + index;
                int bottom = node.gridY - index;
                int right = node.gridX + index;
                int left = node.gridX - index;

                if (top < _gridSizeY && _grid[node.gridX, top].walkable) return _grid[node.gridX, top];
                if (bottom >= 0 && _grid[node.gridX, bottom].walkable) return _grid[node.gridX, bottom];
                if (right < _gridSizeX && _grid[right, node.gridY].walkable) return _grid[right, node.gridY];
                if (left >= 0 && _grid[left, node.gridY].walkable) return _grid[left, node.gridY];
                index++;

            }
        }

        public Node NodeFromWorldPoint(Vector3 point)
        {
            int x = (int)((point.x - (_rootPosition.x - gridWorldSize.x / 2)) / nodeUnit);
            int y = (int)((point.y - (_rootPosition.y - gridWorldSize.y / 2)) / nodeUnit);

            if(x >= 0 && y >= 0 && x < _gridSizeX && y < _gridSizeY)
            {
                return _grid[x, y];
            }
            return default;

        }

        // Lấy node tương ứng với vị trí thế giới
        private Vector3 GetWorldPoint(int x, int y)
        {
            float pointX = x * nodeUnit + (_rootPosition.x - gridWorldSize.x/2);
            float pointY = y * nodeUnit + (_rootPosition.y - gridWorldSize.y / 2);
            return new Vector3(pointX, pointY);
        }

        

        // Lấy các ô lân cận của một node
        private List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                    {
                        neighbors.Add(_grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        //void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        //    if (_grid != null)
        //    {
        //        for (int x = 0; x < _gridSizeX; x++)
        //        {
        //            for (int y = 0; y < _gridSizeY; y++)
        //            {
        //                Node node = _grid[x, y];
        //                Gizmos.color = (node.walkable) ? Color.white : Color.red;
        //                Gizmos.DrawCube(node.worldPoint, Vector3.one * (nodeUnit/2));
        //            }
        //        }
        //    }
        //}

    }
}

