using UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Core
{

    public class TaskManager : MonoBehaviour
    {
        [Header("Scriptable Object Quests Data")]
        [SerializeField] private QuestTaskListSO questTaskListSO;

        [Header("UI Configuration")]
        [SerializeField] private UIFollowPlayer ui;

        [Header("Interactive Elements")]
        [SerializeField] private XRGrabInteractable objectA;
        [SerializeField] private XRSocketInteractor socketB;
        [SerializeField] private VRLeverController lever;

        [Header("Outlines")]
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

            QuestTask foundTask = questTaskListSO.Tasks.Find(task => task.Step == _currentStep);
            ui.SetTextTask(foundTask.TaskDescription);

            switch (_currentStep)
            {
                case QuestStep.GrabObjectA:
                    if (outlineA != null) outlineA.enabled = true;
                    break;

                case QuestStep.PlaceInSocket:
                    if (outlineB != null) outlineB.enabled = true;
                    break;

                case QuestStep.ToggleLever:
                    if (outlineLever != null) outlineLever.enabled = true;
                    break;

                case QuestStep.Completed:
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
                XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;
                Transform attachPoint = socket != null && socket.attachTransform != null
                    ? socket.attachTransform
                    : args.interactorObject.transform;

                objectA.transform.position = attachPoint.position;
                objectA.transform.rotation = attachPoint.rotation;

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

