using UnityEngine;
using UnityEngine.UI;
using BytesCrafter.USocketNet;
using Complete;

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
		public bool m_Dead = false;                                // Has the tank been reduced beyond zero health yet?

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
				view.states.syncValue [0] = m_CurrentHealth + "";
			}

			else
			{
				m_CurrentHealth = System.Convert.ToSingle(view.states.syncValue [0]);
			}

			m_Slider.value = m_CurrentHealth;
			m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
		}

		public void Revived()
		{
			// When the tank is enabled, reset the tank's health and whether or not it's dead.
			m_CurrentHealth = m_StartingHealth;

			m_Dead = false;
			ctrl.enabled = true;
			shoot.enabled = true;
			rigidB.isKinematic = false;
			coolide.enabled = true;
			foreach(Transform trans in transform)
			{
				trans.gameObject.SetActive (true);
			}
		}

		private void ReceivedTriggers(TriggerJson triggerJson)
		{
			if(triggerJson.tKy == "Damage")
			{
				float damage = System.Convert.ToSingle(triggerJson.tVl);

				OnDamage (damage, true);
			}

			if(triggerJson.tKy == "Death")
			{
				OnDeath ();
			}

			if(triggerJson.tKy == "Revived")
			{
				Revived ();
			}
		}

		public void TakeDamage (float amount)
        {
			//Debug.Log ("ON PEER DAMAGE2!");

			if(!view.IsLocalUser)
			{
				//Debug.Log ("ON PEER DAMAGE DETECTED!");

				view.TriggerEvents ("Damage", amount + "", (Returned returned) => {
					if(returned == Returned.Success)
					{
						//Debug.Log ("ON PEER DAMAGE SENT!");
						OnDamage(amount, false);
					}
				});
			}
        }

		void OnDamage(float amount, bool server)
		{
			//Debug.Log ("ON PEER DAMAGE RECEIVED! from server: " + server);

			// Reduce current health by the amount of damage done.
			m_CurrentHealth -= amount;

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
		}

		public Rigidbody rigidB = null;
		public Collider coolide = null;

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
			rigidB.isKinematic = true;
			coolide.enabled = false;
			foreach(Transform trans in transform)
			{
				trans.gameObject.SetActive (false);
			}
        }
    }
