using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Lovable : MonoBehaviour
{
    public SpriteRenderer DebugSpriteRenderer;

    private Rigidbody2D rb;

    public bool IsInLove { get; private set; }

    public List<Lovable> Lovers { get; } = new List<Lovable>();

    public bool _isMoving = false;

    public bool _isRotating = false;

    public float _targetRotation;

    public Vector3 _targetPosition;

    private Vector3 _velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ChangeDirection()
    {
        _targetRotation = ((rb.rotation - 90) + 360) % 360;
        _isRotating = true;
    }

    private void OnMouseUp()
    {
        if (IsInLove)
            return;

        if (_isRotating)
            EndRotation();

        if (CanFallInLove())
            FallInLove();
        else if (!IsInLove)
            ChangeDirection();
    }

    public bool CanFallInLove()
    {
        return !IsInLove && !_isRotating && GetLoveInSight() != null;
    }

    private Lovable GetLoveInSight()
    {
        return Physics2D.Raycast(transform.position + transform.up * 0.5f, transform.up).collider?.GetComponentInParent<Lovable>();
    }

    public void FallInLove()
    {
        EndRotation();

        IsInLove = true;

        var lovable = GetLoveInSight();
        lovable.Lovers.Add(this);

        transform.SetParent(lovable.transform);

        DebugSpriteRenderer.color = Color.red;

        _targetPosition = lovable.transform.position - transform.up;
        _targetPosition = new Vector3(Mathf.RoundToInt(_targetPosition.x), Mathf.RoundToInt(_targetPosition.y), _targetPosition.z);
        _isMoving = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (_isMoving)
        {
            float distance = Vector3.Distance(transform.position, _targetPosition);

            if (distance < 0.05f)
            {
                EndMove();
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, distance * Time.deltaTime);
            }
        }

        if (_isRotating)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(rb.rotation, _targetRotation)) < 0.01f)
            {
                EndRotation();
            }
            else
            {
                rb.rotation = Mathf.LerpAngle(rb.rotation, _targetRotation, 30 * Time.deltaTime);
            }
        }
    }

    private void EndRotation()
    {
        rb.rotation = _targetRotation;

        _isRotating = false;
    }

    private void EndMove()
    {
        transform.position = _targetPosition;

        _isMoving = false;
    }
}
