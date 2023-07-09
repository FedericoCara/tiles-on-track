using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour {
    
    [SerializeField] private ConveyorTileStrategy strategy;
    [SerializeField] private float firstSpawnDelay = 2;
    [SerializeField] private float spawnDraggableFrequency = 2;
    [SerializeField] private DraggableTile draggableTilePrefab;
    [SerializeField] private int draggableLimit;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private float conveyorSpeed = 2;
    [SerializeField] private Transform conveyorEndPoint;
    [SerializeField] private Vector3 conveyorDirection = Vector3.down;
    [SerializeField] private float draggableSize = 5;

    private List<DraggableTile> _spawnedDraggables = new List<DraggableTile>();
    private List<Transform> _draggablePositions = new List<Transform>();
    private int _draggableCount;
    private Player _player;

    private void Awake() {
        _player = FindObjectOfType<Player>();
    }

    void Start() {
        StartCoroutine(SpawnDraggablesCoroutine());
    }

    private void Update() {
        MoveDraggables();
    }

    private void MoveDraggables() {
        for (int i = 0; i < _draggablePositions.Count; i++) {
            MoveDraggableTowardsEndPoint(i);
        }
    }

    private void MoveDraggableTowardsEndPoint(int i) {
        var targetPosition = conveyorEndPoint.position - conveyorDirection * i * draggableSize;
        var draggablePosition = _draggablePositions[i];
        
        if(Vector2.SqrMagnitude(targetPosition-draggablePosition.position)<0.05f)
            return;
        
        draggablePosition.position += conveyorDirection * conveyorSpeed * Time.deltaTime;
        var draggableSpawned = _spawnedDraggables[i];
        if (!draggableSpawned.IsDragging)
            draggableSpawned.transform.position = draggablePosition.position;
    }

    private IEnumerator SpawnDraggablesCoroutine() {
        yield return new WaitForSeconds(firstSpawnDelay);
        while (true) {
            if (_spawnedDraggables.Count < draggableLimit) {
                SpawnDraggable(strategy.CalculateTile(_player.Level, _player.HealthPercentage));
            }

            yield return new WaitForSeconds(spawnDraggableFrequency);
        }
    }

    private void SpawnDraggable(Tile tilePrefab) {
        _draggableCount++;
        var draggable = Instantiate(draggableTilePrefab, transform);
        draggable.transform.position = spawnTransform.position;
        draggable.name = draggable.name.Replace("(Clone)",$"-{_draggableCount:000}");
        var draggablePosition = new GameObject($"Draggable-{_draggableCount:000}").transform;
        draggablePosition.position = spawnTransform.position;
        draggablePosition.parent = transform;
        draggable.SetTilePrefab(tilePrefab);
        _spawnedDraggables.Add(draggable);
        _draggablePositions.Add(draggablePosition);

        draggable.OnReturnToInitialPosition += () => draggable.transform.position = draggablePosition.position;
        draggable.OnTileDropped += () => ClearTile(draggable);
    }

    private void ClearTile(DraggableTile draggable) {
        var indexOfDraggable = _spawnedDraggables.IndexOf(draggable);
        _spawnedDraggables.RemoveAt(indexOfDraggable);
        Destroy(_draggablePositions[indexOfDraggable].gameObject);
        _draggablePositions.RemoveAt(indexOfDraggable);
    }
}