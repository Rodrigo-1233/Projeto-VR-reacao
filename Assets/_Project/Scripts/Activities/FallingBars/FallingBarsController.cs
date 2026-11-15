using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using ReflexLab.Data;

namespace ReflexLab.Activities.FallingBars
{
    public enum ActivityState
    {
        IdleWaitingStart,
        Countdown,
        WaitingDrop,
        Falling,
        TrialCompleted,
        SessionFinished
    }

    /// <summary>
    /// Controlador principal da atividade FallingBars (avaliação de tempo de reação visual simples).
    /// </summary>
    public class FallingBarsController : MonoBehaviour
    {
        [Header("Configurações do Teste")]
        [SerializeField] private int totalTrials = 5;
        [SerializeField] private float minWaitDelaySeconds = 1.5f;
        [SerializeField] private float maxWaitDelaySeconds = 4.0f;
        [SerializeField] private float fallSpeed = 3.5f;
        [SerializeField] private float catchDistanceThreshold = 1.6f;

        [Header("Referências da Barra")]
        [SerializeField] private Transform barTransform;
        [SerializeField] private Vector3 barSpawnPosition = new Vector3(0, 2.2f, 2.2f);
        [SerializeField] private Vector3 barCatchTargetPosition = new Vector3(0, 1.1f, 2.2f);
        [SerializeField] private Renderer barRenderer;
        [SerializeField] private Color readyColor = new Color(0.2f, 0.6f, 0.9f);
        [SerializeField] private Color fallingColor = new Color(0.95f, 0.45f, 0.2f);
        [SerializeField] private Color caughtSuccessColor = new Color(0.2f, 0.8f, 0.4f);
        [SerializeField] private Color missedColor = new Color(0.85f, 0.2f, 0.2f);

        [Header("Interface e Textos (TMP)")]
        [SerializeField] private TextMeshPro statusText;
        [SerializeField] private TextMeshPro trialCounterText;
        [SerializeField] private TextMeshPro reactionTimeResultText;
        [SerializeField] private TextMeshPro instructionText;
        [SerializeField] private GameObject returnButtonObject;

        [Header("Entrada VR / Teclado")]
        [SerializeField] private InputActionProperty vrTriggerAction;

        // Estado interno
        private ActivityState currentState = ActivityState.IdleWaitingStart;
        private int currentTrialIndex = 0;
        private float fallStartTime = 0f;
        private Coroutine activeRoutine;
        private List<TrialRecord> trialHistory = new List<TrialRecord>();

        private void Start()
        {
            ResetBarVisuals();
            UpdateUI();
            ShowInstructions();
        }

        private void Update()
        {
            // Checa input do usuário: Tecla Espaço (no teclado/simulador) ou Gatilho do VR
            bool inputPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            
            if (vrTriggerAction.action != null && vrTriggerAction.action.WasPressedThisFrame())
            {
                inputPressed = true;
            }

            // Também aceita clique com mouse no Editor
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                inputPressed = true;
            }

            if (inputPressed)
            {
                HandleInput();
            }

            // Movimento de queda da barra
            if (currentState == ActivityState.Falling && barTransform != null)
            {
                barTransform.position += Vector3.down * (fallSpeed * Time.deltaTime);

                // Se a barra caiu abaixo do limite sem o paciente reagir
                if (barTransform.position.y <= (barCatchTargetPosition.y - 0.5f))
                {
                    OnMissedReaction();
                }
            }
        }

        private void HandleInput()
        {
            switch (currentState)
            {
                case ActivityState.IdleWaitingStart:
                    StartSession();
                    break;

                case ActivityState.Countdown:
                case ActivityState.WaitingDrop:
                    // Falso início / antecipação antes da barra cair
                    OnPrematureTrigger();
                    break;

                case ActivityState.Falling:
                    // Reação bem-sucedida
                    OnSuccessfulReaction();
                    break;

                case ActivityState.TrialCompleted:
                    // Avança para a próxima tentativa
                    StartNextTrial();
                    break;

                case ActivityState.SessionFinished:
                    // Retorna para a sala hospitalar
                    ReturnToHospitalRoom();
                    break;
            }
        }

        public void StartSession()
        {
            currentTrialIndex = 0;
            trialHistory.Clear();
            if (returnButtonObject != null) returnButtonObject.SetActive(false);
            StartNextTrial();
        }

        private void StartNextTrial()
        {
            if (currentTrialIndex >= totalTrials)
            {
                FinishSession();
                return;
            }

            currentTrialIndex++;
            currentState = ActivityState.Countdown;
            ResetBarVisuals();
            UpdateUI();

            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(TrialRoutine());
        }

