using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.ViewModel.Authorization
{
    public class ChangeVM
    {
        [Required,DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required,DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
