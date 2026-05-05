using UnityEngine;
using System.Collections;

public class SimpleCameraMove : MonoBehaviour
{
    public float moveDuration = 0.5f;

    private Coroutine currentMove;

    public void MoveTo(Transform target)
    {
        if (target == null) return;

        if (currentMove != null)
            StopCoroutine(currentMove);

        currentMove = StartCoroutine(MoveRoutine(target));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z // keep camera Z
        );

        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;

            // smoothstep easing (cleaner than linear)
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = endPos;
    }
}