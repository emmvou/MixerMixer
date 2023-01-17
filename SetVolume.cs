using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MixerMixer;

internal class SetVolume
{
    public static void TestMute(string[] args)
    {
        const string app = "Mozilla Firefox";

        foreach (var (name, icon) in EnumerateApplications())
        {
            Console.WriteLine("name:" + name + "; icon:" + icon);
            if (name == app)
            {
                // display mute state & volume level (% of master)
                Debug.WriteLine("Mute:" + GetApplicationMute(app));
                Debug.WriteLine("Volume:" + GetApplicationVolume(app));

                // mute the application
                SetApplicationMute(app, true);

                // set the volume to half of master volume (50%)
                SetApplicationVolume(app, 50);
            }
        }
    }

    public static void TestUnmute(string[] args)
    {
        const string app = "Mozilla Firefox";

        foreach (var (name, icon) in EnumerateApplications())
        {
            Console.WriteLine("name:" + name + "; icon:" + icon);
            if (name == app)
            {
                // display mute state & volume level (% of master)
                Debug.WriteLine("unMute:" + GetApplicationMute(app));
                Debug.WriteLine("Volume:" + GetApplicationVolume(app));

                // mute the application
                SetApplicationMute(app, false);

                // set the volume to half of master volume (50%)
                SetApplicationVolume(app, 50);
            }
        }
    }

    public static void TestSetVolume(string app, float level)
    {
        foreach (var (name, icon) in EnumerateApplications())
        {
            Console.WriteLine("name:" + name + "; icon:" + icon);
            if (name == app)
            {
                // display mute state & volume level (% of master)
                Debug.WriteLine("unMute:" + GetApplicationMute(app));
                Debug.WriteLine("Volume:" + GetApplicationVolume(app));

                SetApplicationMute(app, false);

                // todo do i have to check if the volume < 100 ?
                SetApplicationVolume(app, level);
            }
        }
    }

    public static float? TestGetVolume(string app)
    {
        return GetApplicationVolume(app);
    }

    public static float? GetApplicationVolume(string name)
    {
        var volume = GetVolumeObject(name);
        if (volume == null)
            return null; //todo error maybe

        volume.GetMasterVolume(out var level);
        return level * 100;
    }

    public static bool? GetApplicationMute(string name)
    {
        var volume = GetVolumeObject(name);
        if (volume == null)
            return null;

        volume.GetMute(out var mute);
        return mute;
    }

    public static void SetApplicationVolume(string name, float level)
    {
        var volume = GetVolumeObject(name);
        if (volume == null)
            return;

        var guid = Guid.Empty;
        volume.SetMasterVolume(level / 100, ref guid);
    }

    public static void SetApplicationMute(string name, bool mute)
    {
        var volume = GetVolumeObject(name);
        if (volume == null)
            return;

        var guid = Guid.Empty;
        volume.SetMute(mute, ref guid);
    }

    public static IEnumerable<(string, string)> EnumerateApplications()
    {
        // get the speakers (1st render + multimedia) device
        var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
        deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out var speakers);

        // activate the session manager. we need the enumerator
        var IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
        speakers.Activate(ref IID_IAudioSessionManager2, 0, nint.Zero, out var o);
        var mgr = (IAudioSessionManager2)o;

        // enumerate sessions for on this device
        mgr.GetSessionEnumerator(out var sessionEnumerator);
        sessionEnumerator.GetCount(out var count);

        for (var i = 0; i < count; i++)
        {
            sessionEnumerator.GetSession(i, out var ctl);
            ctl.GetDisplayName(out var dn);
            ctl.GetIconPath(out var icon);
            yield return (dn, icon);
            Marshal.ReleaseComObject(ctl);
        }

        Marshal.ReleaseComObject(sessionEnumerator);
        Marshal.ReleaseComObject(mgr);
        Marshal.ReleaseComObject(speakers);
        Marshal.ReleaseComObject(deviceEnumerator);
    }

    private static ISimpleAudioVolume? GetVolumeObject(string name)
    {
        // get the speakers (1st render + multimedia) device
        var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
        deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out var speakers);

        // activate the session manager. we need the enumerator
        var IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
        speakers.Activate(ref IID_IAudioSessionManager2, 0, nint.Zero, out var o);
        var mgr = (IAudioSessionManager2)o;

        // enumerate sessions for on this device
        mgr.GetSessionEnumerator(out var sessionEnumerator);
        sessionEnumerator.GetCount(out var count);

        // search for an audio session with the required name
        // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
        ISimpleAudioVolume? volumeControl = null;
        for (var i = 0; i < count; i++)
        {
            sessionEnumerator.GetSession(i, out var ctl);
            ctl.GetDisplayName(out var dn);
            if (string.Compare(name, dn, StringComparison.OrdinalIgnoreCase) == 0)
            {
                volumeControl = ctl as ISimpleAudioVolume;
                break;
            }

            Marshal.ReleaseComObject(ctl);
        }

        Marshal.ReleaseComObject(sessionEnumerator);
        Marshal.ReleaseComObject(mgr);
        Marshal.ReleaseComObject(speakers);
        Marshal.ReleaseComObject(deviceEnumerator);
        return volumeControl;
    }

    public static IEnumerable<string> EnumerateApplicationsPath()
    {
        // get the speakers (1st render + multimedia) device
        var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
        deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out var speakers);

        // activate the session manager. we need the enumerator
        var IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
        speakers.Activate(ref IID_IAudioSessionManager2, 0, nint.Zero, out var o);
        var mgr = (IAudioSessionManager2)o;

        // enumerate sessions for on this device
        mgr.GetSessionEnumerator(out var sessionEnumerator);
        sessionEnumerator.GetCount(out var count);

        for (var i = 0; i < count; i++)
        {
            sessionEnumerator.GetSession(i, out var ctl);
            ctl.GetDisplayName(out var dn);
            ctl.GetIconPath(out var icon);
            ctl.GetProcessId(out var pid);
            var proc = Process.GetProcessById((int)pid);

            string? p = null;
            try
            {
                p = proc.MainModule.FileName;
            }
            catch
            {
                p = "INVALID";
            }

            if (p is { } s)
            {
                yield return s;
            }

            Marshal.ReleaseComObject(ctl);
        }

        Marshal.ReleaseComObject(sessionEnumerator);
        Marshal.ReleaseComObject(mgr);
        Marshal.ReleaseComObject(speakers);
        Marshal.ReleaseComObject(deviceEnumerator);
    }
}