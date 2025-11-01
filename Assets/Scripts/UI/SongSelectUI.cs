using UnityEngine;
using UnityEngine.Events;

public class SongSelectUI : MonoBehaviour
{
    [SerializeField] SongCaroussel songCaroussel;
    [SerializeField] SongSelectReadyMenu songSelectReadyMenu;
    [SerializeField] PlayerSettingsUI playerSettingsUI;
    public static UnityEvent OnSongValidated = new();

    void Awake()
    {
        SongCaroussel.OnSongSelected.AddListener(OnSongSelected);
    }

    void Start()
    {
        songCaroussel.HasFocus = true;
        songSelectReadyMenu.HasFocus = false;
        playerSettingsUI.HasFocus = false;
    }

    void Update()
    {
        // Failsafe
        if (!songCaroussel.HasFocus && !songSelectReadyMenu.HasFocus && !playerSettingsUI.HasFocus)
            songCaroussel.HasFocus = true;

        if (Input.GetButtonDown("P1_Start"))
        {
            playerSettingsUI.HasFocus = !playerSettingsUI.HasFocus;
            songCaroussel.HasFocus = !playerSettingsUI.HasFocus;
        }
    }

    void OnSongSelected()
    {
        songCaroussel.HasFocus = false;
        songSelectReadyMenu.HasFocus = true;
        playerSettingsUI.HasFocus = false;
    }

    void OnDestroy()
    {
        SongCaroussel.OnSongSelected.RemoveListener(OnSongSelected);
    }
}
