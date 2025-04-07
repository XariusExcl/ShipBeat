using System.Collections.Generic;
using UnityEngine;

enum ParseState
{
    None,
    General,
    Editor,
    Metadata,
    Difficulty,
    Events,
    TimingPoints,
    Colours,
    HitObjects,
    Done
};

public class OsuFileParser
{
    public static SongValidationResult ParseFile(TextAsset file, bool fastPass = false)
    {
        SongData songData = new();
        SongInfo songInfo = new();

        string[] lines = file.text.Split('\n');
        if (!lines[0].StartsWith("osu file format v"))
        {
            return new SongValidationResult { Valid = false, Message = "Invalid file format", Data = songData };
        }
        List<Note> notes = new();
        ParseState state = ParseState.None;
        int noteId = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (ParseState.Done == state) break;

            try {
                switch (state) {
                    case ParseState.None:
                        if (line.StartsWith("[General]"))
                            state = ParseState.General;
                        break;
                    case ParseState.General:
                        if (line.StartsWith("AudioFilename:"))
                            songInfo.AudioFile = line.Split(':')[1].Trim();

                        if (line.StartsWith("Mode:")) {
                            int mode = int.Parse(line.Split(':')[1]);
                            if (mode != 3)
                                return new SongValidationResult { Valid = false, Message = $"Invalid mode. Expected 3 (Mania), found {mode}", Data = songData };
                        }

                        if (line.StartsWith("PreviewTime:"))
                            songInfo.SongPreviewStart = float.Parse(line.Split(':')[1]) / 1000f;        

                        if (line.StartsWith("[Metadata]"))
                            state = ParseState.Metadata;
                        break;
                    case ParseState.Metadata:
                        if (line.StartsWith("Title:"))
                            songInfo.Title = line.Split(':')[1].Trim();
                        
                        if (line.StartsWith("Artist:"))
                            songInfo.Artist = line.Split(':')[1].Trim();
                        
                        if (line.StartsWith("Creator:"))
                            songInfo.Creator = line.Split(':')[1].Trim();

                        if (line.StartsWith("Version:"))
                            songInfo.DifficultyName = line.Split(':')[1].Trim();

                        if (line.StartsWith("Tags:"))
                            songInfo.DifficultyRating = int.Parse(line.Split(':')[1]);

                        

                        if (line.StartsWith("[Difficulty]"))
                            state = ParseState.Difficulty;
                        break;
                    case ParseState.Difficulty:
                        if (line.StartsWith("CircleSize:")) {
                            int circleSize = int.Parse(line.Split(':')[1]);
                            /*
                            if (circleSize != 8)
                                return new SongValidationResult { Valid = false, Message = $"Invalid lane count. Expected 8, found {circleSize}.", Data = songData };
                            */
                        }
                        if (line.StartsWith("[Events]"))
                            state = ParseState.Events;
                        break;
                    case ParseState.Events:
                        if (line.StartsWith("0,0,\"")) // Probably not the best way to check for background image, todo: test with a song that has storyboard
                            songInfo.BackgroundImage = line.Split(',')[2].Replace('"', ' ').Trim();

                        if (line.StartsWith("[TimingPoints]"))
                            state = ParseState.TimingPoints;
                        break;
                    case ParseState.TimingPoints:
                        // If I ignore it long enough, maybe it'll go away

                        if (line.StartsWith("[HitObjects]"))
                            state = ParseState.HitObjects;
                        break;
                    case ParseState.HitObjects:
                        string[] lineNote = line.Split(',');
                        if (i == lines.Length - 1)
                            state = ParseState.Done;
                            
                        if (lineNote.Length != 6) break;
                        
                        if (fastPass) {
                            songInfo.SongStart = float.Parse(lineNote[2])/1000f;
                            int j = 1;
                            string[] lastLine;
                            do {
                                lastLine = lines[^j].Split(',');
                                j++;
                            } while (lastLine.Length != 6 || lines.Length - j < i);
                            if (lines.Length - j < i) throw new System.Exception("Invalid file format, could not find end of notes (fastpass).");
                            songInfo.Length = float.Parse(lastLine[2])/1000f - songInfo.SongStart;
                            songInfo.NoteCount = lines.Length - i - 1;

                            state = ParseState.Done;
                            break;
                        }
                        notes.Add(new Note{
                            Id = noteId++,
                            Type = NoteType.Note,
                            Lane = int.Parse(lineNote[0])/ 64,
                            HitTime = float.Parse(lineNote[2])/1000f
                        });
                    break;
                }
            } catch (System.Exception e) {
                Debug.LogError($"Error parsing line: {line}, {e.Message}, {e.StackTrace}");
            }
        }
        if (state != ParseState.Done)
            return new SongValidationResult { Valid = false, Message = "File is missing sections", Data = songData };

        if (!fastPass) {
            songInfo.SongStart = notes[0].HitTime;
            songInfo.Length = notes[^1].HitTime - notes[0].HitTime;
            songInfo.NoteCount = notes.Count;
        }
        
        songInfo.SongPreviewStart = (songInfo.SongPreviewStart < 0) ? songInfo.Length/2 : songInfo.SongPreviewStart; 

        songData.Info = songInfo;
        songData.Notes = notes.ToArray();
        return new SongValidationResult { Valid = true, Message = "Song is valid", Data = songData };
    }
}