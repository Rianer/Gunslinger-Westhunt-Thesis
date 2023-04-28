using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(MovementConfig))]
public class MovementHandler : MonoBehaviour, IObserver<InputObserverArgs>
{
    private Vector2 movement;
    private Rigidbody2D rb;
    private MovementConfig movementConfig;
    private Transform playerPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = new Vector2();
        movementConfig = GetComponent<MovementConfig>();
        playerPosition = GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        rb.velocity = movement;
        GameManager.Instance.UpdatePlayerPosition(new Vector2(playerPosition.position.x, playerPosition.position.y));
    }
    public void UpdateObserver(InputObserverArgs args)
    {
        movement = args.NormalizedInputVector * movementConfig.speed;
    }
}
