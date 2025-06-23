# Wooting键盘通信协议研究
通过逆向[https://wootility.io/](https://wootility.io/)中的JS代码，得到如下功能！解决一些问题！

---

![](https://img.shields.io/badge/QQ-1727532-blue)

# 目前实现了什么
+ 对于键盘Base的保存恢复
+ 对特殊设置的触发键程的保存恢复
+ 快速触发的相关信息保存和恢复

# 待实现
+ 灯光

# 构建
+ VS2022
+ .NET

# 官方网页驱动原理分析
+ sendCommandWithResponse 发送并等待返回值
+ inputreport事件收到设备的数据后存入队列
+ 按键信息分为两层，一层是Base 就是大多数按键的配置，然后是一些单独设置的按键覆盖掉Base的配置

## 有哪些项
+ Base 全部按键的基础参数
+ 单独设置按键的数据

## 获取键盘的数据
1. 构建命令数据
2. 发送数据
3. 接收并解析

## 设置键盘的数据
1. 构建命令数据
2. 发送数据

## 相关数据
```csharp
var KeyboardCommands = (Lo => (Lo[Lo.Ping = 0] = "Ping",
    Lo[Lo.GetVersion = 1] = "GetVersion",
    Lo[Lo.ResetToBootloader = 2] = "ResetToBootloader",
    Lo[Lo.GetSerial = 3] = "GetSerial",
    Lo[Lo.GetRgbProfileCount = 4] = "GetRgbProfileCount",
    Lo[Lo.REMOVED_GetCurrentRgbProfileIndex = 5] = "REMOVED_GetCurrentRgbProfileIndex",
    Lo[Lo.REMOVED_GetRgbMainProfile = 6] = "REMOVED_GetRgbMainProfile",
    Lo[Lo.ReloadProfile0 = 7] = "ReloadProfile0",
    Lo[Lo.SaveRgbProfile = 8] = "SaveRgbProfile",
    Lo[Lo.GetDigitalProfilesCount = 9] = "GetDigitalProfilesCount",
    Lo[Lo.GetAnalogProfilesCount = 10] = "GetAnalogProfilesCount",
    Lo[Lo.GetCurrentKeyboardProfileIndex = 11] = "GetCurrentKeyboardProfileIndex",
    Lo[Lo.GetDigitalProfile = 12] = "GetDigitalProfile",
    Lo[Lo.GetAnalogProfileMainPart = 13] = "GetAnalogProfileMainPart",
    Lo[Lo.GetAnalogProfileCurveChangeMapPart1 = 14] = "GetAnalogProfileCurveChangeMapPart1",
    Lo[Lo.GetAnalogProfileCurveChangeMapPart2 = 15] = "GetAnalogProfileCurveChangeMapPart2",
    Lo[Lo.GetNumberOfKeys = 16] = "GetNumberOfKeys",
    Lo[Lo.GetMainMappingProfile = 17] = "GetMainMappingProfile",
    Lo[Lo.GetFunctionMappingProfile = 18] = "GetFunctionMappingProfile",
    Lo[Lo.GetDeviceConfig = 19] = "GetDeviceConfig",
    Lo[Lo.GetAnalogValues = 20] = "GetAnalogValues",
    Lo[Lo.KeysOff = 21] = "KeysOff",
    Lo[Lo.KeysOn = 22] = "KeysOn",
    Lo[Lo.ActivateProfile = 23] = "ActivateProfile",
    Lo[Lo.getDKSProfile = 24] = "getDKSProfile",
    Lo[Lo.doSoftReset = 25] = "doSoftReset",
    Lo[Lo.REMOVED_GetRgbColorsPart1 = 26] = "REMOVED_GetRgbColorsPart1",
    Lo[Lo.REMOVED_GetRgbColorsPart2 = 27] = "REMOVED_GetRgbColorsPart2",
    Lo[Lo.REMOVED_GetRgbEffects = 28] = "REMOVED_GetRgbEffects",
    Lo[Lo.RefreshRgbColors = 29] = "RefreshRgbColors",
    Lo[Lo.WootDevSingleColor = 30] = "WootDevSingleColor",
    Lo[Lo.WootDevResetColor = 31] = "WootDevResetColor",
    Lo[Lo.WootDevResetAll = 32] = "WootDevResetAll",
    Lo[Lo.WootDevInit = 33] = "WootDevInit",
    Lo[Lo.REMOVED_GetRgbProfileBase = 34] = "REMOVED_GetRgbProfileBase",
    Lo[Lo.GetRgbProfileColorsPart1 = 35] = "GetRgbProfileColorsPart1",
    Lo[Lo.GetRgbProfileColorsPart2 = 36] = "GetRgbProfileColorsPart2",
    Lo[Lo.REMOVED_GetRgbProfileEffect = 37] = "REMOVED_GetRgbProfileEffect",
    Lo[Lo.ReloadProfile = 38] = "ReloadProfile",
    Lo[Lo.GetKeyboardProfile = 39] = "GetKeyboardProfile",
    Lo[Lo.GetGamepadMapping = 40] = "GetGamepadMapping",
    Lo[Lo.GetGamepadProfile = 41] = "GetGamepadProfile",
    Lo[Lo.SaveKeyboardProfile = 42] = "SaveKeyboardProfile",
    Lo[Lo.ResetSettings = 43] = "ResetSettings",
    Lo[Lo.SetRawScanning = 44] = "SetRawScanning",
    Lo[Lo.StartXinputDetection = 45] = "StartXinputDetection",
    Lo[Lo.StopXinputDetection = 46] = "StopXinputDetection",
    Lo[Lo.SaveDKSProfile = 47] = "SaveDKSProfile",
    Lo[Lo.GetMappingProfile = 48] = "GetMappingProfile",
    Lo[Lo.GetActuationProfile = 49] = "GetActuationProfile",
    Lo[Lo.GetRgbProfileCore = 50] = "GetRgbProfileCore",
    Lo[Lo.GetGlobalSettings = 51] = "GetGlobalSettings",
    Lo[Lo.GetAKCProfile = 52] = "GetAKCProfile",
    Lo[Lo.SaveAKCProfile = 53] = "SaveAKCProfile",
    Lo[Lo.GetRapidTriggerProfile = 54] = "GetRapidTriggerProfile",
    Lo[Lo.GetProfileMetadata = 55] = "GetProfileMetadata",
    Lo[Lo.IsFLashChipConnected = 56] = "IsFLashChipConnected",
    Lo[Lo.GetRgbLayer = 57] = "GetRgbLayer",
    Lo[Lo.GetFlashStats = 58] = "GetFlashStats",
    Lo[Lo.GetRGBBins = 59] = "GetRGBBins",
    Lo[Lo._Dev = 60] = "_Dev",
    Lo[Lo.GetFullRapidTriggerProfile = 61] = "GetFullRapidTriggerProfile",
    Lo[Lo.GetProfileCount = 62] = "GetProfileCount",
    Lo[Lo.DeleteProfile = 63] = "DeleteProfile",
    Lo[Lo.CreateProfile = 64] = "CreateProfile",
    Lo[Lo.GetExternalDataSubscriptions = 65] = "GetExternalDataSubscriptions",
    Lo[Lo.GetLEDBarProfile = 66] = "GetLEDBarProfile",
    Lo[Lo.GetGlobalLEDBarProfile = 67] = "GetGlobalLEDBarProfile",
    Lo[Lo.GetLEDBarColors = 68] = "GetLEDBarColors",
    Lo[Lo.GetUSBSpeed = 69] = "GetUSBSpeed",
    Lo[Lo.GetLEDBarProfileMetadata = 70] = "GetLEDBarProfileMetadata",
    Lo[Lo.GetGlobalLEDBarProfileMetadata = 71] = "GetGlobalLEDBarProfileMetadata",
    Lo))(KeyboardCommands || {}), 

    KeyboardReports = (Lo => (Lo[Lo.REMOVED_RgbMainPart = 0] = "REMOVED_RgbMainPart",
    Lo[Lo.REMOVED_DigitalProfileMainPart = 1] = "REMOVED_DigitalProfileMainPart",
    Lo[Lo.REMOVED_AnalogProfileMainPart = 2] = "REMOVED_AnalogProfileMainPart",
    Lo[Lo.REMOVED_AnalogProfileCurveChangeMapPart1 = 3] = "REMOVED_AnalogProfileCurveChangeMapPart1",
    Lo[Lo.REMOVED_AnalogProfileCurveChangeMapPart2 = 4] = "REMOVED_AnalogProfileCurveChangeMapPart2",
    Lo[Lo.REMOVED_MainMappingProfile = 5] = "REMOVED_MainMappingProfile",
    Lo[Lo.REMOVED_FunctionMappingProfile = 6] = "REMOVED_FunctionMappingProfile",
    Lo[Lo.DeviceConfig = 7] = "DeviceConfig",
    Lo[Lo.SetDKSProfile = 8] = "SetDKSProfile",
    Lo[Lo.RgbColorsPart = 9] = "RgbColorsPart",
    Lo[Lo.REMOVED_RgbEffects = 10] = "REMOVED_RgbEffects",
    Lo[Lo.WootDevRawReport = 11] = "WootDevRawReport",
    Lo[Lo.SerialNumber = 12] = "SerialNumber",
    Lo[Lo.REMOVED_RgbProfileBase = 13] = "REMOVED_RgbProfileBase",
    Lo[Lo.RgbProfileColorsPart1 = 14] = "RgbProfileColorsPart1",
    Lo[Lo.RgbProfileColorsPart2 = 15] = "RgbProfileColorsPart2",
    Lo[Lo.REMOVED_RgbProfileEffect = 16] = "REMOVED_RgbProfileEffect",
    Lo[Lo.KeyboardProfile = 17] = "KeyboardProfile",
    Lo[Lo.GamepadMapping = 18] = "GamepadMapping",
    Lo[Lo.GamepadProfile = 19] = "GamepadProfile",
    Lo[Lo.MappingProfile = 20] = "MappingProfile",
    Lo[Lo.ActuationProfile = 21] = "ActuationProfile",
    Lo[Lo.RgbProfileCore = 22] = "RgbProfileCore",
    Lo[Lo.GlobalSettings = 23] = "GlobalSettings",
    Lo[Lo.AKCProfile = 24] = "AKCProfile",
    Lo[Lo.RapidTriggerProfile = 25] = "RapidTriggerProfile",
    Lo[Lo.ProfileMetadata = 26] = "ProfileMetadata",
    Lo[Lo.RgbLayer = 27] = "RgbLayer",
    Lo[Lo.RGBBins = 28] = "RGBBins",
    Lo[Lo.FullRapidTriggerProfile = 29] = "FullRapidTriggerProfile",
    Lo[Lo.RGBLedBarReport = 30] = "RGBLedBarReport",
    Lo[Lo.ExternalDataSubscriptionsUpdate = 31] = "ExternalDataSubscriptionsUpdate",
    Lo[Lo.LEDBarProfile = 32] = "LEDBarProfile",
    Lo[Lo.GlobalLEDBarProfile = 33] = "GlobalLEDBarProfile",
    Lo[Lo.LEDBarProfileMetadata = 34] = "LEDBarProfileMetadata",
    Lo[Lo.GlobalLEDBarProfileMetadata = 35] = "GlobalLEDBarProfileMetadata",
    Lo))(KeyboardReports || {});
```

```csharp
        getBuffer() {
            const To = new Uint8Array(PACKETLENGTH);
            return To.set(intToLittleEndianArray(this.magicWord, 2), 0),
            To[2] = this.reportId,
            this.profileReport ? "includeLength"in this.profileReport ? (To[3] = this.body.length,
            To.set(this.body, 4)) : (To[3] = (this.profileReport.save ? 1 : 0) | (this.profileReport.profileIndex ?? 0) << 1,
            this.profileReport.param1 !== void 0 ? (To[4] = this.profileReport.param1,
            To[5] = 0,
            To[6] = 0,
            To[7] = this.body.length,
            To.set(this.body, 8)) : (To[4] = this.body.length,
            To.set(this.body, 5))) : To.set(this.body, 3),
            To
        }
```

# 
