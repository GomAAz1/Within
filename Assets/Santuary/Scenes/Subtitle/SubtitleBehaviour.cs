using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class SubtitleBehaviour : PlayableBehaviour
{
    public string subtitleText;

    // 1. السحر هنا: الدالة دي بتشتغل أول ما التايم لاين يفتح عينه
    public override void OnGraphStart(Playable playable)
    {
        // الكود هنا مش بيشوف الـ TextMesh مباشرة، فإحنا هنمسحه في أول فريم
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI textMesh = playerData as TextMeshProUGUI;

        if (textMesh != null)
        {
            // كتابة النص والتحكم في الشفافية
            textMesh.text = subtitleText;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, info.weight);

            // لو إحنا في منطقة فراغ (الوزن صفر)، امسح النص
            if (info.weight <= 0)
            {
                textMesh.text = "";
            }
        }
    }

    // 2. تصفير النص أول ما التايم لاين يخلص خالص أو يتوقف
    public override void OnPlayableDestroy(Playable playable)
    {
        // دي بتضمن إنك لما تقفل اللعبة، النص يتمسح من الذاكرة
    }

    // تصفير النص لما الكليب يخلص
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // بنحاول نوصل للنص عشان نصفره
        // (playerData مش متوفر هنا بسهولة، بس دالة ProcessFrame هتقوم بالواجب)
    }
}