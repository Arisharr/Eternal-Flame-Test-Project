using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayableCharactersBehaviour : MonoBehaviour
{
    public UnityAction OnRoleChange;
    public PCBAction currentPcbAction = PCBAction.None;
    public float speed = 5.0f;
    private float moveDirection = 1.0f;
    private Rigidbody2D rigidbody;
    private const float collisionCooldown = 0.75f;
    private float collisionCountdown;
    private bool isSelected = false;
    private Quaternion _rotation;
    private Direction curDirection;
    [SerializeField] private GameObject selectionPFX;
    [SerializeField] private DirectionParticleHandler directionParticle;
    [SerializeField] private GameObject graphic;

    private void OnEnable()
    {
        OnRoleChange += SetDirection;
    }

    private void OnDisable()
    {
        OnRoleChange -= SetDirection;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        switch (currentPcbAction)
        {
            case PCBAction.HorizontalMove:
                DoHorizontalMove();
                break;
            case PCBAction.VerticalMove:
                DoVerticalMove();
                break;
        }

        if (collisionCountdown > 0)
        {
            collisionCountdown -= Time.deltaTime;
        }

        if (collisionCountdown < 0) collisionCountdown = 0;
    }

    public void DoHorizontalMove()
    {
        Vector3 target = rigidbody.position + (Vector2.up * moveDirection * speed * Time.deltaTime);
        rigidbody.MovePosition(target);
    }
    
    
    public void DoVerticalMove()
    {
        Vector3 target = rigidbody.position + (Vector2.right * moveDirection * speed  * Time.deltaTime);
        rigidbody.MovePosition(target);
    }

    private void OnMouseDown()
    {
        GameManager.Instance.AddPCBs(this);
    }

    public void Selection(bool _selection)
    {
        isSelected = _selection;
        selectionPFX.SetActive(_selection);
    }

    private void FaceGraphicsTowardMovement()
    {
        switch (curDirection)
        {
            case Direction.Right:
                _rotation.eulerAngles = Vector3.forward * 0;
                break;
            case  Direction.Left:
                _rotation.eulerAngles = Vector3.forward * 180;
                break;
            case Direction.Up:
                _rotation.eulerAngles = Vector3.forward * 90;
                break;
            case Direction.Down:
                _rotation.eulerAngles = Vector3.forward * 270;
                break;
        }

        graphic.transform.rotation = _rotation;
    }

    private void SetDirection()
    {
        if (currentPcbAction == PCBAction.VerticalMove)
        {
            if (moveDirection > 0) curDirection = Direction.Right;
            if (moveDirection < 0) curDirection = Direction.Left;
        }
        
        else if (currentPcbAction == PCBAction.HorizontalMove)
        {
            if (moveDirection > 0) curDirection = Direction.Up;
            if (moveDirection < 0) curDirection = Direction.Down;
        }
        
        FaceGraphicsTowardMovement();
    }

    public void ShowDirection()
    {
        directionParticle.ShowDirection(curDirection);
    }

    public void DisableDirection()
    {
        directionParticle.ResetParticles();
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (currentPcbAction == PCBAction.VerticalMove)
        {
            if (col.collider.tag == "Vertical Wall")
            {
                ChangeDirection();
            }
        }
        else if (currentPcbAction == PCBAction.HorizontalMove)
        {
            if (col.collider.tag == "Horizontal Wall")
            {
                ChangeDirection();
            }
        }
        
        if (col.gameObject.tag == "Obstacles" || col.gameObject.tag == "Player" || col.gameObject.tag == "NPC")
        {
            ChangeDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Laser")
        {
            LineRenderer lineR = col.GetComponent<LineRenderer>();
            Vector3 collisionPoint = col.ClosestPoint(transform.position);
            lineR.SetPosition(1, collisionPoint);
            Kill();
        }
    }

    private void ChangeDirection()
    {
        if (collisionCountdown != 0)
            return;
        moveDirection = -moveDirection;
        collisionCountdown = collisionCooldown;
        
        SetDirection();
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }
}


public enum PCBAction
{
    None = 0,

    HorizontalMove = 1,
    VerticalMove = 2,
}
public enum Direction
{
    Right,Left,Up,Down
}