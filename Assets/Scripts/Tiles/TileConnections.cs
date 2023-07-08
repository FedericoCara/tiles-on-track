﻿public struct TileConnections {
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

    public Tile GetCorrectConnection() {
        if (Down != null && Down.FollowingTile == null &&
            Down.NextTileDirection == TileDirection.UP)
            return Down;
        if (Left != null && Left.FollowingTile == null &&
            Left.NextTileDirection == TileDirection.RIGHT)
            return Left;
        if (Up != null && Up.FollowingTile == null &&
            Up.NextTileDirection == TileDirection.DOWN)
            return Up;
        return null;
    }
}