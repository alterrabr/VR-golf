namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using DG.Tweening;

    public class Club : VRTK_InteractableObject
    {
        [Header("Physics Settings")]

        [SerializeField]
        [Tooltip("Object, which placed on the end of club and need for correct physics punch work")]
        private ClubPunchBoxFollower clubFollowerPrefab = null;

        [SerializeField]
        [Tooltip("Object, to which follower will be added to.")]
        private GameObject clubPunchBox = null;

        [SerializeField]
        [Tooltip("List of colliders, attached to club")]
        private List<BoxCollider> clubColliders = null;

        [Header("Tilt/Shift Adjustment Settings")]

        [SerializeField]
        [Tooltip("Point on club, where initial grab performs.")]
        private GameObject primaryGrabPoint = null;

        //[SerializeField]
        [Tooltip("Point on club, where secondary controller's model snaps at.")]
        public GameObject secondaryGrabPoint = null;

        [SerializeField]
        [Tooltip("Maximum distance of club's shift adjustment.")]
        private float maxClubShift = .2f;

        [SerializeField]
        [Tooltip("Maximum angle of club's tilt adjustment.")]
        private float maxClubTilt = 15f;

        [SerializeField]
        [Tooltip("Joystick input threshold for shift.")]
        private float clubShiftTreshold = .6f;

        [SerializeField]
        [Tooltip("Joystick input threshold for tilt.")]
        private float clubTiltTreshold = .8f;

        // Variable for follower instance.
        private ClubPunchBoxFollower follower;

        // Get controller events
        private VRTK_ControllerEvents grabbingObjectControllerEvents;

        // Shift position for maximum shift check and offsetting grab/snap points
        private static float currentShift = 0f;

        // Tilt angle for maximum tilt check and offsetting grab points
        private static float currentTilt = 0f;

        // Check if club dropped and destroy coroutine is running
        private bool clubDestroyEnabled = false;

        // If this is true, instance of club was not ever been grabbed
        private bool initiallyGrabbed = true;

        // Club material for changing rendering mode
        private Material clubShaderMaterial;

        // Joint component, applied to club when it grabbed 
        private FixedJoint clubFixedJoint;

        private void Start()
        {
            clubShaderMaterial = GetComponentInChildren<MeshRenderer>().material;
        }

        public override void Grabbed(VRTK_InteractGrab grabbingObject)
        {
            if (clubDestroyEnabled || IsGrabbed())
            {
                StopDestroyClubOnPickUp();
            }

            // Offset grab and snap points local positions / rotation before initial grab performs
            if (!IsGrabbed() && initiallyGrabbed)
            {
                primaryGrabPoint.transform.localPosition -= new Vector3(0, 0, currentShift);
                secondaryGrabPoint.transform.localPosition -= new Vector3(0, 0, currentShift);

                primaryGrabPoint.transform.localEulerAngles -= new Vector3(currentTilt, 0, 0);
            }

            // Perform grab
            base.Grabbed(grabbingObject);

            // Get current grabbing controller's events and store connected joint
            if (IsGrabbed())
            {
                grabbingObjectControllerEvents = GetGrabbingObject().GetComponent<VRTK_ControllerEvents>();
                clubFixedJoint = gameObject.GetComponent<FixedJoint>();
            }

            SpawnClubFollower();

            if (IsGrabbed())
            {
                ToggleCollidersTriggersState(true);
            }
        }

        // On ungrab destroy follower and turn club colliders on
        // Start club fade/destroy coroutine
        public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
        {
            base.Ungrabbed(previousGrabbingObject);

            if (follower != null && !IsGrabbed())
            {
                Destroy(follower.gameObject);
                ToggleCollidersTriggersState(false);
            }

            if (!clubDestroyEnabled && !IsGrabbed())
            {
                StartCoroutine(DestroyClubOnDrop());
            }

            initiallyGrabbed = false;
        }

        protected override void Update()
        {
            base.Update();

            if (!clubFixedJoint)
            {
                clubFixedJoint = gameObject.GetComponent<FixedJoint>();
            }

            // If grab initialized and can access to it's events (usually takes 2 frames after grab)
            if (grabbingObjectControllerEvents && clubFixedJoint)
            {
                // If current shift inside of possible range - perform shift
                if (grabbingObjectControllerEvents.GetTouchpadAxis().y > clubShiftTreshold && currentShift <= 0)
                {
                    ShiftGrabPoint();
                }
                else if (grabbingObjectControllerEvents.GetTouchpadAxis().y < -clubShiftTreshold && currentShift >= -maxClubShift)
                {
                    ShiftGrabPoint();
                }

                // If current tilt inside of possible range - perform tilt
                if (grabbingObjectControllerEvents.GetTouchpadAxis().x > clubTiltTreshold && currentTilt <= maxClubTilt)
                {
                    TiltGrabPoint();
                }
                else if (grabbingObjectControllerEvents.GetTouchpadAxis().x < -clubTiltTreshold && currentTilt >= -maxClubTilt)
                {
                    TiltGrabPoint();
                }
            }
        }

        /// <summary>
        /// Shift club on y-axis touchpad input.
        /// </summary>
        private void ShiftGrabPoint()
        {
            // Store last connected body
            Rigidbody connectedBody = clubFixedJoint.connectedBody;

            // Remove connection
            clubFixedJoint.connectedBody = null;

            // Store current shift to check shift distance and later add it to new clubs 
            currentShift += grabbingObjectControllerEvents.GetTouchpadAxis().y * Time.deltaTime;

            // Move club accordingly to joystick Y-axis input
            transform.position += transform.forward * grabbingObjectControllerEvents.GetTouchpadAxis().y * Time.deltaTime;

            // Move grab/snap points in opposite direction of club's movement 
            primaryGrabPoint.transform.localPosition -= Vector3.forward * grabbingObjectControllerEvents.GetTouchpadAxis().y * Time.deltaTime;
            secondaryGrabPoint.transform.localPosition -= Vector3.forward * grabbingObjectControllerEvents.GetTouchpadAxis().y * Time.deltaTime;

            // Connect stored controller's body back to club
            clubFixedJoint.connectedBody = connectedBody;
        }

        /// <summary>
        /// Tilt club on x-axis touchpad input.
        /// </summary>
        private void TiltGrabPoint()
        {
            // Store last connected body
            Rigidbody connectedBody = clubFixedJoint.connectedBody;

            // Remove connection
            clubFixedJoint.connectedBody = null;

            // Store current tilt to check tilt angle and later add this angle to new clubs
            currentTilt += grabbingObjectControllerEvents.GetTouchpadAxis().x * Time.deltaTime * 100;

            // Rotate club around grab point
            transform.RotateAround(primaryGrabPoint.transform.position, primaryGrabPoint.transform.right, grabbingObjectControllerEvents.GetTouchpadAxis().x * Time.deltaTime * 100);

            // Rotate grab point around its own x-axis to compensate club global rotation
            primaryGrabPoint.transform.RotateAround(primaryGrabPoint.transform.position, primaryGrabPoint.transform.right, grabbingObjectControllerEvents.GetTouchpadAxis().x * Time.deltaTime * 100);

            // Connect stored controller's body back to club
            clubFixedJoint.connectedBody = connectedBody;
        }

        /// <summary>
        /// Create follower object to perform punch.
        /// </summary>
        private void SpawnClubFollower()
        {
            if (!follower)
            {
                follower = Instantiate(
                    clubFollowerPrefab,
                    clubPunchBox.transform.position,
                    clubPunchBox.transform.rotation
                );

                follower.SetFollowTarget(clubPunchBox.transform);
            }
            else
            {
                follower.SetFollowTarget(clubPunchBox.transform);
            }
        }

        /// <summary>
        /// Toggle IsTrigger property in colliders, applied to club.
        /// If club is grabbed, colliders must be just trigger.
        /// If club is not grabbed, colliders must apply physics to club.
        /// </summary>
        private void ToggleCollidersTriggersState(bool state)
        {
            clubColliders.All(collider => { collider.isTrigger = state; return true; });
        }

        /// <summary>
        /// Changes render mode of shader in runtime
        /// </summary>
        /// <param name="shaderMaterial">Reference to material, that need to be changed.</param>
        /// <param name="renderMode">Render mode (Opaque / Fade).</param>
        private void ChangeShaderRenderMode(ref Material shaderMaterial, string renderMode)
        {
            switch (renderMode)
            {
                case "Opaque":
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    shaderMaterial.SetInt("_ZWrite", 1);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = -1;
                    break;

                case "Fade":
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    shaderMaterial.SetInt("_ZWrite", 0);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = 3000;
                    break;
            }
        }

        /// <summary>
        /// Aster short delay fade out club and then destroy its instance.
        /// </summary>
        private IEnumerator DestroyClubOnDrop()
        {
            clubDestroyEnabled = true;

            yield return new WaitForSeconds(2);

            ChangeShaderRenderMode(ref clubShaderMaterial, "Fade");

            clubShaderMaterial.DOFade(0, 2f);

            yield return new WaitForSeconds(2);

            Destroy(this.gameObject);

            yield break;
        }

        /// <summary>
        /// Stop destroying club if pick it up back. Set render mode back to opaque after fade in.
        /// </summary>
        private void StopDestroyClubOnPickUp()
        {
            StopAllCoroutines();

            clubDestroyEnabled = false;

            clubShaderMaterial.DOFade(255, .5f);

            ChangeShaderRenderMode(ref clubShaderMaterial, "Opaque");
        }
    }
}
