using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Grid {

    public class Grid<T> : MonoBehaviour, IGrid where T : Cell {

        [SerializeField]
        protected int rows;
        public int Rows {
            get { return rows; }
        }

        [SerializeField]
        protected int cols;
        public int Cols {
            get { return cols; }
        }
            
        [SerializeField]
        protected T cellPrefab;

        public virtual (int rows, int cols) Size {
            get{ return (rows, cols); }
            set{ 
                if(value.rows >= 0 && value.cols >= 0 && (value.rows != rows || value.cols != cols)) {
                    rows = value.rows;
                    cols = value.cols;
                    Generate();
                    UpdateBounds();
                }
            }
        }

        [SerializeField]
        protected bool generateAtStart = true;
        
        protected T[,] cells;
        public T[,] Cells {
            get{ return cells; }
        }
        
        protected List<T> cellsList;
        public List<T> CellsList {
            get{ return cellsList; }
        }

        protected Bounds bounds;
        public Bounds Bounds {
            get{ return bounds; }
        }

        protected virtual Predicate<Tuple<T, GridElement>> PlacementCondition {
            get{ return tuple => tuple.Item1.Content != null && tuple.Item1.Content != tuple.Item2; }
        }

        protected virtual void Start() {
            UpdateBounds();
            if(generateAtStart) {
                Generate();
            }
        }
        
        protected virtual void Update() {
        }

        protected virtual void Generate() {
            transform.DestroyAllChildren();
            cells = Generate(rows, cols, transform, cellPrefab);
            if(cellsList == null) {
                cellsList = new List<T>(rows * cols);
            } else {
                cellsList.Clear();
            }
            for (int i = 0; i < rows; i++) {             
                for (int j = 0; j < cols; j++) {
                    cellsList.Add(cells[i,j]);
                    cells[i, j].Grid = this;
                }   
            }
        }

        private void UpdateBounds() {
            bounds = new Bounds(transform.position, new Vector2(cols, rows));
        }

        public void Snap(GridElement element) {
            if(bounds.Contains(element.transform.position)) {
                Vector2 position = element.Position;                
                position.x = SnapCoordinate(position.x, element.Width % 2 == 0, transform.position.x);
                position.y = SnapCoordinate(position.y, element.Height % 2 == 0, transform.position.y);
                element.Position = position;
                if(bounds.Contains(element.MinBounds) && bounds.Contains(element.MaxBounds)) {
                    List<T> overlappingCells = GetOverlappingCells(element);
                    element.Invalid = overlappingCells.IsEmpty() || overlappingCells.Exists(cell => PlacementCondition(new Tuple<T, GridElement>(cell, element)));
                } else {
                    element.Invalid = true;
                }
            } else {
                element.Invalid = true;
            }
        }

        public List<T> GetOverlappingCells(GridElement element) {            
            if(element != null && bounds.Contains(element.transform.position)) {
                return GetCells((int) (element.MinBounds.y - transform.position.y + rows / 2f), (int) (element.MinBounds.x - transform.position.x + cols / 2f), element.Width, element.Height);
            } else {
                return new List<T>();
            }
        }

        private List<T> GetCells(int row, int col, int width, int height) {            
            List<T> requestedCells = new List<T>(width * height);
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if(i + row >= 0 && i + row < cells.GetLength(0) && j + col >= 0 && j + col < cells.GetLength(1)) {                    
                        requestedCells.Add(cells[i + row, j + col]);
                    }
                }
            }
            return requestedCells;
        }

        private float SnapCoordinate(float coordinate, bool notCentered, float gridCenter) {            
            if(notCentered) {
                return gridCenter + Mathf.RoundToInt(coordinate - gridCenter);
            } else {
                return gridCenter + Mathf.Floor(coordinate - gridCenter) + 0.5f;
            }
        }
        
        public virtual bool Place(GridElement element) {
            List<T> overlappingCells = GetOverlappingCells(element);
            if(overlappingCells.Count == 0 || overlappingCells.Exists(cell => PlacementCondition(new Tuple<T, GridElement>(cell, element)))) {
                return false;
            } else {
                element.Place(new List<Cell>(overlappingCells));
                return true;    
            }
        }

        public T GetCell(Vector2 position) {
            position = (Vector2) transform.position - position; 
            float startX = -cols / 2f + 0.5f;
            float startY = -rows / 2f + 0.5f;
            
            int row = (int) (position.y + startY);
            if(row < 0 || row >= cells.GetLength(0)){
                return null;
            }

            int col = (int) (position.x + startX);
            if(col < 0 || col >= cells.GetLength(1)){
                return null;
            }

            return cells[row, col];
        }

        protected static T[,] Generate(int rows, int cols, Transform parent, T cellPrefab) {        
            float startX = -cols / 2f + 0.5f;
            float startY = -rows / 2f + 0.5f;
            T[,] cells = new T[rows, cols];
            T newCell;
            Vector3 position = Vector3.zero;
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    position.x = j + startX;
                    position.y = i + startY;
                    newCell = Instantiate<T>(cellPrefab, parent);
                    newCell.transform.localPosition = position;
                    newCell.Row = i;
                    newCell.Column = j;
                    newCell.name = "Cell ("+i+","+j+")";
                    cells[i, j] = newCell;
                }
            }
            return cells;
        }

    }

}
