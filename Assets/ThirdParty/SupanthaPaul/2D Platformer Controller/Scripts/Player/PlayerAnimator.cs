using Unity.Netcode;
using UnityEngine;

namespace SupanthaPaul
{
	public class PlayerAnimator : NetworkBehaviour
	{
		private Rigidbody2D m_rb;
		private PlayerController m_controller;
		private Animator m_anim;
		private static readonly int Move = Animator.StringToHash("Move");
		private static readonly int JumpState = Animator.StringToHash("JumpState");
		private static readonly int IsJumping = Animator.StringToHash("IsJumping");
		private static readonly int WallGrabbing = Animator.StringToHash("WallGrabbing");
		private static readonly int IsDashing = Animator.StringToHash("IsDashing");

		private void Start()
		{
			if (!IsOwner) return;

			m_anim = GetComponentInChildren<Animator>();
			m_controller = GetComponent<PlayerController>();
			m_rb = GetComponent<Rigidbody2D>();
		}

		private Vector2 currentSpeed;

		private void Update()
		{
			if (!IsOwner) return;

			Vector2 newSpeed = m_rb.velocity;

            if (HasMoveStateChanged(currentSpeed, newSpeed))
            {
				currentSpeed = newSpeed;
				UpdateAnimationRPC(currentSpeed);
            }

			// Jump animation
			if (!m_controller.isGrounded && !m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(IsJumping, true);
			}
			else
			{
				m_anim.SetBool(IsJumping, false);
			}

			if(!m_controller.isGrounded && m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(WallGrabbing, true);
			} else
			{
				m_anim.SetBool(WallGrabbing, false);
			}

			// dash animation
			m_anim.SetBool(IsDashing, m_controller.isDashing);
        }

		[Rpc(SendTo.Everyone)]
		private void UpdateAnimationRPC(Vector2 speed)
        {
			if (m_anim == null) return; 

            // Idle & Running animation
            m_anim.SetFloat(Move, Mathf.Abs(speed.x));

            // Jump state (handles transitions to falling/jumping)
            float verticalVelocity = speed.y;
            m_anim.SetFloat(JumpState, verticalVelocity);
        }


		private bool HasMoveStateChanged(Vector2 oldSpeed, Vector2 newSpeed)
		{
			return Mathf.Abs(oldSpeed.x - newSpeed.x) > 0.1f || Mathf.Abs(oldSpeed.y - newSpeed.y) > 0.1f;
		}

	}
}
