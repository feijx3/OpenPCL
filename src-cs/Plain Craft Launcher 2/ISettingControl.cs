using System;

namespace PCL
{
    public interface ISettingControl
    {
        void RefreshSetting(string NewValue);

        string GetCurrentSetting();
    }
}
