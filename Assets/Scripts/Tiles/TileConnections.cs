public struct TileConnections {
    public Tile Center { get; }
    public Tile Up { get; }
    public Tile Left { get; }
    public Tile Right { get; }
    public Tile Down { get; }

    public TileConnections(Tile center, Tile up, Tile left, Tile right, Tile down) {
        Center = center;
        Up = up;
        Left = left;
        Right = right;
        Down = down;
    }
}