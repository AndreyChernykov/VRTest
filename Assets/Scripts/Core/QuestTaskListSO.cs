using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "QuestTaskListSO", menuName = "Scriptable Objects/QuestTaskListSO")]
    public class QuestTaskListSO : ScriptableObject
    {
        [field: SerializeField] public List<QuestTask> Tasks { get; private set; }
    }

    [System.Serializable]
    public class QuestTask
    {
        [field: SerializeField] public QuestStep Step {  get; private set; }
        [field: SerializeField] public string TaskDescription { get; private set; }
    }
}


