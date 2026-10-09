using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Core
{

    public class TaskManager : MonoBehaviour
    {
        [Header("UI Configuration")]
        [SerializeField] private TextMeshProUGUI textTask;

        [Header("Interactive Elements")]
        [SerializeField] private XRGrabInteractable objectA;
        [SerializeField] private XRSocketInteractor socketB;
        [SerializeField] private LeverController lever;

        [Header("Outlines (Компоненты подсветки)")]
        [SerializeField] private Outline outlineA;
        [SerializeField] private Outline outlineB;
        [SerializeField] private Outline outlineLever;

        private QuestStep _currentStep = QuestStep.GrabObjectA;

        private void Start()
        {
            objectA.selectEntered.AddListener(OnObjectAGrabbed);
            socketB.selectEntered.AddListener(OnObjectAPlacedInSocket);
            lever.OnLeverActivated += OnLeverActivated;

            UpdateCurrentStep();
        }

        private void OnDestroy()
        {
            objectA.selectEntered.RemoveListener(OnObjectAGrabbed);
            socketB.selectEntered.RemoveListener(OnObjectAPlacedInSocket);
            lever.OnLeverActivated -= OnLeverActivated;
        }

        private void UpdateCurrentStep()
        {
            ToggleAllOutlines(false);

            switch (_currentStep)
            {
                case QuestStep.GrabObjectA:
                    textTask.text = "Возьмите объект A со стола.";
                    if (outlineA != null) outlineA.enabled = true;
                    break;

                case QuestStep.PlaceInSocket:
                    textTask.text = "Подойдите к объекту B и установите в него объект A.";
                    if (outlineB != null) outlineB.enabled = true;
                    break;

                case QuestStep.ToggleLever:
                    textTask.text = "Подойдите к рычагу и переведите его в положение «Включено».";
                    if (outlineLever != null) outlineLever.enabled = true;
                    break;

                case QuestStep.Completed:
                    textTask.text = "Тренировка завершена.";
                    break;
            }
        }

        private void OnObjectAGrabbed(SelectEnterEventArgs args)
        {
            if (_currentStep == QuestStep.GrabObjectA && !(args.interactorObject is XRSocketInteractor))
            {
                _currentStep = QuestStep.PlaceInSocket;
                UpdateCurrentStep();
            }
        }

        private void OnObjectAPlacedInSocket(SelectEnterEventArgs args)
        {
            if (_currentStep == QuestStep.PlaceInSocket && args.interactableObject.transform == objectA.transform)
            {
                objectA.enabled = false;

                _currentStep = QuestStep.ToggleLever;
                UpdateCurrentStep();
            }
        }

        private void OnLeverActivated()
        {
            if (_currentStep == QuestStep.ToggleLever)
            {
                _currentStep = QuestStep.Completed;
                UpdateCurrentStep();
            }
        }

        private void ToggleAllOutlines(bool state)
        {
            if (outlineA != null) outlineA.enabled = state;
            if (outlineB != null) outlineB.enabled = state;
            if (outlineLever != null) outlineLever.enabled = state;
        }
    }

}
public enum QuestStep
{
    GrabObjectA,
    PlaceInSocket,
    ToggleLever,
    Completed
}

