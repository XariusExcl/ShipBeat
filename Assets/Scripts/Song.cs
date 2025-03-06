public struct SongValidationResult {
    public bool Valid;
    public string Message;
    public SongData Data;
}

public struct SongInfo {
    public string Title;
    public string Artist;
    public string Creator;
    public string ChartFile;
    public string AudioFile;
    public float BPM;
    public float SongStart;
    public float Length;
    public string DifficultyName;
    public int DifficultyRating;
    public int NoteCount;
}

public struct SongData {
    public SongInfo Info;
    public Note[] Notes;
}