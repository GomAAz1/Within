using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    [TextArea(3, 10)]
    public string text;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        SubtitleBehaviour behaviour = playable.GetBehaviour();
        behaviour.subtitleText = text;
        return playable;
    }
}