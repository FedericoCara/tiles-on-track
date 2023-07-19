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
    private bool isTraversingInReverse = false;
    private bool endReached = false;
    private bool fighting = false;
    private bool drinking = false;
    public Action<Enemy, bool> OnFightStarted = (enemy, isTraversingInReverse) => { };
    public Action<Potion> OnDrinkingStarted = potion => { };

    private void Start() {
        currentTile = startingTile;
        StartCoroutine(CheckIfItIsStillEndReached());
    }

    // Update is called once per frame

    void Update()
    {
        if(endReached || fighting || drinking)
            return;
        
        if (pointFollowing == null) {
            SetNextPoint();
        } else {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            if (PointReached()) {
                if (IsLastPointReached()) {
                    StartNextTile();
                } else if (IsFightPointReached()) {
                    StartFight();
                } else if (IsPotionPointReached()) {
                    DrinkPotion();
                } else {
                    SetNextPoint();
                }
            }
        }
    }

    private bool IsLastPointReached() {
        return isTraversingInReverse ? pointFollowing.isFirst : pointFollowing.isLast;
    }

    private bool IsFightPointReached() {
        return pointFollowing.stopsForEnemy;
    }

    private bool IsPotionPointReached() {
        return pointFollowing.stopsForPotion;
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
        if (followingTile == null)
            SetEndReached();
        else 
            UpdateCurrentTile(followingTile);
    }

    private void StartFight() {
        fighting = true;
        OnFightStarted(currentTile.Enemy, isTraversingInReverse);
    }

    private void DrinkPotion() {
        drinking = true;
        OnDrinkingStarted(currentTile.Potion);
    }

    private void UpdateCurrentTile(Tile followingTile) {
        isTraversingInReverse = currentTile.IsNextTileInReverse;
        if(isTraversingInReverse)
            currentIndex = followingTile.GetLastPointIndex();
        else 
            currentIndex = 0;
        
        currentTile = followingTile;
        endReached = false;
    }

    private bool PointReached() =>
        Vector2.SqrMagnitude(pointFollowing.point - (Vector2) transform.position) < 0.05f;

    private void SetNextPoint() {
        pointFollowing = isTraversingInReverse? currentTile.GetPreviousPoint(currentIndex) : currentTile.GetNextPoint(currentIndex);
        if (pointFollowing != null) {
            if (isTraversingInReverse)
                currentIndex--;
            else
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

    public void SetFightFinished() {
        fighting = false;
        TurnToPoint();
        SetNextPoint();
    }

    public void SetDrinkingFinished() {
        drinking = false;
        SetNextPoint();
    }

    public void Turn180() {
        transform.right = -transform.right;
    }
    
}
