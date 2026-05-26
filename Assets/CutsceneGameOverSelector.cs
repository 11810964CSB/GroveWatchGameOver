//using UnityEngine;

//[DefaultExecutionOrder(-100)] //you're a wizard harry

//// THIS DOESNT WORK YET, TBD ONCE I AM ALLOWED TO PUSH TO MAIN
//public class GameOverCutsceneSelector : MonoBehaviour
//{
//    [Header("Cutscene per IncidentType (match with GameManager.cs)")]
//    [SerializeField] private CutsceneData fireEnding;         // 0 - Fire
//    [SerializeField] private CutsceneData droughtEnding;      // 1 - Drought
//    [SerializeField] private CutsceneData floodEnding;        // 2 - Flood
//    [SerializeField] private CutsceneData campingEnding;      // 3 - Camping
//    [SerializeField] private CutsceneData loggingEnding;      // 4 - Logging
//    [SerializeField] private CutsceneData constructionEnding; // 5 - Construction

//    [Header("Fallback")]
//    [SerializeField] private CutsceneData fallbackEnding;

//    ServiceHub _sH;

//    void Awake()
//    {
//        _sH = ServiceHub.Instance;
//        //for testing remove if needed
//        if (CutsceneState.SelectedCutscene != null)
//            return;

//        CutsceneData chosen = SelectFromIncident();
//        CutsceneState.SelectedCutscene = chosen != null ? chosen : fallbackEnding;
//    }

//    CutsceneData SelectFromIncident()
//    {

//        IncidentType dominant = _sH._gM.GetDominantIncident();

//        switch (dominant)
//        {
//            case IncidentType.Fire: return fireEnding;
//            case IncidentType.Drought: return droughtEnding;
//            case IncidentType.Flood: return floodEnding;
//            case IncidentType.Camping: return campingEnding;
//            case IncidentType.Logging: return loggingEnding;
//            case IncidentType.Construction: return constructionEnding;
//            default: return null;
//        }
//    }
//}