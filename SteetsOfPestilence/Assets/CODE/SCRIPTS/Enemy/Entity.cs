using UnityEngine;

public class Entity : MonoBehaviour
{
    protected bool m_isEnabled = true;

    public void Disable(bool _disable)
    {
        m_isEnabled = _disable;
    }

}
