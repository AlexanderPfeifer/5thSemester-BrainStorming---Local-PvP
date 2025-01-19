using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDirectionOfWinArea : MonoBehaviour
{
    [SerializeField] private RectTransform[] brainArrow;
    
    public List<Transform> players;

    [SerializeField] public WinningArea[] winningArea;
    
    public IEnumerator RotateArrowToWinningArea()
    {
        Vector3 _winningAreaPosition = transform.position;
        
        foreach (var _winningArea in winningArea)
        {
            if (_winningArea.canObtainPoints)
            {
                _winningAreaPosition = _winningArea.transform.position;
                _winningAreaPosition.y = 0f;
                break;
            }
        }
        
        if (_winningAreaPosition == transform.position)
        {
            Debug.Log("THERE IS NO WINNING AREA ASSIGNED");
            yield break;
        }
        
        // Store initial and target rotations for each arrow because I want to rotate both arrows parallel
        List<(Quaternion startRotation, Quaternion targetRotation, Transform arrowTransform)> _arrowData = new();
        
        for (int _i = 0; _i < players.Count; _i++)
        {
            var _player = players[_i];
            var _brainArrow = brainArrow[_i];
        
            // Calculate the target rotation for this player and arrow
            Vector3 _directionToTarget = _winningAreaPosition - _player.position;
            float _angle = -Mathf.Atan2(_directionToTarget.x, _directionToTarget.z) * Mathf.Rad2Deg;
        
            Quaternion _startRotation = _brainArrow.transform.rotation;
            Quaternion _targetRotation = Quaternion.Euler(new Vector3(0, 0, _angle));
        
            _arrowData.Add((_startRotation, _targetRotation, _brainArrow.transform));
        }
        
        float _elapsedTime = 0f;
        const float duration = .2f;

        while (_elapsedTime < duration)
        {
            float t = _elapsedTime / duration;
        
            // Rotate all arrows simultaneously
            foreach (var (_startRotation, _targetRotation, _arrowTransform) in _arrowData)
            {
                _arrowTransform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, t);
            }
        
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure all arrows reach their final rotation
        foreach (var (_, _targetRotation, _arrowTransform) in _arrowData)
        {
            _arrowTransform.rotation = _targetRotation;
        }

        StartCoroutine(RotateArrowToWinningArea());
    }
}
