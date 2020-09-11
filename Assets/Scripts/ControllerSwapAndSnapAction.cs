namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;
    using System.Collections;
    using VRTK;
    using DG.Tweening;

    public class ControllerSwapAndSnapAction : VRTK_BaseGrabAction
    {
        [SerializeField]
        [Tooltip("Point on club to snap secondary controller's model.")]
        private Transform pointToSnapSecondaryControllerModel;

        [Tooltip("The distance the secondary controller must move away from the original snap position before the secondary controller's model auto unsnaps.")]
        private float unsnapDistance = .5f;

        [SerializeField]
        [Tooltip("Speed of controller's model snapping.")]
        private float modelSnapSpeed = 1f;

        // Current GameObject that is grabbed (club in our case)
        private Club currentGrabbedObject;

        // This controller perform snap and grab, if initial grabbed controller is stopped it's grab
        private VRTK_InteractGrab secondaryController;
        private GameObject secondaryControllerModel;
        private Transform secondaryControllerBodyTransform;
        private VRTK_InteractTouch secondaryControllerTouch;
        private VRTK_InteractGrab secondaryControllerGrab;

        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);

            currentGrabbedObject = (Club)currentGrabbdObject;

            secondaryController = currentSecondaryGrabbingObject;
            secondaryControllerModel = VRTK_DeviceFinder.GetModelAliasController(secondaryController.gameObject);
            secondaryControllerBodyTransform = secondaryControllerModel.GetComponentInChildren<Rigidbody>().transform;
            secondaryControllerTouch = secondaryController.GetComponentInChildren<VRTK_InteractTouch>();
            secondaryControllerGrab = secondaryController.GetComponentInChildren<VRTK_InteractGrab>();

            // Snap model to club when component initialized (secondary grab performed)
            SnapAndUnsnapControllerModel(pointToSnapSecondaryControllerModel);
        }

        public override void ResetAction()
        {
            base.ResetAction();

            // Snap model back to it's controller when secondary controller release or controller moves too far from club
            SnapAndUnsnapControllerModel(secondaryController.transform);
        }

        public override void OnDropAction()
        {
            base.OnDropAction();

            // Force stop all tweens, now running on club
            currentGrabbedObject.transform.DOKill();

            // Init swap hands coroutine
            StartCoroutine(SwapHands());
        }

        /// <summary>
        /// The SnapAndUnsnapControllerModel method perform snapping secondary controller's model to club and back to controller when unsnap. Move/rotation uses tweener.
        /// </summary>
        /// <param name="snapToParentTransform">Transform, towards controllers's model need to be snapped to.</param>
        private void SnapAndUnsnapControllerModel(Transform snapToParentTransform)
        {
            secondaryControllerModel.transform.SetParent(snapToParentTransform, true);

            // Stop all tweens on controller
            secondaryControllerModel.transform.DOKill();

            // Snap model to club's grab point or back to controller
            if (snapToParentTransform == pointToSnapSecondaryControllerModel)
            {
                Vector3 snapPosition = secondaryControllerBodyTransform.InverseTransformPoint(secondaryControllerModel.transform.position);
                Vector3 snapRotation = secondaryControllerBodyTransform.InverseTransformDirection(secondaryControllerModel.transform.forward);

                secondaryControllerModel.transform.DOLocalMove(snapPosition, modelSnapSpeed);
                secondaryControllerModel.transform.DOLocalRotateQuaternion(Quaternion.LookRotation(snapRotation), modelSnapSpeed);
            }
            else
            {
                secondaryControllerModel.transform.DOLocalMove(Vector3.zero, modelSnapSpeed);
                secondaryControllerModel.transform.DOLocalRotateQuaternion(Quaternion.identity, modelSnapSpeed);
            }
        }

        // Use built-in logic for unsnap model if controller went too far from club
        protected override void CheckForceStopDistance(float ungrabDistance)
        {
            base.CheckForceStopDistance(ungrabDistance);
        }

        public override void ProcessUpdate()
        {
            base.ProcessUpdate();

            // Checking if controller went to far from club
            CheckForceStopDistance(unsnapDistance);
        }

        private IEnumerator SwapHands()
        {
            // Skipping current frame to let VRTK finish it's inner ungrab actions
            yield return null;

            // Toggling isSwappable property for using short-way VRTK grab logic
            isSwappable = true;

            // Attempt force touch and grab club by secondary controller
            secondaryControllerTouch.ForceTouch(currentGrabbedObject.gameObject);
            secondaryControllerGrab.AttemptGrab();

            // Toggling isSwappable property back for normal state
            isSwappable = false;
        }
    }
}