using System;
using System.Collections.Generic;

namespace _05SQL.Models
{
    public partial class Appointment
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}
