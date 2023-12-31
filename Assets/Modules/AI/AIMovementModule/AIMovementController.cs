using System;
using System.Collections;
using UnityEngine;

namespace SDRGames.Cameraman.AI.Movement
{
    public class AIMovementController : MonoBehaviour
    {
        [SerializeField] private float _maxMovementSpeed = 12f;
        [SerializeField] private float _movementSpeedStep = 2f;
        [SerializeField] private float _movementSpeedIncreasePeriod = 1f;
        [SerializeField] private float _minDirectionChangeDelay = 0.5f;
        [SerializeField] private float _maxDirectionChangeDelay = 1.5f;
        [SerializeField] private LayerMask _finishLayerMask;
        [SerializeField] private LayerMask _tubeLayerMask;
        private float _movementSpeed;
        private Rigidbody2D _rigidbody;
        private BoxCollider2D _collider;
        [SerializeField] private bool _isRun = false;

        public event EventHandler<GameOverEventArgs> Finished;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _movementSpeed = _movementSpeedStep;

            if(_isRun)
            {
                StartCoroutine(IncreaseSpeedOverTime());
            }
        }

        private IEnumerator IncreaseSpeedOverTime()
        {
            while (Math.Abs(_movementSpeed) < _maxMovementSpeed)
            {
                yield return new WaitForSeconds(_movementSpeedIncreasePeriod);
                if (_movementSpeed >= 0)
                {
                    _movementSpeed += _movementSpeedStep;
                }
                else
                {
                    _movementSpeed -= _movementSpeedStep;
                }
            }

        }

        void Update()
        {
            if (_isRun)
            {
                _rigidbody.velocity = new Vector2(-_movementSpeed, _rigidbody.velocity.y);
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsFinished())
            {
                _movementSpeed = _movementSpeedStep * 2;
                Finished?.Invoke(this, new GameOverEventArgs(true));
                Director.IsFinished = true;
                StopAllCoroutines();
            }
        }

        public void Run(object sender, EventArgs e)
        {
            _isRun = true;
            _movementSpeed = _movementSpeedStep;
            StartCoroutine(IncreaseSpeedOverTime());
            //StartCoroutine(ChangeDirectionOverTime());
        }

        //private IEnumerator ChangeDirectionOverTime()
        //{
        //    while (true)
        //    {
        //        float directionChangeDelay = UnityEngine.Random.Range(_minDirectionChangeDelay, _maxDirectionChangeDelay);
        //        yield return new WaitForSeconds(directionChangeDelay);
        //        if (UnityEngine.Random.Range(0, 100) > 60)
        //        {
        //            ChangeDirection();
        //        }
        //    }
        //}

        //private void ChangeDirection()
        //{
        //    _movementSpeed = -_movementSpeed;
        //}

        private bool IsFinished()
        {
            RaycastHit2D raycastHit2D = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, -transform.right, 0.15f, _finishLayerMask);
            return raycastHit2D.collider != null;
        }
    }
}