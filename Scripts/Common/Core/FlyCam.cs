using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
using DaftAppleGames.Core.DrawerAttributes;
#endif

namespace DaftAppleGames.Core
{
	[RequireComponent(typeof(Camera))]
	public class FlyCamera : MonoBehaviour
	{
		[BoxGroup("Camera Movement")] public float acceleration = 50;
		[BoxGroup("Camera Movement")] public float lookSensitivity = 1;
		[BoxGroup("Camera Movement")] public float dampingCoefficient = 5;

		[BoxGroup("Debug)")] [SerializeField] private Vector3 _velocity;

		/// <summary>
		/// Check for key presses and update the camera position based on the combined
		/// vector returned
		/// </summary>
		private void Update()
		{
			UpdateInput();

			// Update camera position
			_velocity = Vector3.Lerp(_velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
			transform.position += _velocity * Time.deltaTime;
		}

		/// <summary>
		/// Check for keyboard ad mouse input, combine into a movement vector to apply to the camera
		/// in update.
		/// </summary>
		private void UpdateInput()
		{
			// Position
			_velocity += GetAccelerationVector() * Time.deltaTime;

			// Rotation
			Vector2 mouseDelta = lookSensitivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
			Quaternion rotation = transform.rotation;
			Quaternion horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
			Quaternion vert = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
			transform.rotation = horiz * rotation; // * vert;
		}

		/// <summary>
		/// Returns a motion vector based on what keys are held down. Uses standard
		/// W, A, S, D keys
		/// </summary>
		/// <returns></returns>
		private Vector3 GetAccelerationVector()
		{
			Vector3 moveInput = default;

			if (Input.GetKey(KeyCode.W))
			{
				moveInput += Vector3.forward;
			}

			if (Input.GetKey(KeyCode.S))
			{
				moveInput += Vector3.back;
			}

			if (Input.GetKey(KeyCode.A))
			{
				moveInput += Vector3.left;
			}

			if (Input.GetKey(KeyCode.D))
			{
				moveInput += Vector3.right;
			}

			Vector3 direction = transform.TransformVector(moveInput.normalized);
			return direction * acceleration;
		}
	}
}