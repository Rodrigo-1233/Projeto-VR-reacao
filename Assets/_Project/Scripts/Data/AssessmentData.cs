using System;
using UnityEngine;

namespace ReflexLab.Data
{
    /// <summary>
    /// Registro individual de tentativa de avaliação (para qualquer uma das três tarefas).
    /// </summary>
    [Serializable]
    public class TrialRecord
    {
        public int trialIndex;
        public float reactionTimeMs;
        public bool isSuccess;
        public float accuracyPercentage;
        public Vector2 targetOffset;
        public string selectedObjectName;
        public string targetObjectName;
        public float timestamp;
    }

    /// <summary>
    /// Estrutura para armazenar a sessão completa de um paciente.
    /// Preparado para exportação e integração futura com prontuários ou dashboards.
    /// </summary>
    [Serializable]
    public class AssessmentSession
    {
        public string sessionId;
        public string patientId;
        public ReflexLab.Core.ActivityType activityType;
        public float totalDurationSeconds;
        public float averageReactionTimeMs;
        public float successRate;
        public System.Collections.Generic.List<TrialRecord> trials = new System.Collections.Generic.List<TrialRecord>();
    }
}
