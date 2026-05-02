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
            // كتابة النص
            textMesh.text = subtitleText;

            // التحكم في الشفافية (الفيد)
            // info.weight بيوصل لـ 1 لما الكليب يشتغل، وصفر في الفراغات
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, info.weight);

            // تكة إضافية: لو الشفافية صفر (يعني إحنا بره الكليب)، امسح النص
            if (info.weight <= 0)
            {
                textMesh.text = "";
            }
        }
    }

    // الدالة دي بتشتغل لما الخط الأبيض يخرج بره الكليب
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // أول ما الكليب يخلص، بنصفر النص فوراً
        var textMesh = playable.GetGraph().GetResolver() as TextMeshProUGUI;
        if (textMesh != null) textMesh.text = "";
    }
}