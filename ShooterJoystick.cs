using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class ShooterJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum AxisOption
    {
        Both,
        OnlyHorizontal,
        OnlyVertical
    }

    public int MovementRange = 100;
    public AxisOption axesToUse = AxisOption.Both;
    public float Sensitivity_X = 10;
    public float Sensitivity_Y = 10;
    public string horizontalAxisName = "Mouse X";
    public string verticalAxisName = "Mouse Y";

    private bool pointerDown = false;
    private Vector3 m_StartPos;
    private Vector3 oldPos;
    private int touchIndex;
    private int fingerIndex;
    private Touch touch;

    private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;
    private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

    private void Awake()
    {
        m_StartPos = transform.localPosition;
        oldPos = Vector3.zero;
        StartCoroutine(Initialize());
    }

    private System.Collections.IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        CreateVirtualAxes();
    }

    private void CreateVirtualAxes()
    {
        bool useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        bool useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        if (useX)
        {
            m_HorizontalVirtualAxis = CrossPlatformInputManager.VirtualAxisReference(horizontalAxisName) ??
                                       new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
        }

        if (useY)
        {
            m_VerticalVirtualAxis = CrossPlatformInputManager.VirtualAxisReference(verticalAxisName) ??
                                     new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
        }
    }

    private void UpdateVirtualAxes(Vector3 value)
    {
        value /= MovementRange;

        if (m_HorizontalVirtualAxis != null)
            m_HorizontalVirtualAxis.Update(value.x * Sensitivity_X);

        if (m_VerticalVirtualAxis != null)
            m_VerticalVirtualAxis.Update(value.y * Sensitivity_Y);
    }

    private void Update()
    {
        if (pointerDown)
            Drag();
    }

    private Touch GetUpdatedTouch(int fingerIdAtStart) //Это нужно чтобы запаминало какой палец управляет стиком
    {
        foreach (Touch touchf in Input.touches)
        {
            if (touchf.fingerId == fingerIdAtStart)
                return touchf;
        }
        return default;
    }

    private void Drag()
    {
        touch = GetUpdatedTouch(fingerIndex);

        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Vector3 newPos = touch.position;

            Vector3 touchDirection = newPos - oldPos;
            touchDirection.x = Mathf.Clamp(touchDirection.x, -Sensitivity_X, Sensitivity_X);
            touchDirection.y = Mathf.Clamp(touchDirection.y, -Sensitivity_Y, Sensitivity_Y);

            transform.localPosition = Vector3.ClampMagnitude(transform.parent.InverseTransformPoint(newPos), MovementRange) + m_StartPos;
            UpdateVirtualAxes(touchDirection);

            oldPos = newPos;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        pointerDown = false;
        transform.localPosition = m_StartPos;
        UpdateVirtualAxes(Vector3.zero);
    }

    public void OnPointerDown(PointerEventData data)
    {
        pointerDown = true;
        touchIndex = data.pointerId;
        fingerIndex = touchIndex;
        oldPos = GetUpdatedTouch(fingerIndex).position;
    }
}