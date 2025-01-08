using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LRS
{
    public class PickUpScript : MonoBehaviour
    {
        public GameObject player;
        public Transform holdPos;
        public float throwForce = 500f; //force at which the object is thrown at
        public float pickUpRange = 5f; //how far the player can pickup the object from
        private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
        private GameObject heldObj; //object which we pick up
        private Rigidbody heldObjRb; //rigidbody of object we pick up
        private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object

        private InputAction pickUpAction;
        private InputAction throwAction;
        private InputAction rotateAction;
        private InputAction rotateXAction;
        private InputAction rotateYAction;

        private void Start()
        {
            pickUpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
            throwAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/q");
            rotateAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/r");
            rotateXAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta/x");
            rotateYAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta/y");

            pickUpAction.Enable();
            throwAction.Enable();
            rotateAction.Enable();
            rotateXAction.Enable();
            rotateYAction.Enable();
        }

        private void Update()
        {
            if (pickUpAction.WasPerformedThisFrame())
            {
                Debug.Log("PickUp");
                OnPickUp();
            }

            if (heldObj != null) //if player is holding object
            {
                MoveObject(); //keep object position at holdPos

                if (rotateAction.IsPressed())
                {
                    RotateObject();
                }

                if (throwAction.WasPerformedThisFrame())
                {
                    Debug.Log("Throw");
                    OnThrow();
                }
            }
        }

        private void OnPickUp()
        {
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    Debug.Log(hit.transform.gameObject.tag);
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping(); //prevents object from clipping through walls
                    DropObject();
                }
            }
        }

        private void OnThrow()
        {
            Debug.Log("Throw");
            Debug.Log(canDrop);
            if (heldObj != null && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
        }

        private void RotateObject()
        {
            canDrop = false; //make sure throwing can't occur during rotating

            float XaxisRotation = rotateXAction.ReadValue<float>() * rotationSensitivity;
            float YaxisRotation = rotateYAction.ReadValue<float>() * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
            canDrop = true; //allow throwing again
        }

        void PickUpObject(GameObject pickUpObj)
        {
            if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
            {
                heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
                heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
                heldObjRb.useGravity = false; // Disable gravity while holding the object
                heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
                                                                //make sure object doesnt collide with player, it can cause weird bugs
                Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            }
        }

        void DropObject()
        {
            //re-enable collision with player
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0; //object assigned back to default layer
            heldObjRb.useGravity = true; // Re-enable gravity
            heldObj.transform.parent = null; //unparent object
            heldObj = null; //undefine game object
        }

        void MoveObject()
        {
            //keep object position the same as the holdPosition position
            heldObj.transform.position = holdPos.transform.position;
        }

        void ThrowObject()
        {
            //same as drop function, but add force to object before undefining it
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.useGravity = true; // Re-enable gravity
            heldObj.transform.parent = null;
            heldObjRb.AddForce(transform.forward * throwForce);
            heldObj = null;
        }

        void StopClipping() //function only called when dropping/throwing
        {
            var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
                                                                                              //have to use RaycastAll as object blocks raycast in center screen
                                                                                              //RaycastAll returns array of all colliders hit within the cliprange
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
            //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
            if (hits.Length > 1)
            {
                //change object position to camera position 
                heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
                                                                                              //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
            }
        }
    }
}
