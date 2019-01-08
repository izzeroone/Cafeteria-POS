using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public enum AdminReRole
    {
        SoftwareAd = 1
    }

    public partial class AdminRe
    {
        [NotMapped] public string DecryptedPass { get; set; }
    }
}