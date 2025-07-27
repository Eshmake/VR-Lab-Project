using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DistanceLimitedGrabWithSnap : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [Header("Grab Distance Settings")]
    [SerializeField] private float maxGrabDistance = 0.5f;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor cachedInteractor;

    [Header("Snap Settings")]
    [SerializeField] private HingeJoint hingeJoint;
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float snapThreshold = 10f;
    [SerializeField] private float snapSpringStrength = 150f;
    [SerializeField] private float snapDamper = 20f;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        cachedInteractor = args.interactorObject;

        // Temporarily disable spring while grabbed
        if (hingeJoint != null)
        {
            JointSpring noSpring = hingeJoint.spring;
            noSpring.spring = 0;
            noSpring.damper = 0;
            hingeJoint.spring = noSpring;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        cachedInteractor = null;

        TrySnapDoor();
    }

    private void Update()
    {
        if (cachedInteractor != null)
        {
            float distance = Vector3.Distance(
                cachedInteractor.transform.position,
                transform.position
            );

            if (distance > maxGrabDistance)
            {
                interactionManager.SelectExit(cachedInteractor, this);
                cachedInteractor = null;

                TrySnapDoor();
            }
        }
    }

    private void TrySnapDoor()
    {
        if (hingeJoint == null)
            return;

        float currentAngle = hingeJoint.angle;

        float distanceToClosed = Mathf.Abs(currentAngle - closedAngle);
        float distanceToOpen = Mathf.Abs(currentAngle - openAngle);

        float targetAngle;
        if (distanceToClosed < snapThreshold)
            targetAngle = closedAngle;
        else if (distanceToOpen < snapThreshold)
            targetAngle = openAngle;
        else
            return; // No snap if not near either

        JointSpring spring = hingeJoint.spring;
        spring.spring = snapSpringStrength;
        spring.damper = snapDamper;
        spring.targetPosition = targetAngle;

        hingeJoint.useSpring = true;
        hingeJoint.spring = spring;
    }
}

