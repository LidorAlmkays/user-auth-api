using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class TokensGeneratedDTO
    {
        public string Authorization { get; set; } //AccessToken
        public string RefreshToken { get; set; } //RefreshToken
    }
}