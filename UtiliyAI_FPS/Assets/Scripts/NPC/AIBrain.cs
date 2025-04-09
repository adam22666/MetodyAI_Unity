using UnityEngine;
using System.Collections.Generic;
using TL.Core;
using TL.UI;
using System.Text;

namespace TL.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        [SerializeField] private bool showActionScores = true;
        
        public Action BestAction { get; private set; }
        public bool finishedExecutingBestAction { get; set; }
        
        [SerializeField] private Action[] availableActions;
        [SerializeField] public Billboard billboard;
        
        public Dictionary<string, float> CurrentActionScores { get; private set; } = new Dictionary<string, float>();
        
        private NPCController npc;
        
        void Start()
        {
            npc = GetComponent<NPCController>();
        }
        
        public void DecideBestAction()
        {
            float highestScore = 0f;
            BestAction = null;
            StringBuilder scoreReport = new StringBuilder("\n=== ACTION SCORES ===\n");
            CurrentActionScores.Clear();
            
            foreach (Action action in availableActions)
            {
                float score = ScoreAction(action);
                scoreReport.AppendLine($"{action.name.PadRight(15)}: {score:F2}");
                CurrentActionScores[action.name] = score;
                
                if (score > highestScore)
                {
                    highestScore = score;
                    BestAction = action;
                }
            }
            
            if (showActionScores)
            {
                if (billboard != null)
                    billboard.UpdateActionScoresText(scoreReport.ToString());
            }
            
            if (BestAction == null && availableActions.Length > 0)
            {
                BestAction = availableActions[0];
                Debug.LogWarning($"Using fallback action: {BestAction.name}");
            }
            
            if (BestAction != null)
            {
                Debug.Log($"Selected action: {BestAction.name}");
                BestAction.SetRequiredDestination(npc);
                if (billboard != null)
                    billboard.UpdateBestActionText(BestAction.name);
            }
        }
        
        private float ScoreAction(Action action)
        {
            if (action.considerations == null || action.considerations.Length == 0)
            {
                Debug.LogError($"Action {action.name} has no considerations!");
                return 0f;
            }
            
            float score = 1f;
            foreach (var consideration in action.considerations)
            {
                float considerationScore = consideration.ScoreConsideration(npc);
                score *= considerationScore;
                if (score <= 0f)
                    break;
            }
            
            int considerationsCount = action.considerations.Length;
            float modificationFactor = 1f - (1f / Mathf.Max(considerationsCount, 1));
            float makeupValue = (1f - score) * modificationFactor;
            
            return score + (makeupValue * score);
        }
    }
}