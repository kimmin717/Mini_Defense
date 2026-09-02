using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCamera : MonoBehaviour
{
    #region 인스펙터
    [Header("필수 연결")]
    [SerializeField] private Camera _camera;

    [Header("카메라 속도")]
    [SerializeField] private float _cameraSpeed = 30f;

    [Header("스크롤 속도")]
    [SerializeField] private float _scrollSpeed = 5f;

    [Header("스크롤 범위")]
    [SerializeField] private float _minY = 10f;
    [SerializeField] private float _maxY = 80f;
    #endregion

    #region 내부변수
    private Transform _camTr;
    private float _cameraBorderThickness = 10f;
    private bool _camerMovemenet = true;
    #endregion

    void Update()
    {
        if (_camera == null)
        {
            GameObject mainCameraGo = GameObject.FindGameObjectWithTag("MainCamera");

            if (mainCameraGo != null )
            {
                _camera = mainCameraGo.GetComponent<Camera>();
            }
        }

        _camTr = _camera.transform;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _camerMovemenet = !_camerMovemenet;
        }

        if(!_camerMovemenet)
        {
            return;
        }

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - _cameraBorderThickness)
        {
            _camTr.Translate(Vector3.forward * _cameraSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= _cameraBorderThickness)
        {
            _camTr.Translate(Vector3.back * _cameraSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - _cameraBorderThickness)
        {
            _camTr.Translate(Vector3.right * _cameraSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= _cameraBorderThickness)
        {
            _camTr.Translate(Vector3.left * _cameraSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = _camTr.position;

        // 스클롤 값이 작기 때문에 1000을 곱해 값을 늘려준다.
        pos.y -= scroll * 1000 * _scrollSpeed * Time.deltaTime;
        // 스크롤 했을때 땅을 뚥거나 너무 높게 올라가지 못하게 범위 지정
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

        _camTr.position = pos;
    }
}
