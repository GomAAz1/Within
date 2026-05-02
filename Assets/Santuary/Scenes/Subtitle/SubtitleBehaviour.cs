using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class SubtitleBehaviour : PlayableBehaviour
{
    public string subtitleText;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI textMesh = playerData as TextMeshProUGUI;
        if (textMesh != null)
        {
            textMesh.text = subtitleText;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, info.weight);
        }
    }
}