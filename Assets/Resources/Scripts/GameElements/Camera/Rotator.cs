using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private GameObject ply;
        [SerializeField] private float velocityRotation;
        private Transform player;
        private Quaternion aux;
        private float initialAngle;
        private bool isRotating = false;

        // Use this for initialization
        void Start()
        {
            player = ply.GetComponent<Transform>();
            initialAngle = this.transform.rotation.eulerAngles.y;
        }

        // Update is called once per frame but camera should be the last to move
        void LateUpdate()
        {
            RotateToDirection();
        }

        /// <summary>
        /// Method used to capture if space key was pressed and we should try to rotate the camera y axis
        /// 90f center in the actual position of the player in a increment based of the fixed time (deltaTime) and not allowing to multiple
        /// rotations until the previous one is ended.
        /// </summary>
        private void RotateToDirection() 
        {
            if (Input.GetKeyUp(KeyCode.Space)&& !isRotating) //no rotation happening + space pressed
            {
                /// this.transform.RotateAround(player.position, Vector3.up, 90f);
                aux = this.transform.rotation;              //set the initial value to the previous y-rotation
                isRotating = true;
            }
            else if (isRotating)
            {
                StartCoroutine(Rotate(90f));
            }
        }

        /// <summary>
        /// Coroutine to actually move something X degrees. 
        /// </summary>
        IEnumerator Rotate(float degrees)
        {
            float newAngle = 10f * Time.deltaTime *velocityRotation;        //the new angle
            float desiredAngle = aux.eulerAngles.y + degrees;               //the objective
            float nextAngle = newAngle + transform.rotation.eulerAngles.y;  //the next angle of this object
            if (desiredAngle >= 360f)                                       //if we looped once 
            {
                desiredAngle = desiredAngle - 360f;                         //desired is desired - 360
                if (nextAngle>(initialAngle+360f-degrees))                  // if in this loop would surpass 360º 
                    nextAngle -= 360f;                                      
            }
            if (nextAngle >= desiredAngle)
            {
                newAngle = desiredAngle - transform.rotation.eulerAngles.y;
                isRotating = false;
            }
            this.transform.RotateAround(player.position, Vector3.up, newAngle);
            yield return null;
        }
        
    }

}

