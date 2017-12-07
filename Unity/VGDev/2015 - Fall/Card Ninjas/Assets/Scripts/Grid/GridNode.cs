using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Util;

namespace Assets.Scripts.Grid
{
    public class GridNode : MonoBehaviour
    {
        [SerializeField]
        private Material red;
        [SerializeField]
        private Material blue;
		[SerializeField]
		private Material destroyed;
        [SerializeField]
        private Material white;

        private GridNode[] neighbors = new GridNode[4];
        private bool occupied = false;
        private Character panelOwner;
        private Enums.FieldType type;
        private Enums.FieldType prevType;
        private Vector2 position;
        private float hold = 0;
        private bool first = true;

        public GridNode Up
        {
            get { return neighbors[(int)Enums.Direction.Up]; }
            set { neighbors[(int)Enums.Direction.Up] = value; }
        }
        public GridNode Down
        {
            get { return neighbors[(int)Enums.Direction.Down]; }
            set { neighbors[(int)Enums.Direction.Down] = value; }
        }
        public GridNode Left
        {
            get { return neighbors[(int)Enums.Direction.Left]; }
            set { neighbors[(int)Enums.Direction.Left] = value; }
        }
        public GridNode Right
        {
            get { return neighbors[(int)Enums.Direction.Right]; }
            set { neighbors[(int)Enums.Direction.Right] = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool Occupied
        {
            get { return occupied; }
        }

        public Character Owner
        {
            get { return panelOwner; }
            set
            {
                occupied = true;
                panelOwner = value;
            }
        }

        public Enums.FieldType Type
        {
            get { return type; }
            set {
                if (first)
                {
                    prevType = value;
                    first = false;
                }
                else
                    prevType = type;
                type = value;
                if (type == Enums.FieldType.Red)
                    GetComponent<Renderer>().material = red;
                else if (type == Enums.FieldType.Blue)
                    GetComponent<Renderer>().material = blue;
				else if (type == Enums.FieldType.Destroyed)
					GetComponent<Renderer>().material = destroyed;
                else
                    GetComponent<Renderer>().material = white;
                this.gameObject.tag = type.ToString();
            }
        }

        public bool panelExists(Enums.Direction direction)
        {
            if (direction == Enums.Direction.Up)
                return Up != null;
            if (direction == Enums.Direction.Down)
                return Down != null;
            if (direction == Enums.Direction.Left)
                return Left != null;
            if (direction == Enums.Direction.Right)
                return Right != null;
            return false;
        }

        public bool panelNotDestroyed(Enums.Direction direction)
        {
            if (direction == Enums.Direction.Up)
                return Up != null && (Up.type != Enums.FieldType.Destroyed);
            if (direction == Enums.Direction.Down)
                return Down != null && (Down.type != Enums.FieldType.Destroyed);
            if (direction == Enums.Direction.Left)
                return Left != null && (Left.type != Enums.FieldType.Destroyed);
            if (direction == Enums.Direction.Right)
                return Right != null && (Right.type != Enums.FieldType.Destroyed);
            return false;
        }

        public bool panelAllowed(Enums.Direction direction, Enums.FieldType type)
        {
			//Perform a check to verify that an adjacent tile exists physically and has not
			//been destroyed by a player.
			
			if (direction == Enums.Direction.Up)
				return (Up != null) && (Up.type == type) && (Up.type != Enums.FieldType.Destroyed);
			if (direction == Enums.Direction.Down)
				return (Down != null) && (Down.type == type) && (Down.type != Enums.FieldType.Destroyed);
			if (direction == Enums.Direction.Left)
				return (Left != null) && (Left.type == type) && (Left.type != Enums.FieldType.Destroyed);
			if (direction == Enums.Direction.Right)
				return (Right != null) && (Right.type == type) && (Right.type != Enums.FieldType.Destroyed);
			return false;
        }

        public void clearOccupied()
        {
            occupied = false;
            panelOwner = null;
        }

        void Update()
        {
            if(type != prevType)
            {
                hold += Time.deltaTime;
                if(hold > 6f)
                {
                    type = prevType;
                    if (type == Enums.FieldType.Red)
                        GetComponent<Renderer>().material = red;
                    else if (type == Enums.FieldType.Blue)
                        GetComponent<Renderer>().material = blue;
                    else if (type == Enums.FieldType.Destroyed)
                        GetComponent<Renderer>().material = destroyed;
                    else
                        GetComponent<Renderer>().material = white;
                    this.gameObject.tag = type.ToString();
                }
            }
        }
    }
}
