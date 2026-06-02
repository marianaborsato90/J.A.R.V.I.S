using System;
using System.Diagnostics;

namespace Jarvis.Core.Services;

public interface IApplicationLauncher {
    void OpenDiscord();
    void OpenSteam();
    void OpenSpotify();
    void OpenChrome(string? url = null);
    void OpenApplication(string applicationName);
}