        private IEnumerator TrialRoutine()
        {
            // Contagem regressiva suave para acalmar o paciente
            if (statusText != null) statusText.text = "Atenção...";
            if (instructionText != null) instructionText.text = "Prepare o controle. A barra cairá em instantes.";
            yield return new WaitForSeconds(1.0f);

            currentState = ActivityState.WaitingDrop;
            if (statusText != null) statusText.text = "Pronto...";

            // Delay aleatório imperceptível (evita antecipação motora rítmica)
            float randomDelay = UnityEngine.Random.Range(minWaitDelaySeconds, maxWaitDelaySeconds);
            yield return new WaitForSeconds(randomDelay);

            // Início da queda
            currentState = ActivityState.Falling;
            fallStartTime = Time.time;

            if (barRenderer != null) barRenderer.material.color = fallingColor;
            if (statusText != null) statusText.text = "APERTE AGORA!";
        }

        private void OnSuccessfulReaction()
        {
            float reactionTimeMs = (Time.time - fallStartTime) * 1000f;
            currentState = ActivityState.TrialCompleted;

            if (barRenderer != null) barRenderer.material.color = caughtSuccessColor;

            TrialRecord record = new TrialRecord
            {
                trialIndex = currentTrialIndex,
                reactionTimeMs = reactionTimeMs,
                isSuccess = true,
                accuracyPercentage = 100f,
                timestamp = Time.time
            };
            trialHistory.Add(record);

            if (statusText != null) statusText.text = $"Excelente! {reactionTimeMs:F0} ms";
            if (instructionText != null) instructionText.text = "Pressione o gatilho para continuar.";
            if (reactionTimeResultText != null) reactionTimeResultText.text = $"Último tempo: {reactionTimeMs:F0} ms";

            UpdateUI();
        }

        private void OnMissedReaction()
        {
            currentState = ActivityState.TrialCompleted;
            if (barRenderer != null) barRenderer.material.color = missedColor;

            TrialRecord record = new TrialRecord
            {
                trialIndex = currentTrialIndex,
                reactionTimeMs = 9999f,
                isSuccess = false,
                accuracyPercentage = 0f,
                timestamp = Time.time
            };
            trialHistory.Add(record);

            if (statusText != null) statusText.text = "Barra caiu!";
            if (instructionText != null) instructionText.text = "Não se preocupe. Pressione o gatilho para tentar novamente.";
            UpdateUI();
        }

        private void OnPrematureTrigger()
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            currentState = ActivityState.TrialCompleted;
            if (barRenderer != null) barRenderer.material.color = missedColor;

            if (statusText != null) statusText.text = "Acionamento antecipado!";
            if (instructionText != null) instructionText.text = "Aguarde a barra iniciar o movimento. Pressione para reiniciar.";
            UpdateUI();
        }

        private void FinishSession()
        {
            currentState = ActivityState.SessionFinished;
            ResetBarVisuals();

            float totalMs = 0f;
            int validCount = 0;
            foreach (var r in trialHistory)
            {
                if (r.isSuccess)
                {
                    totalMs += r.reactionTimeMs;
                    validCount++;
                }
            }

            float averageMs = validCount > 0 ? (totalMs / validCount) : 0f;

            if (statusText != null) statusText.text = "Avaliação Concluída!";
            if (reactionTimeResultText != null) reactionTimeResultText.text = $"Tempo Médio: {averageMs:F0} ms ({validCount}/{totalTrials} acertos)";
            if (instructionText != null) instructionText.text = "Excelente trabalho! Pressione o gatilho para voltar à Sala Hospitalar.";
            if (returnButtonObject != null) returnButtonObject.SetActive(true);

            UpdateUI();
        }

        public void ReturnToHospitalRoom()
        {
            SceneManager.LoadScene("HospitalRoom");
        }

        private void ResetBarVisuals()
        {
            if (barTransform != null)
            {
                barTransform.position = barSpawnPosition;
            }
            if (barRenderer != null)
            {
                barRenderer.material.color = readyColor;
            }
        }

        private void ShowInstructions()
        {
            if (statusText != null) statusText.text = "Barras Caindo";
            if (instructionText != null) instructionText.text = "Pressione o gatilho do controle ou [Espaço] para começar.";
            if (reactionTimeResultText != null) reactionTimeResultText.text = "";
        }

        private void UpdateUI()
        {
            if (trialCounterText != null)
            {
                trialCounterText.text = $"Tentativa: {currentTrialIndex}/{totalTrials}";
            }
        }
    }
}
