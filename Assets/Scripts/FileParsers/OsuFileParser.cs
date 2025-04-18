using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // Stopwatch stopwatch = new Stopwatch();
        // stopwatch.Start();
        
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
                            songInfo.SongPreviewStart = float.Parse(line.Split(':')[1]);        

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
                            
                            if (circleSize != 3 && circleSize != 5 && circleSize != 8)
                                return new SongValidationResult { Valid = false, Message = $"Invalid lane count. Expected 3, 5 or 8, found {circleSize}.", Data = songData };
    
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
                        // BPM
                        string[] timingpoint = line.Split(',');
                        if (timingpoint.Length == 8 && timingpoint[6] == "1") {
                            songInfo.BPM = 60000f / float.Parse(timingpoint[1], System.Globalization.CultureInfo.InvariantCulture);
                        }
                        // TODO, calculate "BPM most of the time"

                        if (line.StartsWith("[HitObjects]"))
                            state = ParseState.HitObjects;
                        break;
                    case ParseState.HitObjects:
                        string[] lineNote = line.Split(',');
                        if (i == lines.Length - 1)
                            state = ParseState.Done;
                            
                        if (lineNote.Length != 6) break;
                        string[] lineNoteParams = lineNote[5].Split(':');
                        
                        if (fastPass) {
                            songInfo.SongStart = int.Parse(lineNote[2])/1000f;
                            int j = 0;
                            string[] lastLine;
                            do {
                                j++;
                                lastLine = lines[^j].Split(',');
                            } while (lastLine.Length != 6 && lines.Length - j >= i);
                            if (lines.Length - j < i) throw new Exception("Invalid file format, could not find end of notes (fastpass).");
                            float lastNoteTime = Mathf.Max(int.Parse(lastLine[2])/1000f, (lineNote[3] == "1") ? 0f : int.Parse(lineNoteParams[0]) / 1000f); // If last note is hold, use release time
                            songInfo.Length = lastNoteTime - songInfo.SongStart + .2f;
                            songInfo.NoteCount = lines.Length - i - 1;

                            state = ParseState.Done;
                            break;
                        }
                        
                        notes.Add(new Note{
                            Id = noteId++,
                            Type = (lineNote[3] == "1") ? NoteType.Note : NoteType.Hold,
                            Lane = int.Parse(lineNote[0]) / 64,
                            HitTime = int.Parse(lineNote[2]) / 1000f,
                            ReleaseTime = (lineNote[3] == "1") ? 0 : int.Parse(lineNoteParams[0]) / 1000f
                        });
                    break;
                }
            } catch (Exception e) {
                UnityEngine.Debug.LogError($"Error parsing line: {line}, {e.Message}, {e.StackTrace}");
            }
        }
        if (state != ParseState.Done)
            return new SongValidationResult { Valid = false, Message = "File is missing sections", Data = songData };

        /* Not needed since we reuse songinfo from fastpass
        if (!fastPass) {
            songInfo.SongStart = notes[0].HitTime;
            songInfo.Length = Math.Max(notes[^1].HitTime, notes[^1].ReleaseTime) - notes[0].HitTime;
            songInfo.NoteCount = notes.Count;
        }
        */

        songInfo.SongPreviewStart = (songInfo.SongPreviewStart < 0) ? songInfo.Length/2 : songInfo.SongPreviewStart; 

        songData.Info = songInfo;
        songData.Notes = notes.ToArray();

        // stopwatch.Stop();
        // UnityEngine.Debug.Log($"Parsed {songInfo.Title} - {songInfo.DifficultyName} in {stopwatch.Elapsed.TotalMilliseconds:F3}ms {(fastPass?"(fast":"(full")}, {songData.Info.NoteCount} notes).");
        return new SongValidationResult { Valid = true, Message = "Song is valid", Data = songData };
    }
}