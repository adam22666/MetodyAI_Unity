using UnityEngine;

namespace TL.UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        public string actionName;
        private float _score;
        public float score
        {
            get { return _score; }
            set { _score = Mathf.Clamp01(value); }
        }

        [SerializeField] public Consideration[] considerations;
        public Transform RequiredDestination { get; protected set; }

        // volane ked je akcia skutocne "finalne" spustena (v stave Execute)
        public abstract void Execute(NPCController npc);

        // volane, ked sme v stave Move, ale chceme subezne strielat
        public virtual void ParallelExecute(NPCController npc)
        {
            // default
        }

        public virtual void SetRequiredDestination(NPCController npc) { }
    }
}
