using UnityEngine;
using BytesCrafter.USocketNet;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
		public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

		public float turretSpeed = 7f;
		public Transform turret = null;

        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

		public USocketView view = null;

        private void Awake ()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }

        private void OnEnable ()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play();
            }
        }

        private void OnDisable ()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop();
            }
        }
			
        private void Start ()
        {
            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;
        }


        private void Update ()
        {
			if(!view.IsLocalUser)
				return;

			// If there is no input (the tank is stationary)...
			if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.1f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.1f)
			{
				// ... and if the audio source is currently playing the driving clip...
				if (m_MovementAudio.clip == m_EngineDriving)
				{
					// ... change the clip to idling and play it.
					m_MovementAudio.clip = m_EngineIdling;
					m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
					m_MovementAudio.Play ();
				}
			}
			else
			{
				// Otherwise if the tank is moving and if the idling clip is currently playing...
				if (m_MovementAudio.clip == m_EngineIdling)
				{
					// ... change the clip to driving and play.
					m_MovementAudio.clip = m_EngineDriving;
					m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
					m_MovementAudio.Play();
				}
			}
        }

        private void FixedUpdate ()
        {
			if(!view.IsLocalUser)
				return;

			if(!Application.isFocused)
				return;

			// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
			Vector3 movement = transform.forward * Input.GetAxis ("Vertical") * m_Speed * Time.deltaTime;

			// Apply this movement to the rigidbody's position.
			m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

			// Determine the number of degrees to be turned based on the input, speed and time between frames.
			float turn = Input.GetAxis ("Horizontal") * m_TurnSpeed * Time.deltaTime;

			// Make this into a rotation in the y axis.
			Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

			// Apply this rotation to the rigidbody's rotation.
			m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);

			//Rotate the turret to the current mouse pointer location.
			Ray rays = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit = new RaycastHit ();

			if(Physics.Raycast(rays, out hit))
			{
				Debug.DrawLine (rays.origin, hit.point, Color.red);

				Vector3 target = hit.point - turret.position;
				Quaternion angle = Quaternion.LookRotation (target, turret.up);
				turret.transform.rotation = Quaternion.Lerp (turret.rotation, angle, turretSpeed * Time.deltaTime);
				turret.transform.rotation = Quaternion.Euler (0f, turret.transform.rotation.eulerAngles.y, 0f);
			}
        }
    }
}