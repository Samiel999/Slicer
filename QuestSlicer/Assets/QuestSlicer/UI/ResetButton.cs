namespace QuestSlicer.UI
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Reset-Button für die VR-UI. Setzt alle Modell-Positionen zurück.
    /// </summary>
    public class ResetButton : MonoBehaviour
    {
        [SerializeField] private GameObject modelToReset;
        [SerializeField] private Vector3 defaultPosition = Vector3.zero;
        [SerializeField] private Vector3 defaultRotation = Vector3.zero;
        [SerializeField] private Vector3 defaultScale = Vector3.one;

        public UnityEvent OnReset;

        private void Awake()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnResetClicked);
        }

        private void OnResetClicked()
        {
            if (modelToReset != null)
            {
                modelToReset.transform.position = defaultPosition;
                modelToReset.transform.rotation = Quaternion.Euler(defaultRotation);
                modelToReset.transform.localScale = defaultScale;
            }
            OnReset?.Invoke();
        }
    }
}
