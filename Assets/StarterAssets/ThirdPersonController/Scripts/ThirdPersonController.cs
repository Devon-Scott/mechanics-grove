﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using MyUtils.StateMachine;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour, IEnemyObserver
    {
        private List<IPlayerObserver> observers = new List<IPlayerObserver>();
        [Header("Player")]

        public StateStack<ThirdPersonController> stateStack;
        public PlayerBaseState currentState;
        public string StateName;

        public PlayerBuildState BuildState;
        public PlayerCombatState CombatState;
        public PlayerKnockbackState KnockbackState;
        public PlayerGetUpState GetUpState;
        public PlayerHitState HitState;

        public PlayerStats stats;

        public EventManager eventManager;

        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Tooltip("Time required to pass before attacking again")]
        public float AttackTimeout = 0.25f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _attackTimeoutDelta;

        // animation IDs
        public int _animIDSpeed;
        public int _animIDGrounded;
        public int _animIDJump;
        public int _animIDFreeFall;
        public int _animIDMotionSpeed;
        public int _animIDAttack;
        public int _animIDKnockback;
        public int _animIDHit;
        public int _animIDRevived;

        // Item management system
        [HideInInspector]
        public GameObject _currentWeapon;
        [HideInInspector]
        public GameObject _currentOffHand;
        public GameObject leftHandObject;
        public GameObject rightHandObject;
        public GameObject[] weapons;
        public GameObject[] offHands;
        private Hitbox hitbox;
        private ColliderManager _colliderManager;


#if ENABLE_INPUT_SYSTEM 
        public PlayerInput _playerInput;
#endif
        public Animator _animator;
        public CharacterController _controller;
        public StarterAssetsInputs _input;
        public GameObject _mainCamera;

        private const float _threshold = 0.01f;

        public bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            _hasAnimator = TryGetComponent(out _animator);
        }

        private void Start()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            stateStack = new StateStack<ThirdPersonController>(this);
            BuildState = new PlayerBuildState(this);
            CombatState = new PlayerCombatState(this);
            KnockbackState = new PlayerKnockbackState(this);
            GetUpState = new PlayerGetUpState(this);
            HitState = new PlayerHitState(this);

            stateStack.Push(CombatState);
            currentState = (PlayerBaseState)stateStack.CurrentState;

            _colliderManager = GameObject.FindObjectOfType<ColliderManager>();
            AddObserver(_colliderManager);

            eventManager = FindObjectOfType<EventManager>();

            hitbox = _currentWeapon.GetComponentInChildren<Hitbox>();
            if (hitbox == null)
            {
                print("Error in getting hitbox");
            }
            _animator.SetBool("Combat", true);

            OnSpawn();
        }

        private void Update()
        {
            //_hasAnimator = TryGetComponent(out _animator);
            
            stateStack.Update();
            currentState = (PlayerBaseState)stateStack.CurrentState;

        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDKnockback = Animator.StringToHash("Knockback");
            _animIDHit = Animator.StringToHash("Hit");
            _animIDRevived = Animator.StringToHash("Revived");
        }

        public void OnHit(float damage, Vector3 knockback, float scalar)
        {
            currentState.OnHit(damage, knockback, scalar);
        }

        private void ExitKnockback()
        {
            stateStack.ChangeState(CombatState);
        }

        private void Transition()
        {
            currentState.Transition();
        }

        public void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        public void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        public void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        public void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        // Basic attack
        public void Attack()
        {
            if (Grounded && _input.attack)
            {
                _attackTimeoutDelta = AttackTimeout;
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDAttack, true);
                }
            }
            if (_attackTimeoutDelta > 0f)
            {
                _attackTimeoutDelta -= Time.deltaTime;
            }
            else 
            {
                _attackTimeoutDelta = 0;
            }
            
        }

        private void EnableDamage(float damage)
        {
            hitbox.EnableDamage(damage);
            if (_currentWeapon == null)
            {
                print("Error in getting weapon reference");
            }
        }

        private void DisableDamage()
        {
            hitbox.DisableDamage();
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDAttack, false);
            }
        }

        public void TriggerGameOver()
        {
            eventManager.GameOver.RaiseEvent(new LevelStartEvent());
        }

        public void AddObserver(IPlayerObserver observer)
        {
            observers.Add(observer);
        }

        public void OnSpawn()
        {
            observers.ForEach(observer => observer.OnPlayerSpawn(this)); 
        }
        
        public void OnHealth(int health)
        {
            _animator.SetInteger("Health", health);
            observers.ForEach(observer => observer.OnPlayerHealth(health)); 
        }

        public void OnMoney(int money)
        {
            observers.ForEach(observer => observer.OnPlayerMoney(money)); 
            stats.Score += money;
        }

        public void OnDeath()
        {
            eventManager.PlayerDeath.RaiseEvent();
            observers.ForEach(observer => observer.OnPlayerDeath(this)); 
        }

        // Life lost can be triggered by enemies reaching the end of the path
        public void OnLifeLost(int lives)
        {
            observers.ForEach(observer => observer.OnPlayerLifeLost(lives)); 
        }

        public void OnVictory()
        {
            observers.ForEach(observer => observer.OnPlayerVictory(this)); 
        }

        public void OnEnemySpawn(Enemy enemy){}
        public void OnEnemyDeath(Enemy enemy)
        {
            this.stats.Money += enemy.stats.Value;
        }
        public void OnEnemyVictory(Enemy enemy)
        {
            this.stats.Lives -= 1;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}