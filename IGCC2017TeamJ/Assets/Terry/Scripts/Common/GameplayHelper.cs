using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHelper {
    
    public static bool HasObstaclesBetween(Vector3 _source, Vector3 _target, List<string> _obstacleTags) {
        Vector3 rayDirection = _target - _source;
        RaycastHit[] result = Physics.RaycastAll(_source, rayDirection, rayDirection.magnitude);
        for (int i = 0; i < result.Length; ++i) {
            Collider hitCollider = result[i].collider;
            GameObject hitGameObject = hitCollider.gameObject;

            for (int j = 0; j < _obstacleTags.Count; ++j) {
                if (hitGameObject.tag == _obstacleTags[j]) {
                    return true;
                }
            }
        }

        return false;
    }

    public static float GetDistanceBetween(Vector3 _source, Vector3 _target) {
        return (_source - _target).magnitude;
    }

    public static float GetSquaredDistanceBetween(Vector3 _source, Vector3 _target) {
        return (_source - _target).sqrMagnitude;
    }

}
