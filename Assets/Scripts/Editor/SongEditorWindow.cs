#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SongEditorWindow : EditorWindow
{
    private SongData targetSong;
    private Vector2 scrollPos;

    private static readonly string[] laneNames = { "Lane 0 (key 1)", "Lane 1 (key 2)", "Lane 2 (key 3)" };
    private static readonly string[] colorNames = { "0 - White (neutral)", "1 - Brown (left arrow)", "2 - Green (down arrow)", "3 - Purple (right arrow)" };

    private static readonly Color[] rowColors =
    {
        new Color(0.85f, 0.85f, 0.85f), // white
        new Color(0.75f, 0.55f, 0.35f), // brown
        new Color(0.45f, 0.80f, 0.45f), // green
        new Color(0.65f, 0.45f, 0.85f), // purple
    };

    [MenuItem("Tools/Song Editor")]
    public static void Open() => GetWindow<SongEditorWindow>("Song Editor");

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        targetSong = (SongData)EditorGUILayout.ObjectField("Song Data", targetSong, typeof(SongData), false);

        if (targetSong == null)
        {
            EditorGUILayout.HelpBox("Drag a SongData ScriptableObject above.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(4);
        DrawToolbar();
        EditorGUILayout.Space(4);
        DrawLegend();
        EditorGUILayout.Space(4);
        DrawNoteList();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+ Add Note", GUILayout.Height(28)))
        {
            Undo.RecordObject(targetSong, "Add Note");
            float t = targetSong.notes.Count > 0
                ? targetSong.notes[targetSong.notes.Count - 1].time + 0.5f
                : 1f;
            targetSong.notes.Add(new NoteData { time = t, lane = 0, noteFret = 0 });
            EditorUtility.SetDirty(targetSong);
        }

        if (GUILayout.Button("Sort by Time", GUILayout.Height(28)))
        {
            Undo.RecordObject(targetSong, "Sort Notes");
            targetSong.notes.Sort((a, b) => a.time.CompareTo(b.time));
            EditorUtility.SetDirty(targetSong);
        }

        if (GUILayout.Button("Validate & Fix", GUILayout.Height(28)))
        {
            Undo.RecordObject(targetSong, "Validate Notes");
            int fixed_ = 0;
            for (int i = 0; i < targetSong.notes.Count; i++)
            {
                var n = targetSong.notes[i];
                bool changed = false;
                if (n.lane < 0 || n.lane > 2) { n.lane = Mathf.Clamp(n.lane, 0, 2); changed = true; }
                if (n.noteFret < 0 || n.noteFret > 3) { n.noteFret = Mathf.Clamp(n.noteFret, 0, 3); changed = true; }
                if (changed) { targetSong.notes[i] = n; fixed_++; }
            }
            EditorUtility.SetDirty(targetSong);
            Debug.Log($"[SongEditor] Fixed {fixed_} invalid notes.");
        }

        EditorGUILayout.EndHorizontal();

        string clipInfo = targetSong.music != null
            ? $"{targetSong.music.name}  ({targetSong.music.length:F1}s)"
            : "NO AUDIOCLIP ASSIGNED";
        EditorGUILayout.LabelField($"Notes: {targetSong.notes.Count}  |  Clip: {clipInfo}", EditorStyles.miniLabel);
    }

    private void DrawLegend()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Colors:", EditorStyles.miniLabel, GUILayout.Width(50));
        string[] labels = { "White", "Brown", "Green", "Purple" };
        for (int i = 0; i < 4; i++)
        {
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = rowColors[i];
            GUILayout.Button(labels[i], GUILayout.Width(60), GUILayout.Height(18));
            GUI.backgroundColor = prev;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawNoteList()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("#", EditorStyles.boldLabel, GUILayout.Width(30));
        GUILayout.Label("Time (s)", EditorStyles.boldLabel, GUILayout.Width(80));
        GUILayout.Label("Lane", EditorStyles.boldLabel, GUILayout.Width(130));
        GUILayout.Label("Color", EditorStyles.boldLabel, GUILayout.Width(180));
        GUILayout.Label("", GUILayout.Width(65));
        EditorGUILayout.EndHorizontal();

        var toRemove = new List<int>();

        for (int i = 0; i < targetSong.notes.Count; i++)
        {
            var note = targetSong.notes[i];

            int safeColor = Mathf.Clamp(note.noteFret, 0, 3);
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = rowColors[safeColor] * 0.9f;
            EditorGUILayout.BeginHorizontal("box");
            GUI.backgroundColor = prev;

            GUILayout.Label(i.ToString(), GUILayout.Width(30));

            float newTime = EditorGUILayout.FloatField(note.time, GUILayout.Width(80));
            if (newTime != note.time)
            {
                Undo.RecordObject(targetSong, "Edit Time");
                note.time = Mathf.Max(0f, newTime);
                targetSong.notes[i] = note;
                EditorUtility.SetDirty(targetSong);
            }

            int newLane = EditorGUILayout.Popup(Mathf.Clamp(note.lane, 0, 2), laneNames, GUILayout.Width(130));
            if (newLane != note.lane)
            {
                Undo.RecordObject(targetSong, "Edit Lane");
                note.lane = newLane;
                targetSong.notes[i] = note;
                EditorUtility.SetDirty(targetSong);
            }

            int newColor = EditorGUILayout.Popup(Mathf.Clamp(note.noteFret, 0, 3), colorNames, GUILayout.Width(180));
            if (newColor != note.noteFret)
            {
                Undo.RecordObject(targetSong, "Edit Color");
                note.noteFret = newColor;
                targetSong.notes[i] = note;
                EditorUtility.SetDirty(targetSong);
            }

            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Remove", GUILayout.Width(65)))
                toRemove.Add(i);
            GUI.backgroundColor = prev;

            EditorGUILayout.EndHorizontal();
        }

        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            Undo.RecordObject(targetSong, "Remove Note");
            targetSong.notes.RemoveAt(toRemove[i]);
            EditorUtility.SetDirty(targetSong);
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif