using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairRotator : MonoBehaviour {

    private Vector3 _mousePosition;
    private float _screenWidthFactor, _screenHeightFactor;

    void Start()
    {
        _screenWidthFactor = Screen.width / 2;
        _screenHeightFactor = Screen.height / 2;
    }
	
	// Update is called once per frame
	void Update () {
        //It should be the angle inverted to follow correctly the movements of the mouse crosshair
        transform.rotation = Quaternion.Euler(0f, ObtainAngleFromMouse(), 0f);
    }


    public float ObtainAngleFromMouse()
    {
        float _angle;
        _mousePosition = Input.mousePosition;
        //Esta posicion va de (0,0) a (anchura, altura) ergo hay que cambiarlo para que el hayan
        //distancias relativas o sino calcular el angulo es imposible
        _mousePosition.x = _mousePosition.x - _screenWidthFactor;
        _mousePosition.y = _mousePosition.y - _screenHeightFactor;
        //Calculamos el angulo desde ese punto al 0-0 (el jugador) por eso no hay que restar nada
        _angle = Mathf.Atan2(_mousePosition.x, _mousePosition.y) * Mathf.Rad2Deg;
        //Si es 0 = 360, si es negativo es 360 - angulo
        if (_angle <= 0)
            _angle = Mathf.Abs(360 + _angle);
        return _angle;
    }
}
