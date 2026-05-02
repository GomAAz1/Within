using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

[TrackColor(0.1f, 0.8f, 0.5f)]
[TrackBindingType(typeof(TextMeshProUGUI))]
[TrackClipType(typeof(SubtitleClip))]
public class SubtitleTrack : TrackAsset { }