using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PatientGenerator generator;
    public DialogueController dialogue;

    [Header("Patient Visual")]
    public PatientVisualGenerator visualGenerator;
    public Transform patientTransform;

    [Header("Position Settings")]
    public Vector3 centerPosition = new Vector3(0.66f, 0.33f, 6f);

    public float enterOffset = -3.5f;
    public float exitOffset = -5.5f;

    [Header("Timing")]
    public float moveSpeed = 4f;

    private PatientData currentPatient;

    // ----------------------------
    public void StartGame()
    {
        StartCoroutine(NewPatientSequence());
    }

    // ----------------------------
    public void ProcessTreatment(Dictionary<SymptomTag, int> mix)
    {
        bool success = Evaluate(mix, currentPatient);

        FindObjectOfType<NightManager>().RegisterResult(success);

        StartCoroutine(Transition());
    }

    // ----------------------------
    IEnumerator Transition()
    {
        dialogue.HideDialogue();

        yield return StartCoroutine(MovePatient(GetExitPosition()));

        currentPatient = generator.GeneratePatient();
        visualGenerator.GeneratePatient();

        patientTransform.position = GetEnterPosition();

        yield return StartCoroutine(MovePatient(centerPosition));

        dialogue.ShowPatient(currentPatient);
    }

    // ----------------------------
    IEnumerator NewPatientSequence()
    {
        currentPatient = generator.GeneratePatient();
        visualGenerator.GeneratePatient();

        patientTransform.position = GetEnterPosition();

        yield return StartCoroutine(MovePatient(centerPosition));

        dialogue.ShowPatient(currentPatient);
    }

    // ----------------------------
    public IEnumerator ForcePatientExit()
    {
        yield return StartCoroutine(MovePatient(GetExitPosition()));
    }

    // ----------------------------
    IEnumerator MovePatient(Vector3 target)
    {
        Vector3 start = patientTransform.position;
        float duration = 1f / moveSpeed;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            patientTransform.position = Vector3.Lerp(start, target, smoothT);

            yield return null;
        }

        patientTransform.position = target;
    }

    // ----------------------------
    Vector3 GetEnterPosition()
    {
        return new Vector3(
            centerPosition.x + enterOffset,
            centerPosition.y,
            centerPosition.z
        );
    }

    Vector3 GetExitPosition()
    {
        return new Vector3(
            centerPosition.x + exitOffset,
            centerPosition.y,
            centerPosition.z
        );
    }

    // ----------------------------
    bool Evaluate(Dictionary<SymptomTag, int> mix, PatientData patient)
    {
        foreach (var symptom in patient.symptoms)
        {
            int required = (int)symptom.intensity;

            if (!mix.ContainsKey(symptom.tag))
                return false;

            if (mix[symptom.tag] < required)
                return false;
        }

        return true;
    }
}