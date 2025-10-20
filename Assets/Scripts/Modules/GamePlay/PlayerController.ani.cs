using UnityEngine;

namespace TTGJ.GamePlay
{
    public partial class PlayerController
    {
        private Animator _animator;
        private bool _isMove = false;
        private bool _isLift = false;
        private bool IsMove
        {
            get { return _isMove; }
            set
            {
                _isMove = value;
                _animator.SetBool("IsMove", _isMove);
            }
        }
        private bool IsLift
        {
            get { return _isLift; }
            set
            {
                _isLift = value;
                _animator.SetBool("IsLift", _isLift);
            }
        }
        private void PlayEatAnimation()
        {
            _animator.SetTrigger("Eat");
        }
        private void PlayLiftAnimation()
        {
            _animator.SetTrigger("Lift");
        }
    }
}