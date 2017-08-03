using UnityEngine;
using System.Collections;

namespace Sigtrap.ImageEffects {
	public class Tunnelling : MonoBehaviour {
		#region Public Fields
		[Header("Angular Velocity")]
		/// <summary>
		/// Angular velocity calculated for this Transform. DO NOT USE HMD!
		/// </summary>
		[Tooltip("Angular velocity calculated for this Transform.\nDO NOT USE HMD!")]
		public Transform refTransform;

		/// <summary>
		/// Below this angular velocity, effect will not kick in. Degrees per second
		/// </summary>
		[Tooltip("Below this angular velocity, effect will not kick in.\nDegrees per second")]
		public float minAngVel = 0f;

		/// <summary>
		/// At/above this angular velocity, effect will be maxed out. Degrees per second
		/// </summary>
		[Tooltip("At/above this angular velocity, effect will be maxed out.\nDegrees per second")]
		public float maxAngVel = 180f;

		[Header("Effect Settings")]
		/// <summary>
		/// Screen coverage at max angular velocity.
		/// </summary>
		[Range(0f,1f)][Tooltip("Screen coverage at max angular velocity.\n(1-this) is radius of visible area at max effect (screen space).")]
		public float maxEffect = 0.75f;

		/// <summary>
		/// Feather around cut-off as fraction of screen.
		/// </summary>
		[Range(0f, 0.5f)][Tooltip("Feather around cut-off as fraction of screen.")]
		public float feather = 0.1f;

		/// <summary>
		/// Smooth out radius over time. 0 for no smoothing.
		/// </summary>
		[Tooltip("Smooth out radius over time. 0 for no smoothing.")]
		public float smoothTime = 0.15f;
        #endregion

        public float test = 0.0f;
        public float testPosX = 0.0f;
        public float testPosY = 0.0f;
        public float featherTest = 0.4f;

        #region Smoothing
        private float _avSlew;
		private float _av;
		#endregion

		#region Shader property IDs
		private int _propAV;
		private int _propFeather;
        private int _propPosX;
        private int _propPosY;
        #endregion

        #region Misc Fields
        private Vector3 _lastFwd;
		private Material _m;
        #endregion

        bool trigger = false;

		#region Messages
		void Awake ()
        {
			_m = new Material(Shader.Find("Hidden/Tunnelling"));

			if (refTransform == null){
				refTransform = transform;
			}

			_propAV = Shader.PropertyToID("_AV");
			_propFeather = Shader.PropertyToID("_Feather");
            _propPosX = Shader.PropertyToID("_xPos");
            _propPosY = Shader.PropertyToID("_yPos");
        }

		void Update()
        {
            /*Vector3 fwd = refTransform.forward;
			float av = Vector3.Angle(_lastFwd, fwd) / Time.deltaTime;
			av = (av - minAngVel) / (maxAngVel - minAngVel);
			av = Mathf.Clamp01(av);
			av *= maxEffect;

			_av = Mathf.SmoothDamp(_av, av, ref _avSlew, smoothTime);

			_m.SetFloat(_propAV, _av);
			_m.SetFloat(_propFeather, feather);

			_lastFwd = fwd;*/

            _m.SetFloat(_propAV, test);
            _m.SetFloat(_propPosX, testPosX);
            _m.SetFloat(_propPosY, testPosY);
            _m.SetFloat(_propFeather, featherTest);

            if (Input.GetKeyDown(KeyCode.Space))
                trigger = true;

            if(trigger)
            {
                test = Mathf.Lerp(test, 0.8f, 3.0f * Time.deltaTime);
            }
        }

		void OnRenderImage(RenderTexture src, RenderTexture dest){
			Graphics.Blit(src, dest, _m);
		}

		void OnDestroy(){
			Destroy(_m);
		}
		#endregion
	}
}
