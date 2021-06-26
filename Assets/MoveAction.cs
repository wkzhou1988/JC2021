using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EaseType {
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
}

public abstract class Ease {
    public abstract float Lerp(float value);
}

public class EaseLinear : Ease {
    public override float Lerp(float value) {
        return value;
    }
}

public class EaseIn : Ease {
    public override float Lerp(float value) {
        return 1f - Mathf.Cos((value * Mathf.PI) / 2);
    }
}

public class EaseOut : Ease {
    public override float Lerp(float value) {
        return Mathf.Sin((value * Mathf.PI) / 2);
    }
}

public class EaseInOut : Ease {
    public override float Lerp(float value) {
        return -(Mathf.Cos(Mathf.PI * value) - 1) / 2;
    }
}


public class MoveAction : MonoBehaviour
{
    private Vector3 _begin;
    private Vector3 _end;
    private float _time;
    private bool _pingpong;
    private Ease _ease;

    private float _timePassed;
    private bool _started = false;

    public void Setup(Vector3 begin, Vector3 end, float time, bool pingpong, EaseType easeType = EaseType.Linear) {
        _begin = begin;
        _end = end;
        _time = time;
        _timePassed = 0;
        _pingpong = pingpong;
        switch (easeType) {
            case EaseType.EaseIn:
                _ease = new EaseIn();
                break;
            case EaseType.EaseOut:
                _ease = new EaseOut();
                break;
            case EaseType.EaseInOut:
                _ease = new EaseInOut();
                break;
            default:
                _ease = new EaseLinear();
                break;
        }
        _started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_started) return;

        _timePassed += Time.deltaTime;
        _timePassed = Mathf.Min(_time, _timePassed);
        transform.position = _begin + _ease.Lerp(_timePassed / _time) * (_end - _begin);
        if (_timePassed == _time) {
            if (_pingpong) {
                _timePassed -= _time;
                var tmp = _begin;
                _begin = _end;
                _end = tmp;
            }
            else {
                _started = false;
            }
        }
    }
}
