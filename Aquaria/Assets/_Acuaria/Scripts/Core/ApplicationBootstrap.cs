using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acuaria.Core
{
    [DisallowMultipleComponent]
    public sealed class ApplicationBootstrap : MonoBehaviour
    {
        private const string ExpectedSceneName = "Bootstrap";

        private void OnEnable()
        {
            string activeSceneName = SceneManager.GetActiveScene().name;

            if (activeSceneName != ExpectedSceneName)
            {
                Debug.LogError(
                    $"{nameof(ApplicationBootstrap)} must be loaded from the {ExpectedSceneName} scene. " +
                    $"Active scene: {activeSceneName}.",
                    this);
            }
        }
    }
}
