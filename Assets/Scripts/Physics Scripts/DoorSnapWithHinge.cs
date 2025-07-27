using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorSnapWithHinge : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [Header("Hinge Settings")]
    public HingeJoint hingeJoint;

    [Header("Snap Settings")]
    public float closedAngle = 0f;
    public float openAngle = 90f;
    public float snapThreshold = 10f;
    public float snapSpringStrength = 150f;
    public float snapDamper = 20f;

    private JointSpring originalSpring;
    private bool wasGrabbed = false;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        wasGrabbed = true;

        // Disable spring during grab
        originalSpring = hingeJoint.spring;
        JointSpring noSpring = originalSpring;
        noSpring.spring = 0;
        noSpring.damper = 0;
        hingeJoint.spring = noSpring;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        wasGrabbed = false;

        float currentAngle = hingeJoint.angle;

        float distanceToClosed = Mathf.Abs(currentAngle - closedAngle);
        float distanceToOpen = Mathf.Abs(currentAngle - openAngle);

        float targetAngle;
        if (distanceToClosed < snapThreshold)
            targetAngle = closedAngle;
        else if (distanceToOpen < snapThreshold)
            targetAngle = openAngle;
        else
            return; // No snapping if not near either

        JointSpring snapSpring = hingeJoint.spring;
        snapSpring.spring = snapSpringStrength;
        snapSpring.damper = snapDamper;
        snapSpring.targetPosition = targetAngle;

        hingeJoint.useSpring = true;
        hingeJoint.spring = snapSpring;
    }
}
