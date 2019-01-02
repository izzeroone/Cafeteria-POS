using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public enum AdminReRole
    {
        SoftwareAd = 1,
        CafochaAd = 2,
        CafowareAd = 3,
        HigherAd = 4
    }

    public partial class AdminRe
    {
        [NotMapped] public string DecryptedPass { get; set; }
    }
}