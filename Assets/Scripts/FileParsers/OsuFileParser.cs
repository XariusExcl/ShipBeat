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
        # if DEBUG
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        # endif
        
        SongData songData = new();
        SongInfo songInfo = new();

        string[] lines = file.text.Split('\n');
        if (!lines[0].StartsWith("osu file format v"))
        {
            return new SongValidationResult { Valid = false, Message = "Invalid file format", Data = songData };
        }
        List<Note> notes = new();
        List<TimingPoint> timingPoints = new();

        int circleSize = 0;

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
                            songInfo.SongPreviewStart = float.Parse(line.Split(':')[1])/1000f;        

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
                            circleSize = int.Parse(line.Split(':')[1]);
                            
                            if (circleSize != 4 && circleSize != 7)
                                return new SongValidationResult { Valid = false, Message = $"Invalid lane count. Expected 4 or 7, found {circleSize}.", Data = songData };

                            songInfo.LaneCount = circleSize;
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
                        if (line.StartsWith("[HitObjects]")) {
                            if (timingPoints.Count == 0)
                                return new SongValidationResult { Valid = false, Message = "No timing points found", Data = songData };

                            float lowestBPM = float.MaxValue;
                            float highestBPM = float.MinValue;
                            foreach (TimingPoint tp in timingPoints) {
                                if (tp.BPM > float.MinValue && tp.BPM < lowestBPM) lowestBPM = tp.BPM;
                                if (tp.BPM > highestBPM) highestBPM = tp.BPM;
                            }

                            if (lowestBPM == float.MaxValue || highestBPM == float.MinValue)
                                return new SongValidationResult { Valid = false, Message = "No timing points found", Data = songData };

                            if (lowestBPM == highestBPM)
                                songInfo.BPM = lowestBPM.ToString("F0");
                            else
                                songInfo.BPM = $"{lowestBPM:F0} - {highestBPM:F0}";

                            songData.TimingPoints = timingPoints.ToArray();
                            
                            state = ParseState.HitObjects;
                            break;
                        }
                            
                        TimingPoint timingPoint = new TimingPoint();
                        string[] timingpoint = line.Split(',');
                        if (timingpoint.Length != 8) break;
                        timingPoint.Time = float.Parse(timingpoint[0], System.Globalization.CultureInfo.InvariantCulture) / 1000f;
                        timingPoint.BPM = (timingpoint[6] == "1") ? 60000f / float.Parse(timingpoint[1], System.Globalization.CultureInfo.InvariantCulture) : float.MinValue;
                        timingPoint.Meter = int.Parse(timingpoint[2]);
                        timingPoint.KiaiMode = (timingpoint[7] == "1") ? true : false;
                        timingPoints.Add(timingPoint);

                        break;
                    case ParseState.HitObjects:
                        string[] lineNote = line.Split(',');
                        if (i == lines.Length - 1)
                            state = ParseState.Done;
                            
                        if (lineNote.Length != 6) break;
                        string[] lineNoteParams = lineNote[5].Split(':');
                        
                        if (fastPass) {
                            songInfo.SongStart = float.Parse(lineNote[2], System.Globalization.CultureInfo.InvariantCulture) / 1000f;
                            int j = 0;
                            string[] lastLine;
                            do {
                                j++;
                                lastLine = lines[^j].Split(',');
                            } while (lastLine.Length != 6 && lines.Length - j >= i);
                            if (lines.Length - j < i) throw new Exception("Invalid file format, could not find end of notes (fastpass).");
                            // If last note is hold (3:128), use release time
                            float lastNoteTime = (lastLine[3] == "1") ? float.Parse(lastLine[2], System.Globalization.CultureInfo.InvariantCulture) / 1000f : float.Parse(lastLine[5].Split(':')[0], System.Globalization.CultureInfo.InvariantCulture) / 1000f;
                            songInfo.Length = lastNoteTime - songInfo.SongStart + Judge.MissHitWindow;
                            songInfo.NoteCount = lines.Length - i - 1;

                            state = ParseState.Done;
                            break;
                        }
                        
                        int lane = (int)Mathf.Floor(int.Parse(lineNote[0]) * circleSize / 512);

                        notes.Add(new Note{
                            Id = noteId++,
                            Type = (lane == 0 || lineNote[3] == "1") ? NoteType.Note : NoteType.Hold,
                            Lane = lane,
                            HitTime = float.Parse(lineNote[2], System.Globalization.CultureInfo.InvariantCulture) / 1000f,
                            ReleaseTime = (lineNote[3] == "1") ? 0 : float.Parse(lineNoteParams[0], System.Globalization.CultureInfo.InvariantCulture) / 1000f
                        });
                    break;
                }
            } catch (Exception e) {
                UnityEngine.Debug.LogError($"{songInfo.Title}: Error parsing line {i+1}: {e.Message}{e.StackTrace}");
            }
        }
        if (state != ParseState.Done)
            return new SongValidationResult { Valid = false, Message = "File is missing sections", Data = songData };

        if (!fastPass) {
            songInfo.SongStart = notes[0].HitTime;
            songInfo.Length = Math.Max(notes[^1].HitTime, notes[^1].ReleaseTime) - notes[0].HitTime;
            songInfo.NoteCount = notes.Count;
        }

        songInfo.SongPreviewStart = (songInfo.SongPreviewStart < 0) ? songInfo.Length/2 : songInfo.SongPreviewStart; 

        songData.Info = songInfo;
        songData.Notes = notes.ToArray();

        # if DEBUG
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Parsed {songInfo.Title} - {songInfo.DifficultyName} in {stopwatch.Elapsed.TotalMilliseconds:F3}ms {(fastPass?"(fast":"(full")}, {songData.Info.NoteCount} notes).");
        # endif
        return new SongValidationResult { Valid = true, Message = "Song is valid", Data = songData };
    }
}