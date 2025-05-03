using Dalamud.Plugin;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Common;

public static class CrashLogger
{
    private static IDalamudPluginInterface _plugin = null!;

    private static bool    _isCrashed    = false;
    private static string  _crashTitle   = string.Empty;
    private static string  _crashText    = string.Empty;
    private static string? _exceptionTxt = null;

    private static string _defaultCrashTitle = "Umbra Error";
    private static string _defaultCrashText  = "An uncaught exception occurred in Umbra.";

    private static string _crashHintText =
        "If you have any custom plugins installed, please disable them and try again. If the issue persists, please report it to the Umbra team by joining our Discord server and reporting the issue in the #support channel. Please include the following information:\n\n" +
        "\t1. The error message displayed below.\n" +
        "\t2. A description of what you were doing when the error occurred.\n" +
        "\t3. Any steps you took to try to resolve the issue.\n\n" +
        "Thank you for your help!";

    public delegate Task GuardTask();

    internal static void Initialize(IDalamudPluginInterface plugin)
    {
        _plugin                =  plugin;
        _plugin.UiBuilder.Draw += OnDraw;
    }

    internal static void Dispose()
    {
        _plugin.UiBuilder.Draw -= OnDraw;
    }

    public static async Task Guard(GuardTask action)
    {
        await Guard(null, null, action);
    }

    public static async Task Guard(string? crashTitle, GuardTask action)
    {
        await Guard(crashTitle, null, action);
    }

    public static async Task Guard(string? crashWindowTitle, string? crashWindowMessage, GuardTask action)
    {
        _crashTitle = crashWindowTitle ?? _defaultCrashTitle;
        _crashText  = crashWindowMessage ?? _defaultCrashText;

        try {
            await action.Invoke();
        } catch (Exception ex) {
            Scheduler.Stop();

            _isCrashed    = true;
            _exceptionTxt = CreateErrorString(ex);
        }
    }

    private static void OnDraw()
    {
        if (false == _isCrashed) return;

        Vector2 wp = ImGui.GetMainViewport().WorkPos;
        Vector2 ws = ImGui.GetMainViewport().WorkSize;
        Vector2 sz = new(800, 500);

        Vector2 windowPos = new(
            wp.X + (ws / 2f).X - (sz.X / 2f),
            wp.Y + (ws / 2f).Y - (sz.Y / 2f)
        );

        ImGui.SetNextWindowSize(sz, ImGuiCond.Always);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.Always);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, 0xFF212021);
        ImGui.PushStyleColor(ImGuiCol.Border, 0xFF585A8A);
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, 0xFF282A8F);
        ImGui.PushStyleColor(ImGuiCol.TitleBg, 0xFF282A8F);
        ImGui.PushStyleColor(ImGuiCol.Text, 0xFFD7DADA);
        ImGui.PushStyleColor(ImGuiCol.TextDisabled, 0xFFAFB2B2);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 8));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 5);

        ImGui.Begin(_crashTitle, CrashWindowFlags);
        ImGui.Text(_crashText);
        ImGui.TextWrapped(_crashHintText);

        Vector2 size = ImGui.GetContentRegionAvail();

        ImGui.BeginChild("##CrashText", new Vector2(size.X, size.Y - 30), true, ImGuiWindowFlags.AlwaysVerticalScrollbar | ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.TextWrapped(_exceptionTxt);
        ImGui.EndChild();


        if (ImGui.Button("Copy error text")) {
            ImGui.SetClipboardText(_exceptionTxt);
        }

        ImGui.SameLine();
        if (ImGui.Button("Discord")) {
            Util.OpenLink("https://discord.gg/xaEnsuAhmm");
        }

        const string closeStr = "Close this dialog";
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize(closeStr).X - 4);

        if (ImGui.Button(closeStr)) {
            _isCrashed    = false;
            _exceptionTxt = null;
        }

        ImGui.End();

        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(4);
    }

    private static ImGuiWindowFlags CrashWindowFlags =>
        ImGuiWindowFlags.NoResize |
        ImGuiWindowFlags.NoMove |
        ImGuiWindowFlags.NoCollapse |
        ImGuiWindowFlags.Modal;

    private static string CreateErrorString(Exception exception)
    {
        List<Exception> el    = GetExceptionList(exception);
        StringBuilder   sb    = new();
        int             count = 1;

        foreach (Exception ex in el) {
            sb.AppendLine($"Exception {count}: {ex.Message}");
            sb.AppendLine(ex.StackTrace ?? "-- No stack trace available --");
            sb.AppendLine("");
            sb.AppendLine("");
            count++;
        }

        return sb.ToString();
    }

    private static List<Exception> GetExceptionList(Exception exception)
    {
        List<Exception> exceptions = [];

        Exception e = exception;

        int count = 0;

        while (true) {
            exceptions.Add(e);
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace);
            count++;

            if (e.InnerException != null) {
                e = e.InnerException;
                Logger.Error("---------------------------------");

                if (count > 10) break;
            } else {
                break;
            }
        }

        exceptions.Reverse();

        return exceptions;
    }
}
