using UnityEngine;

public class PassengerMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    private Transform[] _pathPoints;
    private int _currentIndex = 0;
    private bool _isMoving = false;

    public void SetPath(Transform[] points)
    {
        _pathPoints = points;
        _currentIndex = 0;
    }

    public void StartMoving()
    {
        if (_pathPoints == null || _pathPoints.Length == 0) return;
        _isMoving = true;
    }

    private void Update()
    {
        if (!_isMoving || _pathPoints == null || _pathPoints.Length == 0) return;

        Transform target = _pathPoints[_currentIndex];
        Vector3 targetPos = new Vector3(
            target.position.x,
            transform.position.y, 
            target.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            _currentIndex++;

            if (_currentIndex >= _pathPoints.Length)
            {
                _isMoving = false;
               
            }
        }
    }
}
