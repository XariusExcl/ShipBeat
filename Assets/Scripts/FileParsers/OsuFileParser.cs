using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
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
    HitObjects
};

public class OsuFileParser
{
    public static SongValidationResult ParseFile(TextAsset file)
    {
        SongData songData = new SongData();
        SongInfo songInfo = new SongInfo();

        // FIXME: Does not work in build!
        // The whole "Reading into the Resources Folder" thing doesn't scale well, and can't read files from the machine using system APIs in WebGL.
        // will have to dig in the StreamingAssets thing, it's basically making webrequests to the local machine, but it's the only way iirc.
        // Could also use AssetBundle?
        songInfo.ChartFile = AssetDatabase.GetAssetPath(file).Replace("Assets/Resources/", "").Replace(".txt", "");

        string[] lines = file.text.Split('\n');
        if (!lines[0].StartsWith("osu file format v"))
        {
            return new SongValidationResult { Valid = false, Message = "Invalid file format", Data = songData };
        }
        List<Note> notes = new();
        ParseState state = ParseState.None;
        int noteId = 0;
        foreach (string line in lines) {
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
                            if (circleSize != 8){
                                return new SongValidationResult { Valid = false, Message = $"Invalid lane count. Expected 8, found {circleSize}.", Data = songData };;
                            }
                        }
                        if (line.StartsWith("[Events]"))
                            state = ParseState.Events;
                        break;
                    case ParseState.Events:
                        if (line.StartsWith("0,0,\"")) // Probably not the best way to check for background image, todo: test with a song that has storyboard
                            songInfo.BackgroundImage = line.Split(',')[2].Trim();

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
                        if (lineNote.Length != 6) break;
                        notes.Add(new Note{
                            Id = noteId++,
                            Type = NoteType.Note,
                            Lane = int.Parse(lineNote[0])/ 64,
                            HitTime = float.Parse(lineNote[2])/1000f
                        });
                        break;
                }
            } catch (System.Exception e) {
                Debug.LogError($"Error parsing line: {line}, {e.InnerException}");
            }
        }
        if (state != ParseState.HitObjects)
        {
            return new SongValidationResult { Valid = false, Message = "File is missing sections", Data = songData };
        }
        songInfo.SongStart = notes[0].HitTime;
        songInfo.Length = notes[notes.Count - 1].HitTime - notes[0].HitTime;
        songInfo.NoteCount = notes.Count;

        songData.Info = songInfo;
        songData.Notes = notes.ToArray();
        return new SongValidationResult { Valid = true, Message = "Song is valid", Data = songData };
    }
}