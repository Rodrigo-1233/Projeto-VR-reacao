namespace ReflexLab.Core
{
    /// <summary>
    /// Tipos de atividades disponíveis no ReflexLab.
    /// </summary>
    public enum ActivityType
    {
        FallingBars,
        TargetShooting,
        ObjectSelection
    }

    /// <summary>
    /// Gerenciador principal de fluxo e estado das avaliações do ReflexLab.
    /// </summary>
    public class GameManager : UnityEngine.MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public ActivityType CurrentActivity { get; private set; } = ActivityType.FallingBars;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SelectActivity(ActivityType activity)
        {
            CurrentActivity = activity;
            UnityEngine.Debug.Log($"[ReflexLab] Atividade selecionada: {CurrentActivity}");
        }
    }
}
