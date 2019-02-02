using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{
    public enum Direction
    {
        Arriba, Abajo, Izquierda, Derecha
    };
    public class Vector2Int
    {
        private int x;
        private int y;

        public Vector2Int(int x1, int y1) {
            x = x1;
            y = y1;
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
    }
    public class Walker
    {
        private Direction _direction;
        private Vector2Int actualPosition;

        public Walker(Direction dir, Vector2Int initialPoint)
        {
            _direction = dir;
            actualPosition = initialPoint;
        }
        public Direction Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }
        public Vector2Int ActualPosition
        {
            get
            {
                return actualPosition;
            }
            set
            {
                actualPosition = value;
            }
        }
    }

    public class LevelGenerator : MonoBehaviour
    {
       
        [SerializeField] private int changeDirection = 40, placeBlock = 12, numMovements=100;
        [SerializeField] private int numwalkers=5, height=14, width=14, maxheight3d=4;

        [SerializeField] GameObject _parentHolder; //empty gameobject that will sustain all the childs;
        private int[,] map;
        ///El mapa tiene los ejes X-Z en vez de i-j puesto que el mapa en los ejes de unity debe de estar acostado.
        private int[,] positionsUsedByEnemy;
        private Walker[] walkers;
        private Vector2Int midPoint;
        private GameObject _floorTile;

        private string _floorTileName = "/FloorTile";
        private string _enemyModel = "/EnemyModel";
        private int placedBlocks = 0;

        private bool _levelWasGenerated = false;

        public bool LevelWasGenerated
        {
            get { return _levelWasGenerated; }
        }
   
        public void GenerateLevel()
        {
            _floorTile = Resources.Load(ControlManager.Instance.PathToDungeonTiles + _floorTileName) as GameObject;
            map = new int[height, width];
            for (int x = 0; x < height; x++)
                for (int z = 0; z < width; z++)
                {
                    map[x, z] = 0;
                    /*GameObject go = Instantiate(_floorTile, new Vector3(x, map[x, z], z)
                                , Quaternion.identity, _parentHolder.transform);
                    go.transform.name = x.ToString() + "-" + map[x, z].ToString() + "-" + z.ToString();*/
                }
            midPoint = new Vector2Int(Mathf.RoundToInt(height / 2), Mathf.RoundToInt(width / 2));
            walkers = new Walker[numwalkers];
            StartToGenerateLevel();
            StartCoroutine(BuildDungeonCoroutine());
        }

        void StartToGenerateLevel()
        {
            for (int x=0; x < numwalkers; x++)
                walkers[x] = new Walker((Direction)Random.Range(0, 3), midPoint);
            for (int num_mov = 0; num_mov < numMovements; num_mov++)
                for (int x = 0; x < numwalkers; x++)
                    MoveWalker(x);
        }

        /// <summary>
        /// This method allows for one movement of the actual walker passed as argument.
        /// </summary>
        /// <param name="index">The index of the walker in the array of walkers</param>
        void MoveWalker(int index)
        {
            int shouldChangeDirection = Random.Range(0, 99);
            if (changeDirection > shouldChangeDirection)     //to changeDirection 4% -> i only should change if random [0-4] 
            {
                int movement = Random.Range(1, 4);
                if ((Direction)movement == walkers[index].Direction) movement = (movement + 1) % 4;
                walkers[index].Direction = (Direction)movement;
            }
            if (!MovementAvailable(index)) ChangeToDesirableDirection(index);
            walkers[index].ActualPosition = TransformDirectionIntoAxis(index);
            Vector2Int nextLocation = walkers[index].ActualPosition;
             int shouldIncreaseTerrain = Random.Range(0, 99);
            if (placeBlock >= shouldIncreaseTerrain)            //to placeBloock 40% -> i only should place if random [0..40]
            {
                if (map[nextLocation.X, nextLocation.Y] < maxheight3d)
                {
                    map[nextLocation.X, nextLocation.Y]++;
                }
            }
            
        }

        /// <summary>
        /// This method returns if a movement is available (it would NOT move the walker
        /// beyond the limits of the map).
        /// </summary>
        bool MovementAvailable(int index)
        {
            bool available = false;
            switch (walkers[index].Direction)
            {
                case Direction.Abajo:
                    if (walkers[index].ActualPosition.X < height - 1) available = true;
                    break;
                case Direction.Arriba:
                    if (walkers[index].ActualPosition.X > 0) available = true;
                    break;
                case Direction.Derecha:
                    if (walkers[index].ActualPosition.Y < width - 1) available = true;
                    break;
                case Direction.Izquierda:
                    if (walkers[index].ActualPosition.Y > 0) available = true;
                    break;
                default:
                    break;
            }
            return available;
        }

        /// <summary>
        /// This method allows to swap the direction of a walker to another one that's available. 
        /// </summary>
        void ChangeToDesirableDirection(int index)
        {
     
            List<int> posibles = new List<int>();
            for (int i = 0; i < System.Enum.GetNames(typeof(Direction)).Length; i++) posibles.Add(i);
            int newDirection = -1;
            do
            {
                newDirection = posibles[Mathf.RoundToInt(Random.Range(0, posibles.Count - 1))];
                if ((Direction)newDirection == walkers[index].Direction) posibles.RemoveAt(newDirection);
                else walkers[index].Direction = (Direction)newDirection;
            } while (!MovementAvailable(index));
            walkers[index].Direction = (Direction)newDirection;
        }

        /// <summary>
        /// This method returns in what position on the 'grid' should the walker move
        /// we assume that the position is already posible so it's always valid.
        /// </summary>
        Vector2Int TransformDirectionIntoAxis(int index)
        {
            Vector2Int result = new Vector2Int(-1, -1);
            switch (walkers[index].Direction)
            {
                case Direction.Abajo:
                    result = new Vector2Int(walkers[index].ActualPosition.X + 1, walkers[index].ActualPosition.Y);
                    break;
                case Direction.Arriba:
                    result = new Vector2Int(walkers[index].ActualPosition.X - 1, walkers[index].ActualPosition.Y);
                    break;
                case Direction.Derecha:
                    result = new Vector2Int(walkers[index].ActualPosition.X, walkers[index].ActualPosition.Y + 1);
                    break;
                case Direction.Izquierda:
                    result = new Vector2Int(walkers[index].ActualPosition.X, walkers[index].ActualPosition.Y - 1);
                    break;
                default:
                    break;
            }
            return result;
        }

        IEnumerator BuildDungeonCoroutine()
        {
            for (int x = 0; x < height; x++)        /*for each row*/
                for (int z = 0; z < width; z++)     /*for each column*/
                    for (int w = 0; w<=map[x,z]; w++) { /*for the maxium height 'til the floor, this will be our next height*/
                        GameObject go = Instantiate(_floorTile, new Vector3(x, w, z)
                           , Quaternion.identity, _parentHolder.transform);
                        go.transform.name = x.ToString() + "-" + map[x, z].ToString() + "-" + z.ToString();
                        yield return null; /*wait another frame until the next iteration*/
                    }
            _levelWasGenerated = true;
        }

        public void InstantiateEnemies(int levelEnemy, int numberEnemies)
        { 
            positionsUsedByEnemy = new int[height, width];
            for (int x = 0; x < height; x++)
                for (int z = 0; z < width; z++)
                    positionsUsedByEnemy[x, z] = 0;
            int auxWidth;
            int auxHeight;
            TypeEnemy enemyType;
            bool wasThePositionUsed;
            for (int i = 0; i < numberEnemies; i++) {
                wasThePositionUsed = true;
                do
                {
                    auxWidth = Random.Range(0, width);
                    auxHeight = Random.Range(0, height);
                    if ((positionsUsedByEnemy[auxHeight, auxWidth] == 0) && 
                        (map[auxHeight, auxWidth]==0)) 
                    {
                        wasThePositionUsed = false;
                        positionsUsedByEnemy[auxHeight, auxWidth] = 1;
                        GameObject enemyModel = Resources.Load(ControlManager.Instance.PathToDungeonTiles + _enemyModel) as GameObject;
                        //0.65f because 1 will be too high
                        GameObject go = Instantiate(enemyModel, new Vector3(auxHeight, map[auxHeight,auxWidth]+0.65f, auxWidth)
                          , Quaternion.identity, _parentHolder.transform);
                        enemyType = (TypeEnemy)Random.Range(0, (int)TypeEnemy.EnemySize);
                        go.GetComponent<Enemy>().Name = enemyType.ToString();
                        go.transform.name = enemyType.ToString()+": ("+auxHeight.ToString() + "-" + (map[auxHeight, auxWidth]+1).ToString() + "-" + auxWidth.ToString()+")";
                       
                    }
                } while (wasThePositionUsed);
            } 
           
        }

        public void SpawnBoss(int levelEnemy)
        {
            bool wasThePositionAvailable = false;
            int auxWidth, auxHeight;
            TypeEnemy enemyType;
            do
            {

                auxWidth = Random.Range(0, width);
                auxHeight = Random.Range(0, height);
                if (map[auxHeight, auxWidth] == 0)
                {
                    wasThePositionAvailable = true;
                    positionsUsedByEnemy[auxHeight, auxWidth] = 1;
                    GameObject enemyModel = Resources.Load(ControlManager.Instance.PathToDungeonTiles + _enemyModel) as GameObject;
                    //0.65f because 1 will be too high
                    GameObject go = Instantiate(enemyModel, new Vector3(auxHeight, map[auxHeight, auxWidth] + 0.65f, auxWidth)
                      , Quaternion.identity, _parentHolder.transform);
                    enemyType = (TypeEnemy)Random.Range(100, (int)TypeEnemy.BossSize);
                    go.GetComponent<Enemy>().Name = enemyType.ToString();
                    go.transform.name = enemyType.ToString() + ": (" + auxHeight.ToString() + "-" + (map[auxHeight, auxWidth] + 1).ToString() + "-" + auxWidth.ToString() + ")";

                }
            } while (wasThePositionAvailable == false);
        }

        public void DestroyLevel()
        {
            _levelWasGenerated = false;
            for (int i = 0; i < _parentHolder.transform.childCount; i++)
                Destroy(_parentHolder.transform.GetChild(0).gameObject);

        }

        public void SetPositionPlayer()
        {
            int auxWidth;
            int auxHeight;
            bool wasThePositionCorrect = false;
            while (!wasThePositionCorrect)
            {
                auxWidth = Random.Range(0, width);
                auxHeight = Random.Range(0, height);
                if (positionsUsedByEnemy[auxHeight, auxWidth] == 0 && map[auxHeight, auxWidth] == 0)
                {
                    wasThePositionCorrect = true;
                    //0.75f because 1f will be too high
                    GameManager.Instance.SetPositionPlayerInDungeon(new Vector3(auxHeight, map[auxHeight, auxWidth]+0.75f, auxWidth));
                }
            }
        }
    }
 }
    


