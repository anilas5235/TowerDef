using System;
using UnityEngine;

namespace Background.SplinePath
{
    public class PointBehaviour : MonoBehaviour
    {
        public Action PointMoved;

        private bool currentlyDraged;
        private Vector3 oldPosition;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            oldPosition = transform.position;
        }

        private void OnMouseDown()
        {
            currentlyDraged = !currentlyDraged;
        }

        private void Update()
        {
            if (currentlyDraged)
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                transform.position = mousePosition;
                if (Vector3.Distance(transform.position, oldPosition) > 0.05f)
                {
                    PointMoved?.Invoke();
                    oldPosition = transform.position;
                }
            }
        }
    }
}