using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace IdentityTaskProject.Models;

public partial class Task
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    public string? Userid { get; set; }
}
