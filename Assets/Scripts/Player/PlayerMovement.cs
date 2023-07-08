using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private Tile startingTile;
    [SerializeField] private float speed = 1;
    private Tile currentTile;
    private TilePoint pointFollowing;
    private Vector2 direction;
    private int currentIndex;
    private bool endReached = false;

    private void Start() {
        currentTile = startingTile;
    }

    // Update is called once per frame
    void Update()
    {
        if(endReached)
            return;
        
        if (pointFollowing == null) {
            SetNextPoint();
        } else {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            if (PointReached(pointFollowing)) {
                if (pointFollowing.isLast) {
                    StartNextTile();
                } else {
                    SetNextPoint();
                }
            }
        }
    }

    private void StartNextTile() {
        currentTile = currentTile.GetFollowingTile();
        pointFollowing = null;
        currentIndex = 0;
        if (currentTile == null)
            SetEndReached();
    }

    private bool PointReached(TilePoint tilePoint) =>
        Vector2.SqrMagnitude(pointFollowing.point - (Vector2) transform.position) < 0.05f;

    private void SetNextPoint() {
        pointFollowing = currentTile.GetNextPoint(currentIndex);
        if (pointFollowing != null) {
            currentIndex++;
            direction = pointFollowing.point - (Vector2) transform.position;
            direction.Normalize();  
            TurnToPoint();
        }
    }

    private void TurnToPoint() {
        transform.right = direction;
    }

    private void SetEndReached() {
        endReached = true;
    }
}
