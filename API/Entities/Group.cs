using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Entities
{
    public class Group
    {
        public Group(string name)
        {
            Name = name;
        }

        [Key] //makes primary index
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}