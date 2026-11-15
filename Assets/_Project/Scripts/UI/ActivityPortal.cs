using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace ReflexLab.UI
{
    /// <summary>
    /// Componente de estação de atividade no ambiente virtual.
    /// Permite ao paciente visualizar instruções e iniciar a avaliação com o controle VR.
    /// </summary>
    public class ActivityPortal : MonoBehaviour
    {
        [Header("Configuração da Atividade")]
        [SerializeField] private ReflexLab.Core.ActivityType activityType;
        [SerializeField] private string targetSceneName;
        [SerializeField] private string activityTitle;
        [TextArea(3, 6)]
        [SerializeField] private string activityDescription;
        [SerializeField] private string durationOrTrialsInfo;

        [Header("Referências Visuais")]
        [SerializeField] private TextMeshPro titleText;
        [SerializeField] private TextMeshPro descriptionText;
        [SerializeField] private TextMeshPro detailsText;
        [SerializeField] private Renderer indicatorLight;
        [SerializeField] private Color normalColor = new Color(0.18f, 0.54f, 0.74f); // Azul hospitalar suave
        [SerializeField] private Color hoverColor = new Color(0.24f, 0.76f, 0.65f);  // Verde água relaxante
        [SerializeField] private Color activeColor = new Color(0.95f, 0.65f, 0.22f); // Âmbar sutil

        private Material indicatorMat;

        private void Awake()
        {
            if (indicatorLight != null)
            {
                indicatorMat = indicatorLight.material;
                SetIndicatorColor(normalColor);
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (titleText != null) titleText.text = activityTitle;
            if (descriptionText != null) descriptionText.text = activityDescription;
            if (detailsText != null) detailsText.text = durationOrTrialsInfo;
        }

        public void OnPointerEnter()
        {
            SetIndicatorColor(hoverColor);
        }

        public void OnPointerExit()
        {
            SetIndicatorColor(normalColor);
        }

        public void OnSelectActivity()
        {
            SetIndicatorColor(activeColor);
            Debug.Log($"[ReflexLab] Iniciando atividade: {activityType} -> Cena: {targetSceneName}");

            if (ReflexLab.Core.GameManager.Instance != null)
            {
                ReflexLab.Core.GameManager.Instance.SelectActivity(activityType);
            }

            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }

        private void SetIndicatorColor(Color color)
        {
            if (indicatorMat != null)
            {
                indicatorMat.color = color;
                if (indicatorMat.HasProperty("_EmissionColor"))
                {
                    indicatorMat.SetColor("_EmissionColor", color * 0.4f);
                    indicatorMat.EnableKeyword("_EMISSION");
                }
            }
        }
    }
}
