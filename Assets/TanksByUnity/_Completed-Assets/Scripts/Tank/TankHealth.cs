using UnityEngine;
using UnityEngine.UI;
using BytesCrafter.USocketNet;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        
        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
		public float m_CurrentHealth = 100f;                      // How much health the tank currently has.
        private bool m_Dead = false;                                // Has the tank been reduced beyond zero health yet?

		public USocketView view = null;
		public TankMovement ctrl = null;
		public TankShooting shoot = null;

        void Awake ()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            // Get a reference to the audio source on the instantiated prefab.
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

            // Disable the prefab so it can be activated when it's required.
            m_ExplosionParticles.gameObject.SetActive (false);

			//Add listeners for damage!
			view.ListenTriggers(ReceivedTriggers);

        }

		void LateUpdate()
		{
			if(view.IsLocalUser)
			{
				//view.states.syncValue [0] = m_CurrentHealth + "";

				if(m_Dead)
				{
					if(Input.GetKeyDown(KeyCode.Insert))
					{
						// When the tank is enabled, reset the tank's health and whether or not it's dead.
						m_CurrentHealth = m_StartingHealth;

						// Set the slider's value appropriately.
						m_Slider.value = m_CurrentHealth;

						// Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
						m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

						m_Dead = false;
						ctrl.enabled = true;
						shoot.enabled = true;

						foreach(Transform trans in transform)
						{
							trans.gameObject.SetActive (true);
						}
					}
				}
			}

			else
			{
				//m_CurrentHealth = System.Convert.ToSingle(view.states.syncValue [0]);

				// Set the slider's value appropriately.
				//m_Slider.value = m_CurrentHealth;

				// Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
				//m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
			}
		}

		private void ReceivedTriggers(TriggerJson triggerJson)
		{
			if(triggerJson.tKy == "Damage")
			{
				float damage = System.Convert.ToSingle(triggerJson.tVl);

				OnDamage (damage);
			}

			if(triggerJson.tKy == "Death")
			{
				OnDeath ();
			}

			Debug.Log (triggerJson.tKy + ": " + triggerJson.tVl);
		}

		public void TakeDamage (float amount)
        {
			if(view.IsLocalUser)
			{
				view.TriggerEvents ("Damage", amount + "", (Returned returned) => {
					if(returned == Returned.Success)
					{
						OnDamage(amount);
					}
				});
			}
        }

		void OnDamage(float amount)
		{
			// Reduce current health by the amount of damage done.
			m_CurrentHealth -= amount;

			// Set the slider's value appropriately.
			m_Slider.value = m_CurrentHealth;

			// Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
			m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);


			if(m_CurrentHealth <= 0)
			{
				if(view.IsLocalUser)
				{
					view.TriggerEvents ("Death", "", (Returned returned) => {
						if(returned == Returned.Success)
						{
							OnDeath();
						}
					});
				}
			}

			Debug.LogError("Sent Triggers!");
		}

        private void OnDeath ()
        {
            // Set the flag so that this function is only called once.
            m_Dead = true;

            // Move the instantiated explosion prefab to the tank's position and turn it on.
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            // Play the particle system of the tank exploding.
            m_ExplosionParticles.Play ();

            // Play the tank explosion sound effect.
            m_ExplosionAudio.Play();

            // Turn the tank off.
			ctrl.enabled = false;
			shoot.enabled = false;
			foreach(Transform trans in transform)
			{
				trans.gameObject.SetActive (false);
			}
        }
    }
}