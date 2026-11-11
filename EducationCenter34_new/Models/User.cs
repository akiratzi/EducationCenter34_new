using System;
using System.Collections.Generic;

namespace EducationCenter34_new.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int EmployeesId { get; set; }

    public int RoleId { get; set; }

    public virtual Employee Employees { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
