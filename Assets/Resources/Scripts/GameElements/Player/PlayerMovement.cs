using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class PlayerMovement : MonoBehaviour
    {

        [SerializeField] private float _movementSpeed = 10;
        private Rigidbody _rigidBody;

        public float MovementSpeed {
            get { return _movementSpeed; }
            set { _movementSpeed = value; }
        }

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal") * _movementSpeed * Time.deltaTime;
            /*float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
            transform.Rotate(0, horizontal, 0);*/

            float vertical = Input.GetAxis("Vertical") * _movementSpeed * Time.deltaTime;
            //our vertical w-s should be going towards and forward
            //because we will move right-to left based on the camera we should swap vertical to it's negative
            transform.Translate(horizontal, 0f, vertical);
        }

        private void OnCollisionEnter(Collision collision)
        {
            _rigidBody.velocity = new Vector3(0f, 0f, 0f);
        }


    }
}

