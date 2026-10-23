using System;
using System.Security.Principal;

namespace MeloongCore
{
    public static class WindowsUtils
    {
        public static bool HasAdminRole()
        {
            bool flag;
            using (WindowsIdentity current = WindowsIdentity.GetCurrent())
            {
                flag = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
            }
            return flag;
        }
    }
}
