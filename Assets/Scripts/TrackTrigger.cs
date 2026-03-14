using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var trolley = other.GetComponentInParent<TrolleyMovement>();
        if (trolley != null)
        {
            trolley.choicesButtons.SetActive(true);
        }
    }
}
