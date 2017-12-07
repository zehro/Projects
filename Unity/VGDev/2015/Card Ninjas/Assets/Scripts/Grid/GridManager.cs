using UnityEngine;

namespace Assets.Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridNode Tile;
        [SerializeField]
        private int gridWidth = 3;
        [SerializeField]
        private int gridHeight = 6;

        private GridNode[,] grid;
        private bool[,] filledArray;
        private float tileWidth;
        private float tileHeight;

        public GridNode[,] Grid
        {
            get
            {
                if (grid == null)
                {
                    setSizes();
                    createGrid();
                }
                return grid;
            }
        }

        void Start()
        {
            if (grid == null)
            {
                setSizes();
                createGrid();
            }
        }

        void setSizes()
        {
            tileWidth = Tile.GetComponent<Renderer>().bounds.size.x;
            tileHeight = Tile.GetComponent<Renderer>().bounds.size.z;
        }
        
        private void createGrid()
        {
            grid = new GridNode[gridWidth, gridHeight];
            filledArray = new bool[gridWidth, gridHeight];
            GridNode tile;
            for (int r = 0; r < gridWidth; r++)
            {
                for (int c = 0; c < gridHeight; c++)
                {
                    tile = Instantiate(Tile);
                    Vector2 gridPos = new Vector2(r, c);
                    tile.transform.position = calcWorldCoord(gridPos);
                    grid[r, c] = tile;
                    filledArray[r, c] = false;
                }
            }

            //NOTES on how the grid system works.
            //It's a HACK. Just how it is. Nevertheless:
            //UP: 			y--
            //DOWN:			y++
            //RIGHT:		x--
            //LEFT:			x++
            //Input FORMAT:	[y,x]
            for (int x = 0; x < gridHeight; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    if (y > 0)
                        grid[y, x].Up = grid[y - 1, x];
                    if (y < gridWidth - 1)
                        grid[y, x].Down = grid[y + 1, x];
                    if (x > 0)
                        grid[y, x].Right = grid[y, x - 1];
                    if (x < gridHeight - 1)
                        grid[y, x].Left = grid[y, x + 1];
                    grid[y, x].Position = new Vector2(y, x);
                }
            }

            for (int x = 0; x < gridHeight / 2; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    grid[y, x].Type = Util.Enums.FieldType.Blue;
                }
            }
            for (int x = gridHeight / 2; x < gridHeight; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    grid[y, x].Type = Util.Enums.FieldType.Red;
                }
            }
        }

        //Method to calculate the position of the first tile
        //The center of the grid is (0,0,0)
        Vector3 calcInitPos()
        {
            //the initial position will be in the left upper corner
            return new Vector3(-tileWidth * gridWidth / 2f + tileWidth / 2, 0, gridHeight / 2f * tileHeight - tileHeight / 2);
        }

        //method used to convert hex grid coordinates to game world coordinates
        public Vector3 calcWorldCoord(Vector2 gridPos)
        {
            Vector3 initPos = calcInitPos();
            float x = initPos.x + gridPos.x * tileWidth;
            float z = initPos.z - gridPos.y * tileHeight;
            return new Vector3(x, 0, z);
        }
    }
}