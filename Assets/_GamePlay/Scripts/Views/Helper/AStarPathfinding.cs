using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        public int gCost = int.MaxValue; // Khởi tạo chi phí lớn
        public int hCost;
        public int fCost { get { return gCost + hCost; } }
        public Node parent;

        public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPosition;
            gridX = _gridX;
            gridY = _gridY;
        }

        public override bool Equals(object obj)
        {
            Node other = obj as Node;
            if (other == null) return false;
            return gridX == other.gridX && gridY == other.gridY;
        }

        public override int GetHashCode()
        {
            return gridX * 397 ^ gridY;
        }
    }

    public class AStarPathfinding : MonoBehaviour
    {
        public static AStarPathfinding Instance;

        [Header("Thiết Lập Lưới")]
        public LayerMask unwalkableMask;  // Mask xác định các ô không đi được (vật cản)
        public Vector2 gridWorldSize;     // Kích thước vùng lưới trong thế giới
        public float nodeRadius;          // Bán kính của mỗi node

        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        private void Awake()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            Instance = this;
        }
        [Button]

        // Tạo lưới các node dựa trên kích thước vùng lưới
        public void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    // Kiểm tra xem vị trí này có bị che bởi vật cản không (sử dụng Physics2D cho game 2D)
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                    grid[x, y] = new Node(walkable, worldPoint, x, y);
                }
            }
        }

        [Button]
        public void UpdateGrid()
        {
            // Tính vị trí góc dưới bên trái của vùng lưới
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

            // Duyệt qua từng node trong lưới
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if(grid[x, y].walkable)
                    {
                        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                        // Kiểm tra xem vị trí này có bị che bởi vật cản không (sử dụng Physics2D cho game 2D)
                        bool walkable = !(Physics2D.Raycast(worldPoint, worldPoint, unwalkableMask));
                        grid[x, y] = new Node(walkable, worldPoint, x, y);
                    }
                }
            }
        }

        public List<Vector3> FindPathAsVector3(Vector3 startPos, Vector3 targetPos)
        {
            // Gọi hàm FindPath để lấy đường đi dưới dạng danh sách Node
            List<Node> nodePath = FindPath(startPos, targetPos);
            List<Vector3> vectorPath = new List<Vector3>();

            // Nếu không tìm được đường đi, trả về danh sách rỗng
            if (nodePath == null)
                return vectorPath;

            // Chuyển đổi từng node thành vị trí Vector3
            foreach (Node node in nodePath)
            {
                vectorPath.Add(node.worldPosition);
            }
            return vectorPath;
        }


        // Tìm đường đi từ vị trí bắt đầu đến đích
        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = NodeFromWorldPoint(startPos);
            startNode.gCost = 0;
            Node targetNode = NodeFromWorldPoint(targetPos);

            // Nếu target không walkable, tìm node walkable gần nhất
            if (!targetNode.walkable)
            {
                Node nearestWalkable = FindNearestWalkableNode(targetNode);
                if (nearestWalkable != null)
                {
                    targetNode = nearestWalkable;
                }
                else
                {
                    // Nếu không có node nào walkable, trả về null hoặc danh sách rỗng
                    return null;
                }
            }

            if (!startNode.walkable)
            {
                Node nearestWalkable = FindNearestWalkableNode(startNode);
                if (nearestWalkable != null)
                {
                    startNode = nearestWalkable;
                    startNode.gCost = 0;
                }
                else
                {
                    return null;
                }
            }

            List<Node> openSet = new List<Node>();       // Các node cần được kiểm tra
            HashSet<Node> closedSet = new HashSet<Node>(); // Các node đã được kiểm tra

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Lấy node có fCost (gCost + hCost) thấp nhất
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                       (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // Nếu đạt đến target (hoặc node walkable gần target), dựng lại đường đi
                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                // Duyệt các neighbor của currentNode
                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                        continue;

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            // Nếu không tìm được đường đi tới target, ta có thể chọn node trong closedSet có hCost nhỏ nhất
            Node bestNode = null;
            int bestHCost = int.MaxValue;
            foreach (Node node in closedSet)
            {
                if (node.hCost < bestHCost)
                {
                    bestHCost = node.hCost;
                    bestNode = node;
                }
            }

            if (bestNode != null)
            {
                return RetracePath(startNode, bestNode);
            }

            return null;
        }



        private Node FindNearestWalkableNode(Node target)
        {
            // Nếu target đã walkable thì trả về ngay
            if (target.walkable)
                return target;

            Queue<Node> queue = new Queue<Node>();
            HashSet<Node> visited = new HashSet<Node>();

            queue.Enqueue(target);
            visited.Add(target);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();
                foreach (Node neighbor in GetNeighbors(current))
                {
                    if (visited.Contains(neighbor))
                        continue;

                    visited.Add(neighbor);
                    // Nếu neighbor walkable, trả về ngay
                    if (neighbor.walkable)
                        return neighbor;

                    // Nếu không, thêm neighbor vào hàng đợi để tiếp tục tìm kiếm
                    queue.Enqueue(neighbor);
                }
            }
            // Nếu không tìm được node nào (trường hợp hiếm gặp), trả về null
            return null;
        }


        // Dựng lại đường đi từ đích về điểm bắt đầu
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            int loop = 0;

            while (currentNode != startNode && loop <= 1000)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
                loop++;
            }
            path.Reverse();
            return path;
        }

        // Tính khoảng cách giữa 2 node (dùng chi phí 14 cho đường chéo và 10 cho đường thẳng)
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        // Lấy node tương ứng với vị trí thế giới
        private Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
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

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        //void OnDrawGizmos()
        //{
        //    //return;
        //    // Vẽ khung giới hạn của grid
        //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        //    // Nếu grid đã được tạo, vẽ các node
        //    if (grid != null)
        //    {
        //        for (int x = 0; x < gridSizeX; x++)
        //        {
        //            for (int y = 0; y < gridSizeY; y++)
        //            {
        //                Node node = grid[x, y];
        //                // Chọn màu: trắng nếu có thể đi được, đỏ nếu không thể đi
        //                Gizmos.color = (node.walkable) ? Color.white : Color.red;
        //                // Vẽ một hình hộp nhỏ tại vị trí của node
        //                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));                        
        //            }
        //        }
        //    }
        //}

    }
}

