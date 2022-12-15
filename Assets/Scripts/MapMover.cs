using System;
using System.Collections;
using UnityEngine;

namespace Strid {
    public class MapMover : MonoBehaviour {
        private enum StartingPoint { TopLeft, TopRight, BottomLeft, BottomRight }
        private enum Movement { ClockwiseCircle, CounterClockwiseCircle, ClockwiseInfinity, CounterClockwiseInfinity }
        
        [SerializeField] private Vector3 topLeft;
        [SerializeField] private Vector3 bottomRight;
        [SerializeField] private float speed;
        
        [Space]
        
        [SerializeField] private StartingPoint startingPoint;
        [SerializeField] private Movement movement;

        private Vector3 _topRight, _bottomLeft;

        private Vector3 Position {
            set => transform.position = value;
            get => transform.position;
        }

        private void Start() {
            _topRight = new Vector3(bottomRight.x, topLeft.y);
            _bottomLeft = new Vector3(topLeft.x, bottomRight.y);
            
            Position = startingPoint switch {
                StartingPoint.TopLeft => topLeft,
                StartingPoint.TopRight => _topRight,
                StartingPoint.BottomLeft => _bottomLeft,
                StartingPoint.BottomRight => bottomRight,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void Update() {
            if (Position == topLeft) {
                var move = movement switch {
                    Movement.ClockwiseCircle => Move(topLeft, _topRight, PredicateLtR),
                    Movement.CounterClockwiseCircle => Move(topLeft, _bottomLeft, PredicateTtB),
                    Movement.ClockwiseInfinity => Move(topLeft, bottomRight, PredicateTLtBR),
                    Movement.CounterClockwiseInfinity => Move(topLeft, _bottomLeft, PredicateTtB),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                StartCoroutine(move);
            } else if (Position == _topRight) {
                var move = movement switch {
                    Movement.ClockwiseCircle => Move(_topRight, bottomRight, PredicateTtB),
                    Movement.CounterClockwiseCircle => Move(_topRight, topLeft, PredicateRtL),
                    Movement.ClockwiseInfinity => Move(_topRight, _bottomLeft, PredicateTRtBL),
                    Movement.CounterClockwiseInfinity => Move(_topRight, bottomRight, PredicateTtB),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                StartCoroutine(move);
            } else if (Position == bottomRight) {
                var move = movement switch {
                    Movement.ClockwiseCircle => Move(bottomRight, _bottomLeft, PredicateRtL),
                    Movement.CounterClockwiseCircle => Move(bottomRight, _topRight, PredicateBtT),
                    Movement.ClockwiseInfinity => Move(bottomRight, _topRight, PredicateBtT),
                    Movement.CounterClockwiseInfinity => Move(bottomRight, topLeft, PredicateBRtTL),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                StartCoroutine(move);
            } else if (Position == _bottomLeft) {
                var move = movement switch {
                    Movement.ClockwiseCircle => Move(_bottomLeft, topLeft, PredicateBtT),
                    Movement.CounterClockwiseCircle => Move(_bottomLeft, bottomRight, PredicateLtR),
                    Movement.ClockwiseInfinity => Move(_bottomLeft, topLeft, PredicateBtT),
                    Movement.CounterClockwiseInfinity => Move(_bottomLeft, _topRight, PredicateBLtTR),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                StartCoroutine(move);
            }
        }

        private IEnumerator Move(Vector3 start, Vector3 finish, Func<Vector3, bool> predicate) {
            var timeElapsed = 0f;
            
            while (predicate(finish)) {
                Position = Vector3.Lerp(start, finish, speed * timeElapsed);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Position = finish;
        }

        private bool PredicateLtR(Vector3 destination) { return Position.x > destination.x; }
        private bool PredicateRtL(Vector3 destination) { return Position.x < destination.x; }

        private bool PredicateTtB(Vector3 destination) { return Position.y < destination.y; }
        private bool PredicateBtT(Vector3 destination) { return Position.y > destination.y; }

        private bool PredicateTLtBR(Vector3 destination) { return PredicateLtR(destination) && PredicateTtB(destination); }
        private bool PredicateTRtBL(Vector3 destination) { return PredicateRtL(destination) && PredicateTtB(destination); }
        private bool PredicateBLtTR(Vector3 destination) { return PredicateLtR(destination) && PredicateBtT(destination); }
        private bool PredicateBRtTL(Vector3 destination) { return PredicateRtL(destination) && PredicateBtT(destination); }
    }
}