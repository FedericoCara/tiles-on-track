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
        StartCoroutine(CheckIfItIsStillEndReached());
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

    private IEnumerator CheckIfItIsStillEndReached() {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            if (endReached) {
                StartNextTile();
            }
        }
    }

    private void StartNextTile() {
        var followingTile = currentTile.FollowingTile;
        pointFollowing = null;
        currentIndex = 0;
        if (followingTile == null)
            SetEndReached();
        else 
            UpdateCurrentTile(followingTile);
    }

    private void UpdateCurrentTile(Tile followingTile) {
        currentTile = followingTile;
        endReached = false;
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
