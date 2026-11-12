namespace QuestSlicer.Core
{
    using UnityEngine;

    /// <summary>
    /// Hält die einzige WorkspaceScale-Instanz des Workspace, damit Controller
    /// und Workspace denselben Ansichtsmaßstab verwenden.
    /// </summary>
    public class WorkspaceScaleHost : MonoBehaviour
    {
        private readonly WorkspaceScale _scale = new WorkspaceScale();

        public WorkspaceScale Scale => _scale;
    }
}
