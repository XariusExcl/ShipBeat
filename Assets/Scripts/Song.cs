public struct SongValidationResult {
    public bool Valid;
    public string Message;
    public SongData Data;
}

public struct SongDataInfo {
    public string Title;
    public string Artist;
    public string Creator;
    public string ChartFile;
    public string AudioFile;
    public string BackgroundImage;
    public string BPM;
    public float SongStart;
    public float SongPreviewStart;
    public float Length;
    public string DifficultyName;
    public int DifficultyRating;
    public int NoteCount;
    public int LaneCount;
}

public struct TimingPoint {
    public float Time;
    public float BPM;
    public int Meter;
    public bool KiaiMode;
}

public struct SongData {
    public SongDataInfo Info;
    public TimingPoint[] TimingPoints;
    public Note[] Notes;
}