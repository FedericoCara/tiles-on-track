using UnityEngine;

namespace Mimic.Grid {

    public class Cell : MonoBehaviour {

        protected int row;
        public int Row {
            get{ return row; }
            set{ row = value; }
        }
        
        protected int column;
        public int Column {
            get{ return column; }
            set{ column = value; }
        }

        private IGrid grid;
        public IGrid Grid {
            get { return grid; }
            set { grid = value; }
        }        

        protected GridElement content;
        public GridElement Content {
            get{ return content; }
            set{ content = value; }
        }
        
    }

}
