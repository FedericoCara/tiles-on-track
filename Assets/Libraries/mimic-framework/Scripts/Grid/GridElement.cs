using System;
using System.Collections.Generic;

using UnityEngine;

namespace Mimic.Grid {

    public class GridElement : MonoBehaviour {
        
        [SerializeField]
        protected int width = 1;
        public int Width { 
            get{ return width; }
        }

        [SerializeField]
        protected int height = 1;
        public int Height { 
            get{ return height; }
        }

        public Vector2 Position {
            get{ return (Vector2) transform.position; }
            set{ transform.position = new Vector3(value.x, value.y, transform.position.z); }
        }

        public Vector2 MinBounds {
            get{ return new Vector2(transform.position.x - width / 2f, transform.position.y - height / 2f); }
        }

        public Vector2 MaxBounds {
            get{ return new Vector2(transform.position.x + width / 2f, transform.position.y + height / 2f); }
        }

        [SerializeField]
        protected Color invalidColor = new Color(1,0,0,0.5f);

        [SerializeField]
        protected SpriteRenderer renderer;

        [SerializeField]
        protected bool canBeMoved = true;
        protected bool isBeingDragged = false;
        public virtual bool IsBeingDragged {
            protected set { 
                if(isBeingDragged != value) {                    
                    isBeingDragged = value;
                    if(isBeingDragged) {
                        OnStartedBeingDragged();
                    } else {
                        OnStoppedBeingDragged();
                    }
                }
            }
            get { return isBeingDragged; }
        }

        protected bool invalid = false;
        public bool Invalid {
            set{ 
                renderer.color = value ? invalidColor : Color.white;
                invalid = value;
            }
        }

        protected List<Cell> currentCells;
        protected Vector3 placementPosition;

        public virtual bool Placed {
            get{ return currentCells != null && !currentCells.IsEmpty(); }
        }

        protected virtual void Awake() {
            if(renderer == null) {
                renderer = GetComponent<SpriteRenderer>();
            }
        }

        protected virtual void Update() {
            if(isBeingDragged) {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;
                transform.position = position;
                currentCells[0].Grid.Snap(this);
            }
        }

        protected virtual void OnMouseDown(){
            if(Placed) {
                IsBeingDragged = true;                
            }
        }

        protected virtual void OnMouseUp(){
            IsBeingDragged = false;
        }

        public virtual void Place(List<Cell> cells) {    
            if(currentCells != null) {        
                currentCells.ForEach(cell => cell.Content = null);        
            }
            cells.ForEach(cell => cell.Content = this);
            currentCells = cells;            
            renderer.sortingOrder = -cells[0].Row; 
            placementPosition = transform.position;
        }

        protected virtual void OnStartedBeingDragged() {
        }

        protected virtual void OnStoppedBeingDragged(){
            if(!currentCells[0].Grid.Place(this)){
                transform.position = placementPosition;
                Invalid = false;
            }
        }

    }

}
