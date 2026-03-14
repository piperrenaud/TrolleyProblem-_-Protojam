using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    public TrolleyMovement trolley;
    public Button doNothingButton;
    public Button pullLeverButton;
    public TrackSpawner trackSpawner;

    private void Start()
    {
        doNothingButton.onClick.AddListener(OnDoNothing);
        pullLeverButton.onClick.AddListener(OnPullLever);

        trolley.choicesButtons.SetActive(false);
    }

    private void OnDoNothing()
    {
        trackSpawner.ForceNextTrack(2); //junction left
        trolley.choicesButtons.SetActive(false);
    }

    private void OnPullLever()
    {
        trackSpawner.ForceNextTrack(3); //junction right
        trolley.choicesButtons.SetActive(false);
    }
}
