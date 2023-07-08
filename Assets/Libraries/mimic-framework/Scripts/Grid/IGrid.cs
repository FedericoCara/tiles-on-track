namespace Mimic.Grid {

    public interface IGrid {        
        void Snap(GridElement element);        
        bool Place(GridElement element);
    }

}