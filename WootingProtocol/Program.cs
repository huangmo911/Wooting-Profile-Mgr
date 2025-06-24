using HidSharp;
using System;
using System.Linq;

namespace WootingProtocol
{
    public static class Program
    {
        static void Main(string[] args)
        {
            foreach (var hidDevice in DeviceList.Local.GetHidDevices(0x31e3).Where(d => d.GetUsageInfo()?.UsagePage == 4919))
            {
                if (!hidDevice.TryOpen(out var device)) continue;

                try
                {
                    Console.WriteLine(device.Device);

                    var wootingProtocol = new Protocol.WootingProtocol(device);
                    if (!wootingProtocol.Ping())
                        return;

                    wootingProtocol.Save($"{hidDevice.GetFriendlyName()}.JlaiWootingProfileList", false);

                    //wootingProtocol.Load("h60.JlaiWootingProfileList");
                }
                finally
                {
                    device.Close();
                }
            }
        }
    }
}
