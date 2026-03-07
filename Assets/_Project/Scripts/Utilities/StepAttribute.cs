using UnityEngine;

public class StepAttribute : PropertyAttribute
{
    public float Step { get; private set; }

    public StepAttribute(float step = 0.05f)
    {
        Step = step;
    }
}
