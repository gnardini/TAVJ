using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public Transform _health;
    public Transform _missingHealth;

    private float _totalScale;
    private float _healthPercent;

	void Start () {
        _totalScale = _health.localScale.z;
	}
	
	void Update () {
        float healthScaleSize = _healthPercent * _totalScale;
        float missingHealthScaleSize = _totalScale - healthScaleSize;
        float healthCenter = healthScaleSize / 2 - _totalScale / 2;
        float missingHealthCenter = _totalScale / 2 - missingHealthScaleSize / 2;
        SetPosition(_health, healthCenter);
        SetScale(_health, healthScaleSize);
        SetPosition(_missingHealth, missingHealthCenter);
        SetScale(_missingHealth, missingHealthScaleSize);
	}

    public void SetHealthPercent(float percent) {
        _healthPercent = percent;
    }

    private void SetPosition(Transform transform, float zValue) {
        Vector3 position = transform.localPosition;
        position.z = zValue;
        transform.localPosition = position;
    }

    private void SetScale(Transform transform, float zValue) {
        Vector3 scale = transform.localScale;
        scale.z = zValue;
        transform.localScale = scale;
    }
}
