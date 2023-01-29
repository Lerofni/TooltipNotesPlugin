using System;
using System.IO;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dalamud.Logging;

namespace NotesPlugin.Windows;

public class NoteWindow : Window, IDisposable
{
    private readonly Notes notes;
    private bool focusNoteField = false;
    private string noteKey = "";
    private string text = "";

    public NoteWindow(Notes notes) : base(
        "Item Note", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.notes = notes;
        Flags = ImGuiWindowFlags.AlwaysAutoResize;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        // thanks to MidoriKami from the Discord for the keyboard focus
        // Reference: https://github.com/KazWolfe/Mappy/blob/970e5ce6888d19dd9e2b9b6568c70cea71c4f059/Mappy/UserInterface/Components/MapSelect.cs#L46
        if (focusNoteField)
        {
            ImGui.SetKeyboardFocusHere();
            focusNoteField = false;
        }

        ImGui.PushItemWidth(350);
        var enterPressed = ImGui.InputText("", ref text, 1000, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll);
        ImGui.PopItemWidth();

        // Check if the user pressed ESC
        // https://github.com/ocornut/imgui/issues/2620#issuecomment-501136289
        if (ImGui.IsItemDeactivated() && ImGui.IsKeyPressed(ImGuiKey.Escape))
        {
            IsOpen = false;
        }
        else
        {
            if (ImGui.Button("Save") || enterPressed)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    notes[noteKey] = text;
                }
                else
                {
                    notes.Remove(noteKey);
                }
                IsOpen = false;

                // TODO: trigger a tooltip refresh for a better controller experience
            }
        }
    }

    public void Edit(string noteKey)
    {
        IsOpen = true;
        focusNoteField = true;

        this.noteKey = noteKey;
        if (notes.ContainsKey(noteKey))
        {
            text = notes[noteKey];
        }
        else
        {
            text = "";
        }
    }
}
