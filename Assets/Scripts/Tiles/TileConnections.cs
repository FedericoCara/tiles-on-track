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

    public Tile GetCorrectConnection(TileDirection comingDirection) {
        if (Down != null && Down.CanConnectWith(comingDirection, TileDirection.UP))
            return Down;
        if (Left != null && Left.CanConnectWith(comingDirection, TileDirection.RIGHT))
            return Left;
        if (Up != null && Up.CanConnectWith(comingDirection, TileDirection.DOWN))
            return Up;
        return null;
    }
}