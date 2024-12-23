using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class GenerateTokenInfoDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}