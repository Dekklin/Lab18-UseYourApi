using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18UseYourApi.Models
{
    public class ToDo
    {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, Display(Name = "Are You Finished Yet?")]
        public bool Finished { get; set; }

        [Required, Display(Name = "To-do List")]
        public int ListID { get; set; }

        public ToDoList ToDoList { get; set; }
    }
}
