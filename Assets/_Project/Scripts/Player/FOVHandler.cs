using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

// Se encarga de modificar el FOV en base a la velocidad del jugador

public class FOVHandler : MonoBehaviour
{
    [SerializeField] private float _fovSmoothness = 0.8f;
    [SerializeField] private float _fovIntensity = 2f;

    private CharacterController _cc;
    private PlayerCharacterController _plrController;
    private Camera _cam;

    private float _baseFOV = 60f;
    private float _baseSpeed;
    private float _targetFOV;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _plrController = GetComponent<PlayerCharacterController>();
        _cam = GetComponentInChildren<Camera>();

        _targetFOV = _baseFOV;
    }

    private void Update()
    {
        float maxSpeed = _plrController.MaxSpeedOnGround;
        Vector3 groundVel = _plrController.CharacterVelocity;
        groundVel.y = 0f;
        float currentSpeed = Mathf.Clamp(groundVel.magnitude, maxSpeed, 1000f);
        _targetFOV = _baseFOV + (maxSpeed - currentSpeed) * _fovIntensity;
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetFOV, _fovSmoothness * Time.deltaTime);
    }
}