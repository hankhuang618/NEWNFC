using System.ComponentModel.DataAnnotations;

namespace ProductionBoardApi.Models;

public class ProductionBoardRequest
{
    [Required]
    [RegularExpression("^(ZH|VN|TC)$", ErrorMessage = "Invalid plant code.")]
    public string Plant { get; set; } = string.Empty;

    [Range(1, 6)]
    public int Division { get; set; }

    [Range(1, 4)]
    public int Section { get; set; }

    [Range(1, 10)]
    public int ClassNumber { get; set; }
}
