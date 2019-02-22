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

    public bool IsLoved { get; private set; }

    private Lovable LoveInterest { get; set; }

    private bool _isMoving = false;

    private bool _isRotating = false;

    private float _targetRotation;

    private Vector3 _targetPosition;

    private Vector3 _velocity;

    [SerializeField]
    private GameObject HeartsPrefab = null;

    [SerializeField]
    private AudioSource WinAudioSource = null;

    [SerializeField]
    private AudioSource RotateAudioSource = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public const int UpRotation = 0;
    public const int RightRotation = 270;
    public const int DownRotation = 180;
    public const int LeftRotation = 90;

    public void SetRotation(int rotation)
    {
        _targetRotation = rotation;

        EndRotation();
    }

    public void ChangeDirection()
    {
        RotateAudioSource.Play();

        _targetRotation = ((rb.rotation - 90) + 360) % 360;
        _isRotating = true;
    }

    //private void OnMouseUp()
    //{
    //    if (IsInLove)
    //        return;

    //    if (_isRotating)
    //        EndRotation();

    //    if (CanFallInLove())
    //        FallInLove();
    //    else if (!IsInLove)
    //        ChangeDirection();
    //}

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

        Instantiate(HeartsPrefab, transform);
        IsInLove = true;

        var lovable = GetLoveInSight();
        LoveInterest = lovable;

        if (lovable.LoveInterest != this)
            LevelManager.Lovables--;

        transform.SetParent(lovable.transform);

        DebugSpriteRenderer.color = Color.red;

        _targetPosition = lovable.transform.position - transform.up;
        _targetPosition = new Vector3(Mathf.RoundToInt(_targetPosition.x), Mathf.RoundToInt(_targetPosition.y), _targetPosition.z);
        _isMoving = true;
    }

    public void Update()
    {
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

        if (IsInLove || _isMoving || _isRotating)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (GetComponentInChildren<BoxCollider2D>().OverlapPoint(mousePosition))
            {
                DoLeftClick();
                return;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (GetComponentInChildren<BoxCollider2D>().OverlapPoint(mousePosition))
            {
                DoRightClick();
                return;
            }
        }
    }

    private void DoLeftClick()
    {
        if (CanFallInLove())
            FallInLove();
    }

    private void DoRightClick()
    {
        if (!IsInLove)
            ChangeDirection();
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

        if (LevelManager.Lovables == 1)
            StartCoroutine(Win());
    }

    private IEnumerator Win()
    {
        WinAudioSource.Play();

        yield return new WaitForSeconds(1);

        LevelManager.Win();
    }
}
