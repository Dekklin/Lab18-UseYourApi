using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18UseYourApi.Models
{
    public class ToDoList
    {
        public int ID { get; set; }

        [Required, Display(Name = "Name of List")]
        public string Name { get; set; }

        [Display(Name = "To-Do's")]
        public List<ToDo> Contents { get; set; }
}
}
