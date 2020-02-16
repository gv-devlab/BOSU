using System.Security.Principal;

namespace BOSU
{
    class PermissionsValidator
    {
        public static bool IsAdministrator =>
            new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);
    }
}
