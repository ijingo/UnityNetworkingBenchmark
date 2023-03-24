using System.Collections.Generic;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Random = System.Random;

namespace Benchmark.Fishnet
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public enum GroundState : byte { Jumping, Falling, Grounded }
        
        private static int range = 30;

        public GameObject chasingTarget;
        public bool hasTarget;
        
        [Header("Avatar Components")]
        public CharacterController characterController;

        [Header("Movement")]
        [Range(1, 20)]
        public float moveSpeedMultiplier = 8f;
        [Range(1, 20)]
        public float autoMoveSpeedMultiplier = 2f;

        [Header("Turning")]
        [Range(1f, 200f)]
        public float maxTurnSpeed = 100f;
        [Range(.5f, 5f)]
        public float turnDelta = 3f;

        [Header("Jumping")]
        [Range(0.1f, 1f)]
        public float initialJumpSpeed = 0.2f;
        [Range(1f, 10f)]
        public float maxJumpSpeed = 5f;
        [Range(0.1f, 1f)]
        public float jumpDelta = 0.2f;

        [Header("Diagnostics - Do Not Modify")]
        public GroundState groundState = GroundState.Grounded;

        [Range(-1f, 1f)]
        public float horizontal;
        [Range(-1f, 1f)]
        public float vertical;

        [Range(-200f, 200f)]
        public float turnSpeed;

        [Range(-10f, 10f)]
        public float jumpSpeed;

        [Range(-1.5f, 1.5f)]
        public float animVelocity;

        [Range(-1.5f, 1.5f)]
        public float animRotation;

        public Vector3Int velocity;
        public Vector3 direction;

        protected override void OnValidate()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
        
            // Override CharacterController default values
            characterController.enabled = false;
            characterController.skinWidth = 0.02f;
            characterController.minMoveDistance = 0f;
        
            GetComponent<Rigidbody>().isKinematic = true;
        
            this.enabled = false;
        }
        
        public void ClearTarget()
        {
            hasTarget = false;
            chasingTarget = null;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-range + 1, range), 1, UnityEngine.Random.Range(-range + 1, range));
            transform.position = spawnPosition;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                characterController.enabled = true;
                this.enabled = true;
            }
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (IsOwner)
            {
                characterController.enabled = false;
                this.enabled = false;
            }
        }
        
        [Client]
        void findChasingTarget()
        {
            GameObject[] prizes = GameObject.FindGameObjectsWithTag("Prize");
            GameObject[] Icospheres = GameObject.FindGameObjectsWithTag("Icosphere");

            List<GameObject> list = new List<GameObject>();
            list.AddRange(prizes);
            list.AddRange(Icospheres);

            var random = new Random();
            int index = random.Next(list.Count);

            chasingTarget = list[index];
            hasTarget = true;

            if (chasingTarget.CompareTag("Prize"))
            {
                Reward reward = chasingTarget.GetComponent<Reward>();
                reward.SetChasingPlayer(this);
            }
        }
        
        void moveTowardsTheTarget()
        {
            // // rotate to the target.
            transform.LookAt(chasingTarget.transform);
            
            // move to the target
            direction = chasingTarget.transform.position - transform.position;
            
            var rotateDirection = Vector3.ClampMagnitude(direction, 1f);
            transform.Rotate(rotateDirection);
            
            // // Clamp so diagonal strafing isn't a speed advantage.
            direction = Vector3.ClampMagnitude(direction, 1f);
            // // Transforms direction from local space to world space.
            // direction = transform.TransformDirection(direction);
            // Multiply for desired ground speed.
            direction *= autoMoveSpeedMultiplier;
            // Finally move the character.
            characterController.Move(direction * Time.deltaTime);
        }

        [TargetRpc]
        public void TargetClearTarget(NetworkConnection target)
        {
            hasTarget = false;
            chasingTarget = null;
        }
        
        void Update()
        {
            if (!base.IsOwner)
                return;
            
            if (!characterController.enabled)
                return;
            
            if (Configuration.autoControl)
            {
                if (!hasTarget)
                {
                    findChasingTarget();
                }
                else
                {
                    moveTowardsTheTarget();
                }
            }

            HandleTurning();
            HandleJumping();
            HandleMove();

            // Reset ground state
            if (characterController.isGrounded)
                groundState = GroundState.Grounded;
            else if (groundState != GroundState.Jumping)
                groundState = GroundState.Falling;

            // Diagnostic velocity...FloorToInt for display purposes
            velocity = Vector3Int.FloorToInt(characterController.velocity);
        }

        // TODO: Turning works while airborne...feature?
        void HandleTurning()
        {
            // Q and E cancel each other out, reducing the turn to zero.
            if (Input.GetKey(KeyCode.Q))
                turnSpeed = Mathf.MoveTowards(turnSpeed, -maxTurnSpeed, turnDelta);
            if (Input.GetKey(KeyCode.E))
                turnSpeed = Mathf.MoveTowards(turnSpeed, maxTurnSpeed, turnDelta);

            // If both pressed, reduce turning speed toward zero.
            if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
                turnSpeed = Mathf.MoveTowards(turnSpeed, 0, turnDelta);

            // If neither pressed, reduce turning speed toward zero.
            if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
                turnSpeed = Mathf.MoveTowards(turnSpeed, 0, turnDelta);

            transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
        }

        void HandleJumping()
        {
            // Handle variable force jumping.
            // Jump starts with initial power on takeoff, and jumps higher / longer
            // as player holds spacebar. Jump power is increased by a diminishing amout
            // every frame until it reaches maxJumpSpeed, or player releases the spacebar,
            // and then changes to the falling state until it gets grounded.
            if (groundState != GroundState.Falling && Input.GetKey(KeyCode.Space))
            {
                if (groundState != GroundState.Jumping)
                {
                    // Start jump at initial power.
                    groundState = GroundState.Jumping;
                    jumpSpeed = initialJumpSpeed;
                }
                else
                    // Jumping has already started...increase power toward maxJumpSpeed over time.
                    jumpSpeed = Mathf.MoveTowards(jumpSpeed, maxJumpSpeed, jumpDelta);

                // If power has reached maxJumpSpeed, change to falling until grounded.
                // This prevents over-applying jump power while already in the air.
                if (jumpSpeed == maxJumpSpeed)
                    groundState = GroundState.Falling;
            }
            else if (groundState != GroundState.Grounded)
            {
                // handles running off a cliff and/or player released Spacebar.
                groundState = GroundState.Falling;
                jumpSpeed = Mathf.Min(jumpSpeed, maxJumpSpeed);
                jumpSpeed += Physics.gravity.y * Time.deltaTime;
            }
            else
                jumpSpeed = Physics.gravity.y * Time.deltaTime;
        }

        // TODO: Directional input works while airborne...feature?
        void HandleMove()
        {
            // Capture inputs
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            // Create initial direction vector without jumpSpeed (y-axis).
            direction = new Vector3(horizontal, 0f, vertical);

            // Clamp so diagonal strafing isn't a speed advantage.
            direction = Vector3.ClampMagnitude(direction, 1f);

            // Transforms direction from local space to world space.
            direction = transform.TransformDirection(direction);

            // Multiply for desired ground speed.
            direction *= moveSpeedMultiplier;

            // Add jumpSpeed to direction as last step.
            direction.y = jumpSpeed;

            // Finally move the character.
            characterController.Move(direction * Time.deltaTime);
        }
        
        public void OnDestroy()
        {
            if (hasTarget && IsOwner)
            {
                if (chasingTarget)
                {
                    if (chasingTarget.CompareTag("Prize"))
                    {
                        Reward reward = chasingTarget.GetComponent<Reward>();
                        reward.ClearChasingPlayer();
                    }
                }
            }
        }
    }
}