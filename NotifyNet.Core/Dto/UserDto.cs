using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyNet.Core.Dto;

public class UserDto
{
    public string Email { get; set; }
    
    public string Password { get; set; }

    public string Name { get; set; }
}
