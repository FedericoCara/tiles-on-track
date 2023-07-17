public readonly struct TileSpawnData {
    public Tile Tile { get; }
    public Enemy Enemy { get; }

    public TileSpawnData(Tile tile, Enemy enemy = null) {
        Tile = tile;
        Enemy = enemy;
    }

    public bool IsEmpty => Tile == null;

    public static TileSpawnData Empty() => new();
}